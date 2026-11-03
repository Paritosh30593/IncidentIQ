using System;
using System.Threading;
using System.Threading.Tasks;
using IC.Application.ServiceContracts.Common;
using IC.Domain.Common;
using IC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IC.Infrastructure.Persistence.Interceptors
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditSaveChangesInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService
                ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyAuditInfo(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAuditInfo(DbContext context)
        {
            if (context is null)
            {
                return;
            }

            DateTime utcNow = DateTime.UtcNow;

            foreach (EntityEntry<IHasCreatedAt> entry in context.ChangeTracker.Entries<IHasCreatedAt>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = utcNow;
                }
            }

            foreach (EntityEntry<IHasUpdatedAt> entry in context.ChangeTracker.Entries<IHasUpdatedAt>())
            {
                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = utcNow;
                }
            }

            Guid? userId = _currentUserService.UserId;

            foreach (EntityEntry<User> entry in context.ChangeTracker.Entries<User>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.UpdatedBy = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedBy = userId;
                }
            }
        }
    }
}
