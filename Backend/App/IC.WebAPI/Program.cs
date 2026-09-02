using IC.WebAPI.StartupExtensions;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();

WebApplication app = builder.Build();
app.ConfigureApplications();
