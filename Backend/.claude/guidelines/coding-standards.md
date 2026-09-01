# Coding Standards

## Language & Framework

- **Language**: C#
- **Framework**: .NET 6+ (or current LTS)
- **Target**: Azure-compatible, async-first patterns

## Naming Conventions

- **Classes, Methods, Properties**: PascalCase

  ```csharp
  public class UserService
  public async Task<User> GetUserByIdAsync(int id)
  public int Id { get; set; } // Primary key of the user
  public string Name { get; set; } // Name of the user
  public DateTime ValidFrom { get; set; } // Table versioning: record valid from date
  public DateTime ValidTo { get; set; } // Table versioning: record valid to date
  ```

- **Local Variables, Parameters**: camelCase

  ```csharp
  int userId = request.Id;
  string userName = GetName(userId);
  ```

- **Constants**: UPPER_SNAKE_CASE

  ```csharp
  private const int MAX_RETRIES = 3;
  private const string API_KEY = "key";
  ```

- **Private Fields**: \_camelCase

  ```csharp
  private readonly IUserRepository _userRepository;
  private string _cachedValue;
  ```

- **Interfaces**: IPascalCase
  ```csharp
  public interface IUserService
  public interface IRepository<T>
  ```

## Code Style

- **Async/Await**: Always use async patterns

  ```csharp
  public async Task<User> GetUserAsync(int id)
  public async Task ProcessAsync()
  ```

- **LINQ**: Use method syntax; keep queries readable

  ```csharp
  IQueryable<User> activeUsers = users
    .Where(u => u.IsActive)
    .OrderBy(u => u.Name)
    .ToList();
  ```

- **Null Handling**: Use null-coalescing and null-conditional operators

  ```csharp
  string name = user?.Name ?? "";
  if (user is not null) { }
  ```

- **Strings**: Use interpolation

  ```csharp
  string message = $"User {user.Name} has {user.Credits} credits";
  ```

- **Braces**: Always use braces, even for single-line statements
  ```csharp
  if (condition)
  {
    DoSomething();
  }
  ```

## Comments

- Write code that is self-documenting through clear naming
- Comments should explain **why**, not **what**
- In case DI containers using Interfaces, apply comments in the interface instead of the implementation
- Use XML documentation for public APIs:
  ```csharp
  /// <summary>
  /// Retrieves a user by their ID.
  /// </summary>
  /// <param name="id">The user ID</param>
  /// <returns>The user if found; null otherwise</returns>
  public async Task<User> GetUserByIdAsync(int id)
  {
    // Implementation here
  }
  ```

## Access Modifiers

- Default to `private`; only expose what's necessary
- Use `public` only for API contracts
- Use `internal` for cross-layer access within the assembly

## Error Handling

- Throw meaningful exceptions with descriptive messages
- Don't catch exceptions you can't handle
- Use Serilog for structured logging and store logs appropriately in logs/ directory
- Use specific exception types

  ```csharp
  if (user is null)
    throw new ArgumentNullException(nameof(user));

  try
  {
    /* operation */
  }
  catch (InvalidOperationException ex)
  {
    _logger.LogError(ex, "Failed to process");
  }
  finally
  {
    // (optional) Cleanup code here
  }
  ```

## Logging

- Use structured logging (ILogger)
- Include context in log messages
- Use appropriate log levels: Debug, Information, Warning, Error, Critical
  ```csharp
  _logger.LogInformation("User {UserId} logged in", userId);
  _logger.LogError(ex, "Database operation failed for user {UserId}", userId);
  ```

## Dependency Injection

- Register services in startup configuration
- Constructor inject all dependencies
- Avoid service locator pattern

## File Organization

- One public class per file (with exceptions for small related classes)
- Organize members: fields, properties, constructors, public methods, private methods
- Use regions sparingly

## Async Best Practices

- Use `async Task` for void-returning methods only in event handlers
- Prefer `async Task<T>` for methods returning values
- Use `ConfigureAwait(false)` in library code
- Don't use `.Result` or `.Wait()` (creates deadlock risk)

## Performance Considerations

- Use `IEnumerable<T>` for streaming data
- Use `List<T>` when you need Count or indexing
- Use IQueryable<T> for deferred execution and efficient querying
- Avoid creating unnecessary objects in loops
- Use string builders for concatenation in loops
- Always use strict typing instead of var whenever possible
- Use switch expressions where appropriate for cleaner and more concise code
- Use ternary operators for simple conditional assignments
  ```csharp
  int result = condition
    ? value1
    : value2;
  ```
