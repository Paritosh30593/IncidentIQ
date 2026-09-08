using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using IC.WebAPI.Middleware;
using Serilog;

namespace IC.WebAPI.StartupExtensions
{
    public static class ConfigureApplicationsExtensions
    {
        public static void ConfigureApplications(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseSerilogRequestLogging();

            if (app.Environment.IsProduction())
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

            app.MapControllers();

            app.Run();
        }
    }
}

