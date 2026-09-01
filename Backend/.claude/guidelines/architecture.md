# Architecture Guidelines

## Project Structure

The IncidentIQ backend follows a layered architecture pattern:

```plaintext
Backend/
├── App/                      # CLEAN architecture layers
|   ├── IC.Application/        # Application: business logic orchestration
|   ├── IC.Domain/             # Domain: entities, value objects, domain services
|   ├── IC.Infrastructure/     # Infrastructure: external services, data access implementations
|   └── IC.WebAPI/             # Web API: controllers, HTTP concerns
└── Tests/                    # Unit and integration tests
   ├── IC.Unit/                # Unit tests
   └── IC.Integration/         # Integration tests
```

## Design Principles

1. **Separation of Concerns** - Each layer has a clear responsibility
   - IC.WebAPI layer handles HTTP concerns (routing, serialization)
   - IC.Application layer orchestrates business logic
   - IC.Domain layer contains entities, value objects, and domain services
   - IC.Infrastructure layer handles external services and data access

2. **Dependency Injection** - Use constructor injection for all dependencies
   - All services should be registered in the DI container
   - Avoid service locator patterns
   - IC.Application DI container should be configured in IC.Application/ServiceExtension.cs
   - IC.Infrastructure DI container should be configured in IC.Infrastructure/ServiceExtension.cs

3. **Interface-Based Design** - Program against interfaces, not implementations
   - Define interfaces in Domain layer
   - Implement in appropriate layers
   - Facilitates testing and loose coupling

4. **Domain-Driven Design** - Keep domain logic in the Domain layer
   - Domain entities should not reference infrastructure
   - Business rules stay in IC.Domain, not scattered in Services

5. **Configuration Management**
   - Use appsettings.json and environment variables
   - Avoid hardcoding configuration values
   - IC.WebAPI contains extensions for service registration and middleware:
     - StartupExtensions/ConfigureAppExtensions.cs for WebApplication -> app (middleware)
     - StartupExtensions/ConfigureServicesExtension.cs for WebApplicationBuilder -> builder (service registration)

## Database Design

- Use Entity Framework Core conventions
- Keep migration files versioned and documented
- The Scripts folder contains raw SQL scripts for views, functions, procedures, and triggers, make sure:
  - Each script follows the naming convention: <ObjectName>\_<ObjectType>.sql
  - Include script in migration files when applicable (create or alter) to ensure database consistency
- Folder structure under IC.Infrastructure (CQRS pattern):

```plaintext
IC.Infrastructure
├── Persistence
│  ├── DbContext/
│  │   └── AppDbContext.cs
│  ├── EntityConfigurations/
│  │   └── ProductConfiguration.cs
│  ├── Functions/
│  │   ├── TopCustomer.cs
│  │   ├── SalesByMonth.cs
│  │   └── FunctionConfiguration.cs
│  ├── Views/
│  │   ├── ProductSales.cs
│  │   ├── CustomerSummary.cs
│  │   └── ViewConfiguration.cs
│  ├── Seed/
│  │   └── ProductData.cs
│  └── Scripts/ (*.sql)
│  │   ├── Views/ # contains SQL view definitions <ViewName>_View.sql
│  │   ├── Functions/ # contains SQL function definitions <FunctionName>_Function.sql
│  │   ├── Procedures/ # contains SQL procedure definitions <ProcedureName>_Procedure.sql
│  │   └── Triggers/ # contains SQL trigger definitions <TriggerName>_Trigger.sql
│  └── Migrations/
├── Queries/
│   └── ProductQueries.cs
```

## API Design

- Follow RESTful conventions where applicable
- Consistent naming: PascalCase for DTOs, camelCase in JSON responses
- Version endpoints if breaking changes are needed
- Document all public endpoints

## Common Patterns

- **CQRS (Command Query Responsibility Segregation) Pattern** - Separate read and write operations to improve scalability and maintainability
- **Repository Pattern** - Encapsulate data access logic and provide a clean API for the application layer
- **Service Pattern** - Encapsulate business logic
- **DTO Pattern** - Map between API and domain models
- **Dependency Injection** - Decouple components

## Anti-Patterns to Avoid

- Circular dependencies between layers
- Direct database queries in IC.WebAPI controllers
- Tightly coupled classes across IC layers
- God services in IC.Application or IC.Domain that do too much
- Mixing async/sync patterns without clear purpose
