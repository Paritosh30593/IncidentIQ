# Integration Testing Guidelines

## Testing Framework & Tools

- **Framework**: xUnit or NUnit
- **HTTP Testing**: xUnit with WebApplicationFactory
- **Database**: Real database instance or testcontainers
- **Assertions**: FluentAssertions
- **Test Data**: Seed data via migrations or builders

## Test Structure

```csharp
public class UserIntegrationTests : IAsyncLifetime
{
  private readonly WebApplicationFactory<Program> _factory;
  private HttpClient _client;

  public UserIntegrationTests()
  {
    _factory = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(builder =>
      {
        // Configure test database, mocks, etc.
      });
  }

  public async Task InitializeAsync()
  {
    _client = _factory.CreateClient();
    // Setup test data
  }

  public async Task DisposeAsync()
  {
    _client?.Dispose();
    _factory?.Dispose();
  }

  [Fact]
  public async Task CreateUser_WithValidData_ReturnsCreatedStatusAndUser()
  {
    // Arrange
    var request = new CreateUserRequest { Name = "John", Email = "john@example.com" };

    // Act
    var response = await _client.PostAsJsonAsync("/api/users", request);

    // Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    var content = await response.Content.ReadAsAsync<UserDto>();
    content.Name.Should().Be("John");
  }
}
```

## Scope of Integration Tests

Integration tests verify interactions between multiple components:

- **API Endpoints** - Full request/response cycle
- **Database Operations** - Data persistence and retrieval
- **Service Orchestration** - Multiple services working together
- **External Dependencies** - API calls, message queues, etc. (with mocks/stubs for external systems)

**NOT Unit Tests** - Don't test individual methods in isolation

## Test Database Setup

### Option 1: In-Memory Database (Faster, Simpler)

```csharp
var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
  .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
  .Options;

var context = new ApplicationDbContext(builder);
```

**Pros**: Fast, no external dependencies
**Cons**: Doesn't match production database behavior exactly

### Option 2: Real Database (More Realistic)

```csharp
var connectionString = "Server=localhost;Database=IncidentIQ_Test;...";
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
  .UseSqlServer(connectionString)
  .Options;
```

**Pros**: Tests real database behavior, migrations, constraints
**Cons**: Slower, requires database setup, needs cleanup

### Option 3: Testcontainers (Best Practice)

```csharp
private static readonly MsSqlContainer _container = new MsSqlBuilder()
  .WithPassword("YourStrongPassword123!")
  .Build();

public async Task InitializeAsync()
{
  await _container.StartAsync();
  var connectionString = _container.GetConnectionString();
  // Use connectionString for DbContext
}

public async Task DisposeAsync()
{
  await _container.StopAsync();
}
```

**Pros**: Real database, isolated per test, automatic cleanup
**Cons**: Slightly slower startup

## Test Data Management

### Seed Data Approach

```csharp
private async Task SeedTestDataAsync(ApplicationDbContext context)
{
  var users = new[]
  {
    new User { Id = 1, Name = "Alice", Email = "alice@example.com", IsActive = true },
    new User { Id = 2, Name = "Bob", Email = "bob@example.com", IsActive = false }
  };

  context.Users.AddRange(users);
  await context.SaveChangesAsync();
}
```

### Builder Pattern Approach

```csharp
private User CreateTestUser(string name = "TestUser", string email = "test@example.com")
{
  return new User
  {
    Name = name,
    Email = email,
    CreatedAt = DateTime.UtcNow,
    IsActive = true
  };
}
```

### Database Cleanup

```csharp
public async Task DisposeAsync()
{
  using var context = _factory.Services.GetRequiredService<ApplicationDbContext>();
  await context.Database.EnsureDeletedAsync();
}
```

## API Testing Best Practices

1. **Test Complete User Flows**
   ```csharp
   [Fact]
   public async Task UserCreation_ThenRetrieval_ReturnsCreatedData()
   {
     // Create user
     var createResponse = await _client.PostAsJsonAsync("/api/users", new { Name = "John" });
     var createdUser = await createResponse.Content.ReadAsAsync<UserDto>();

     // Retrieve user
     var getResponse = await _client.GetAsync($"/api/users/{createdUser.Id}");

     // Verify
     getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
   }
   ```

2. **Test Error Scenarios**
   ```csharp
   [Fact]
   public async Task GetUser_WithInvalidId_Returns404()
   {
     var response = await _client.GetAsync("/api/users/99999");
     response.StatusCode.Should().Be(HttpStatusCode.NotFound);
   }
   ```

3. **Test Authentication/Authorization**
   ```csharp
   [Fact]
   public async Task DeleteUser_WithoutAuthorization_Returns401()
   {
     var response = await _client.DeleteAsync("/api/users/1");
     response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
   }
   ```

4. **Verify Response Content**
   ```csharp
   [Fact]
   public async Task GetUsers_ReturnsPaginatedResults()
   {
     var response = await _client.GetAsync("/api/users?page=1&pageSize=10");
     var content = await response.Content.ReadAsAsync<PagedResponse<UserDto>>();
     
     content.Should().NotBeNull();
     content.TotalCount.Should().BeGreaterThan(0);
     content.Items.Should().HaveCount(10);
   }
   ```

## File Organization

```
Tests/IntegrationTests/
├── Controllers/
│   └── UserControllerTests.cs
├── Services/
│   └── UserServiceIntegrationTests.cs
├── Fixtures/
│   ├── DatabaseFixture.cs
│   └── TestDataBuilder.cs
└── Common/
    └── IntegrationTestBase.cs
```

## Common Test Base Class

```csharp
public abstract class IntegrationTestBase : IAsyncLifetime
{
  protected readonly WebApplicationFactory<Program> Factory;
  protected HttpClient Client;
  protected IServiceProvider Services;

  protected IntegrationTestBase()
  {
    Factory = new WebApplicationFactory<Program>();
    Services = Factory.Services;
  }

  public virtual async Task InitializeAsync()
  {
    Client = Factory.CreateClient();
    await Task.CompletedTask;
  }

  public virtual async Task DisposeAsync()
  {
    Client?.Dispose();
    Factory?.Dispose();
    await Task.CompletedTask;
  }

  protected async Task<T> GetScopedServiceAsync<T>() where T : notnull
  {
    return Services.GetRequiredService<T>();
  }
}
```

## Mocking External Services

Mock external APIs, message queues, etc., but use real databases:

```csharp
.WithWebHostBuilder(builder =>
{
  builder.ConfigureTestServices(services =>
  {
    // Mock external email service
    var mockEmailService = new Mock<IEmailService>();
    mockEmailService
      .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
      .ReturnsAsync(true);

    services.AddScoped(_ => mockEmailService.Object);
  });
});
```

## Performance Considerations

- Run integration tests separately from unit tests
- Consider parallel execution carefully (shared database state)
- Use `ICollectionFixture` for shared test database
- Cache database setup where appropriate
- Keep tests focused on critical paths

## CI/CD Integration

- Run integration tests in a dedicated test environment
- Use testcontainers for database isolation
- Set reasonable timeouts (integration tests are slower)
- Mock slow external services
- Log test failures with full context
