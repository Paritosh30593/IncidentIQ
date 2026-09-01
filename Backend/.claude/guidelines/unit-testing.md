# Unit Testing Guidelines

## Testing Framework & Tools

- **Framework**: xUnit or NUnit
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Test Data**: Bogus or Builder pattern

## Test Structure (Arrange-Act-Assert)

```csharp
[Fact]
public async Task GetUserById_WithValidId_ReturnsUser()
{
  // Arrange
  var userId = 1;
  var mockRepository = new Mock<IUserRepository>();
  mockRepository
    .Setup(r => r.GetUserByIdAsync(userId))
    .ReturnsAsync(new User { Id = userId, Name = "John" });
  
  var service = new UserService(mockRepository.Object);

  // Act
  var result = await service.GetUserByIdAsync(userId);

  // Assert
  result.Should().NotBeNull();
  result.Name.Should().Be("John");
}
```

## Naming Conventions

Use the pattern: `MethodName_Condition_ExpectedResult`

```csharp
[Fact]
public void Validate_WithNullEmail_ThrowsArgumentNullException()

[Fact]
public void CalculateDiscount_WithValidPercentage_ReturnsCorrectAmount()

[Fact]
public async Task CreateUser_WhenEmailExists_ThrowsDuplicateException()
```

## Unit Test Best Practices

1. **Test One Thing** - Each test should verify a single behavior
   - Don't test multiple scenarios in one test
   - One assertion per test when possible

2. **Use Descriptive Names** - Names should document what is being tested
   - Include the condition and expected outcome
   - Avoid generic names like "Test1" or "Works"

3. **Isolate External Dependencies** - Mock all dependencies
   ```csharp
  private readonly Mock<IRepository> _mockRepository;
  private readonly Mock<IEmailService> _mockEmailService;
  private readonly UserService _service;

  public UserServiceTests()
  {
    _mockRepository = new Mock<IRepository>();
    _mockEmailService = new Mock<IEmailService>();
    _service = new UserService(_mockRepository.Object, _mockEmailService.Object);
  }
   ```

4. **Use Fixtures for Setup** - Create reusable test fixtures
   ```csharp
  private User CreateValidUser()
  {
    return new User
    {
      Id = 1,
      Name = "John Doe",
      Email = "john@example.com"
    };
  }
   ```

5. **Verify Behavior, Not Implementation** - Test what, not how
   - Test public contracts, not private methods
   - Use mocks to verify calls when testing behavior

6. **Keep Tests Fast** - Use mocks instead of real I/O
   - Avoid database calls (use mocks)
   - Avoid file system operations (use mocks)
   - Avoid network calls (use mocks)

7. **Make Tests Deterministic** - No random data or timing issues
   - Use fixed test data
   - Avoid Thread.Sleep()
   - Mock time-dependent operations

8. **Test Edge Cases & Error Conditions**
   ```csharp
  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public void Validate_WithInvalidEmail_ReturnsFalse(string email)
  {
    var result = validator.Validate(email);
    result.Should().BeFalse();
  }
   ```

9. **Avoid Test Interdependencies**
   - Each test should be independent
   - Use fresh objects for each test
   - Don't share state between tests

10. **Use Data-Driven Tests** - Test multiple inputs/outputs
    ```csharp
    [Theory]
    [InlineData(0, 0)]
    [InlineData(10, 1)]
    [InlineData(100, 10)]
    public void CalculateDiscount_WithValidAmount_ReturnsExpectedDiscount(
      decimal amount, decimal expectedDiscount)
    {
      var result = calculator.CalculateDiscount(amount);
      result.Should().Be(expectedDiscount);
    }
    ```

## Test Coverage

- Target 80%+ code coverage for business logic
- Focus coverage on:
  - Complex calculations
  - Error handling paths
  - Business rules
  - Public APIs

- Lower priority for:
  - Simple getters/setters
  - Framework-generated code
  - Trivial wrappers

## File Organization

- Keep tests in `Tests/UnitTests/` directory
- Mirror the source structure
  ```
  Tests/UnitTests/
  ├── Services/
  │   └── UserServiceTests.cs
  ├── Repositories/
  │   └── UserRepositoryTests.cs
  └── Core/
      └── Validators/
          └── EmailValidatorTests.cs
  ```

- One test class per source class
- Test class name: `[SourceClassName]Tests`

## Common Testing Patterns

### Testing Exceptions

```csharp
[Fact]
public void DoSomething_WithInvalidInput_ThrowsException()
{
  var action = () => service.DoSomething(null);
  action.Should().Throw<ArgumentNullException>();
}
```

### Testing Async Methods

```csharp
[Fact]
public async Task GetUserAsync_WithValidId_ReturnsUser()
{
  var result = await service.GetUserAsync(1);
  result.Should().NotBeNull();
}
```

### Testing Collections

```csharp
[Fact]
public void GetActiveUsers_ReturnsOnlyActiveUsers()
{
  var result = service.GetActiveUsers();
  result.Should().AllSatisfy(u => u.IsActive.Should().BeTrue());
}
```
