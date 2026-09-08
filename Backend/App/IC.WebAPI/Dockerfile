# Use: docker build -t incident-iq-be -f App/IC.WebAPI/Dockerfile .
# On: C:\Users\parir\source\repos\IncidentIQ\Backend

# ======================= Build Stage ==========================

# --- Dotnet Core SDK image
FROM mcr.microsoft.com/dotnet/sdk:10.0.300 AS build

# Directory for building code on container
WORKDIR /src

# Copy only .csproj files to /src. This is a Docker cache optimization: NuGet restore runs again only when a .csproj changes, not whenever ordinary .cs source changes)
COPY ["App/IC.WebAPI/IC.WebAPI.csproj", "App/IC.WebAPI/"]
COPY ["App/IC.Application/IC.Application.csproj", "App/IC.Application/"]
COPY ["App/IC.Infrastructure/IC.Infrastructure.csproj", "App/IC.Infrastructure/"]
COPY ["App/IC.Domain/IC.Domain.csproj", "App/IC.Domain/"]

# Download all the package dependencies
RUN dotnet restore "App/IC.WebAPI/IC.WebAPI.csproj"

# Now copy all content from App except what is ignored
COPY App/ App/

# Switch directory to main api project
WORKDIR /src/App/IC.WebAPI

# Publish the project (--no-restore makes sure dotnet restore won't run again during publish)
RUN dotnet publish "IC.WebAPI.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

# ==================== Production Stage ========================

# --- Dotnet Core Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0.8 AS production

# Directory for the runtime content
WORKDIR /app

# Copy content from build image to production image
COPY --from=build /app/publish .

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Setup kestrel server to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Expose the same port to listen in final runtime container
EXPOSE 8080

# --- Set the default entry command ---
ENTRYPOINT [ "dotnet", "IC.WebAPI.dll" ]