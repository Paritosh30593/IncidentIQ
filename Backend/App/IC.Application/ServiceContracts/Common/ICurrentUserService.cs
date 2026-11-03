using System;

namespace IC.Application.ServiceContracts.Common
{
    /// <summary>
    /// Resolves the identity of the user making the current request, for audit purposes.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// The internal <see cref="IC.Domain.Entities.User"/> id of the current user, or <c>null</c> when unresolved (e.g. unauthenticated/background contexts).
        /// </summary>
        Guid? UserId { get; }
    }
}
