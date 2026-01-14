# Unit Test Template

**Category:** Testing  
**Use Case:** Generate xUnit tests for C# code  
**Maturity Level:** 2-3  
**Last Updated:** January 2026

---

## Purpose

Generate comprehensive unit tests with high coverage and meaningful assertions.

---

## Prompt Template

```markdown
Generate xUnit unit tests for the following C# code:

```csharp
{PASTE_CODE_HERE}
```

## Requirements

### Test Framework
- xUnit (latest version)
- FluentAssertions for assertions
- Moq for mocking dependencies

### Test Structure
- Use AAA pattern (Arrange, Act, Assert)
- One test class per class under test
- Name: {ClassUnderTest}Tests
- Namespace: IncidentManagement.Api.Tests.{Namespace}

### Test Coverage

Generate tests for:

1. **Happy path scenarios**
   - Valid inputs produce expected outputs
   - All public methods work correctly

2. **Edge cases**
   - Null/empty inputs
   - Boundary values (min, max)
   - Zero, negative numbers (if applicable)

3. **Error scenarios**
   - Invalid inputs throw expected exceptions
   - Validation failures handled correctly

4. **Async behavior** (if applicable)
   - Async methods tested with await
   - CancellationToken handling verified

### Naming Conventions

Use this pattern:
```
{MethodName}_{Scenario}_{ExpectedOutcome}
```

Examples:
- `GetById_ValidId_ReturnsEntity`
- `GetById_InvalidId_ReturnsNull`
- `Create_NullInput_ThrowsArgumentNullException`

### Assertions

- Use FluentAssertions syntax:
  - `result.Should().NotBeNull()`
  - `result.Should().Be(expected)`
  - `exception.Should().BeOfType<ArgumentException>()`
- Include meaningful failure messages where helpful

### Mocking

- Mock all dependencies injected via constructor
- Use Moq's Setup/Returns pattern
- Verify interactions when appropriate (e.g., service method called once)

### Test Data

- Use realistic test data (not "test", "foo", "bar")
- Consider using fixture or builder pattern for complex objects
- Avoid magic numbers/strings

## Constraints

**Do NOT:**
- Test private methods (test through public API)
- Test framework code (ASP.NET Core, Entity Framework)
- Create integration tests (use unit tests only)
- Use actual database or external services

**DO:**
- Test business logic thoroughly
- Mock external dependencies
- Use descriptive test names
- Add comments for complex test scenarios
- Follow existing test patterns in codebase

## Context

**Test Framework:** xUnit  
**Assertion Library:** FluentAssertions  
**Mocking Library:** Moq  
**Existing Examples:** See tests in {ProjectName}.Tests/  
**.NET Version:** 9.0

## Expected Output

Provide:
1. Complete test class with all using statements
2. Constructor with test fixtures (if needed)
3. 10+ test methods covering scenarios above
4. Mock setup code
5. Clear AAA sections (comments optional but helpful for complex tests)

## Example Test Structure

```csharp
using FluentAssertions;
using Moq;
using Xunit;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly OrderService _sut; // System Under Test

    public OrderServiceTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _sut = new OrderService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetById_ValidId_ReturnsOrder()
    {
        // Arrange
        var orderId = 123;
        var expectedOrder = new Order { Id = orderId, Name = "Test Order" };
        _mockRepository.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _sut.GetByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(orderId);
        result.Name.Should().Be("Test Order");
    }
    
    // ... more tests
}
```
```

---

## Usage Example

**Input:**

```csharp
public class IncidentService
{
    private readonly IIncidentRepository _repository;
    
    public IncidentService(IIncidentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    public async Task<Incident?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0) throw new ArgumentException("ID must be positive", nameof(id));
        return await _repository.GetByIdAsync(id, cancellationToken);
    }
}
```

**AI generates tests for:**
- Constructor throws on null repository
- GetByIdAsync with valid ID returns incident
- GetByIdAsync with invalid ID throws ArgumentException
- GetByIdAsync with zero throws ArgumentException
- GetByIdAsync with negative ID throws ArgumentException
- GetByIdAsync calls repository once
- GetByIdAsync passes CancellationToken to repository

---

## Expected Output Quality

AI should generate:
- ✅ Tests that compile without errors
- ✅ All using statements included
- ✅ Proper mocking setup
- ✅ Descriptive test names
- ✅ Comprehensive coverage (happy path + edge cases + errors)
- ✅ FluentAssertions syntax
- ✅ AAA pattern

Common issues to watch for:
- ❌ Missing using statements
- ❌ Tests that always pass (wrong assertions)
- ❌ Incomplete mocking (missing Setup calls)
- ❌ Testing implementation details instead of behavior
- ❌ Duplicate test scenarios

---

## After Generation

1. **Review** tests for correctness
2. **Run tests** to ensure they pass
3. **Deliberately break code** to ensure tests fail as expected (mutation testing concept)
4. **Add missing tests** if coverage gaps identified
5. **Commit** with message: `test: add {ClassName} unit tests (AI-assisted)`

---

## Advanced Variations

### Test with Test Fixtures

For tests needing shared setup:

```markdown
Add IClassFixture<{FixtureName}> to test class for shared setup.
```

### Theory Tests (Parameterized)

For testing multiple inputs:

```markdown
Use [Theory] and [InlineData] for parameterized tests where appropriate.

Example:
[Theory]
[InlineData(0)]
[InlineData(-1)]
[InlineData(-100)]
public void GetById_InvalidId_ThrowsException(int invalidId)
{
    // test implementation
}
```

### Async Testing

Ensure async methods are properly tested:

```markdown
All async methods must be tested with async Task test methods.
Use await, not .Result or .Wait().
```

---

## Code Coverage Target

- **Target:** ≥80% line coverage
- **Measure:** Use Coverlet or similar tool
- **Report:** Include in CI/CD pipeline

---

## Related Prompts

- [integration-test-template.md](integration-test-template.md) - Generate integration tests
- [test-data-builder.md](test-data-builder.md) - Generate test data builders
- [../dev/api-controller-template.md](../dev/api-controller-template.md) - Generate code to test

---

## Ownership

- **Created by:** QA + Engineering Team
- **Maintained by:** Senior Engineers
- **Last reviewed:** January 2026
- **Usage count:** TBD

---

## Feedback

If generated tests are low quality or miss important scenarios, file an issue with specifics.
