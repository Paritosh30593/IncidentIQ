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
            // Configure middleware here
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseMiddleware<ExceptionHandlingMiddleware>();
            }
            else
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
            }

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

