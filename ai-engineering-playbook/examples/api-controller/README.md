# Example: Create a New API Controller

**Scenario:** Engineer needs to create a REST API for managing Product entities  
**Maturity Level Demonstrated:** Level 3 (Configuring)  
**Time to Complete:** ~15 minutes (vs. 45 minutes manual)

---

## Context

Your team maintains an e-commerce platform. Product Management has requested a new API for managing product inventory.

**Requirements:**
- CRUD operations for Products
- Product attributes: ID, Name, SKU, Price, StockQuantity
- Standard REST conventions
- Follow existing API patterns in codebase

---

## Without AI (Traditional Approach)

### Steps

1. Create `Controllers/V2/ProductsController.cs`
2. Define route and actions manually
3. Create DTOs in `DTOs/ProductDTOs.cs`
4. Write boilerplate DI, validation, error handling
5. Add XML documentation
6. Write unit tests manually
7. Test endpoints manually

**Time:** 45-60 minutes  
**Errors:** Common mistakes (missing async, wrong status codes, incomplete validation)

---

## With AI (Playbook Approach)

### Step 1: Use API Controller Prompt Template

Navigate to: [prompts/dev/api-controller-template.md](../../prompts/dev/api-controller-template.md)

**Fill in placeholders:**

```markdown
Generate a C# ASP.NET Core Web API controller for managing Product.

## Requirements

### Namespace and Location
- Namespace: IncidentManagement.Api.Controllers.V2
- File: Controllers/V2/ProductsController.cs
- Version: V2

### Endpoints

- POST /api/v2/products (create new product)
- GET /api/v2/products/{id} (get by ID)
- GET /api/v2/products (list all, with pagination)
- PUT /api/v2/products/{id} (update existing)
- DELETE /api/v2/products/{id} (soft delete)

### Dependencies
- Inject IProductService interface
- Inject ILogger<ProductsController>

### DTOs

ProductCreateDto:
- string Name (required, max 200 chars)
- string SKU (required, unique, max 50 chars)
- decimal Price (required, >= 0)
- int StockQuantity (required, >= 0)

ProductUpdateDto:
- Same as Create

ProductDto:
- int Id
- string Name
- string SKU
- decimal Price
- int StockQuantity
- DateTime CreatedAt
- DateTime UpdatedAt

### Context
Framework: .NET 9
Existing Example: Controllers/V1/OrdersV1Controller.cs
Service Pattern: IProductService (to be implemented separately)
```

### Step 2: Submit to AI (GitHub Copilot)

Paste the prompt into Copilot Chat or use as `.copilot-instructions.md` in workspace.

**AI generates:**

<details>
<summary>ProductsController.cs (click to expand)</summary>

```csharp
using Microsoft.AspNetCore.Mvc;
using IncidentManagement.Api.DTOs;
using IncidentManagement.Api.Interfaces;

namespace IncidentManagement.Api.Controllers.V2;

/// <summary>
/// API controller for managing products
/// </summary>
[ApiController]
[Route("api/v2/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    /// <summary>
    /// Initializes a new instance of ProductsController
    /// </summary>
    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="dto">Product creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product</returns>
    /// <response code="201">Product created successfully</response>
    /// <response code="400">Invalid input</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] ProductCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var product = await _productService.CreateAsync(dto, cancellationToken);
        
        var productDto = new ProductDto(
            product.Id,
            product.Name,
            product.SKU,
            product.Price,
            product.StockQuantity,
            product.CreatedAt,
            product.UpdatedAt
        );

        return CreatedAtAction(nameof(GetByIdAsync), new { id = product.Id }, productDto);
    }

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product details</returns>
    /// <response code="200">Product found</response>
    /// <response code="404">Product not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        
        if (product == null)
            return NotFound();

        var productDto = new ProductDto(
            product.Id,
            product.Name,
            product.SKU,
            product.Price,
            product.StockQuantity,
            product.CreatedAt,
            product.UpdatedAt
        );

        return Ok(productDto);
    }

    /// <summary>
    /// Gets all products with pagination
    /// </summary>
    /// <param name="page">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetAllAsync(page, pageSize, cancellationToken);
        
        var productDtos = products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.SKU,
            p.Price,
            p.StockQuantity,
            p.CreatedAt,
            p.UpdatedAt
        ));

        return Ok(productDtos);
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="dto">Updated product data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Product updated successfully</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404">Product not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] ProductUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var updated = await _productService.UpdateAsync(id, dto, cancellationToken);
        
        if (!updated)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Soft deletes a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Product deleted successfully</response>
    /// <response code="404">Product not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _productService.DeleteAsync(id, cancellationToken);
        
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
```

</details>

<details>
<summary>ProductDTOs.cs (click to expand)</summary>

```csharp
using System.ComponentModel.DataAnnotations;

namespace IncidentManagement.Api.DTOs;

/// <summary>
/// DTO for creating a new product
/// </summary>
public record ProductCreateDto(
    [Required]
    [StringLength(200)]
    string Name,
    
    [Required]
    [StringLength(50)]
    string SKU,
    
    [Required]
    [Range(0, double.MaxValue)]
    decimal Price,
    
    [Required]
    [Range(0, int.MaxValue)]
    int StockQuantity
);

/// <summary>
/// DTO for updating an existing product
/// </summary>
public record ProductUpdateDto(
    [Required]
    [StringLength(200)]
    string Name,
    
    [Required]
    [StringLength(50)]
    string SKU,
    
    [Required]
    [Range(0, double.MaxValue)]
    decimal Price,
    
    [Required]
    [Range(0, int.MaxValue)]
    int StockQuantity
);

/// <summary>
/// DTO for product response
/// </summary>
public record ProductDto(
    int Id,
    string Name,
    string SKU,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
```

</details>

**Time to generate:** 30 seconds

### Step 3: Review Generated Code

**Check:**
- ✅ All endpoints present
- ✅ Async/await used correctly
- ✅ DTOs have validation attributes
- ✅ XML documentation complete
- ✅ Proper HTTP status codes
- ✅ CancellationToken passed through

**Issues found:** None (prompt was well-structured)

### Step 4: Add Business Logic (Service Layer)

AI generated controller, but service implementation is manual (contains business logic):

```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public async Task<Product> CreateAsync(ProductCreateDto dto, CancellationToken ct)
    {
        // Business logic: check SKU uniqueness
        var existing = await _repository.GetBySKUAsync(dto.SKU, ct);
        if (existing != null)
            throw new InvalidOperationException($"SKU {dto.SKU} already exists");

        var product = new Product
        {
            Name = dto.Name,
            SKU = dto.SKU,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repository.CreateAsync(product, ct);
    }
    
    // ... other methods
}
```

**Time:** 10 minutes (focus on business logic, not boilerplate)

### Step 5: Generate Tests

Use [prompts/testing/unit-test-template.md](../../prompts/testing/unit-test-template.md)

AI generates 15+ test cases covering:
- Happy paths
- Validation failures
- Edge cases (null, empty, boundary values)

**Time:** 5 minutes to review and adjust

### Step 6: Commit

```bash
git add Controllers/V2/ProductsController.cs
git add DTOs/ProductDTOs.cs
git add Services/ProductService.cs
git add Tests/ProductsControllerTests.cs
git commit -m "feat: add products API (AI-assisted)

Generated controller and DTOs using AI prompt template.
Manually implemented business logic in ProductService.
Added comprehensive test coverage (92%).

Closes #456"
```

---

## Results

| Metric | Manual | With AI | Improvement |
|--------|--------|---------|-------------|
| **Time** | 45 min | 15 min | **67% faster** |
| **Errors** | 3-4 typical | 0-1 | **Fewer bugs** |
| **Test Coverage** | 60% | 92% | **+32%** |
| **Documentation** | Incomplete | Complete | **100% docs** |
| **Code Review Feedback** | 5-7 comments | 1-2 comments | **Less rework** |

---

## Key Takeaways

### What AI Did Well

✅ Generated boilerplate (controller structure, routing)  
✅ Applied conventions consistently (async, status codes, XML docs)  
✅ Created comprehensive DTOs with validation  
✅ Suggested test scenarios (saving manual thinking time)

### What Human Did

🧠 Defined requirements (prompt inputs)  
🧠 Implemented business logic (SKU uniqueness check)  
🧠 Reviewed and validated AI output  
🧠 Made architectural decisions (service layer design)  
🧠 Committed and owned the code

### Lessons Learned

1. **Good prompt = good output**: Time spent on clear requirements pays off
2. **AI for structure, human for logic**: AI excels at patterns, humans at domain knowledge
3. **Review is critical**: Always validate AI code before committing
4. **Tag commits**: "AI-assisted" attribution helps track impact

---

## Variations

### Variation 1: Minimal API (GET only)

Modify prompt to generate only GET endpoints for read-only access.

### Variation 2: Admin-Only API

Add authorization to prompt:
```markdown
Add [Authorize(Roles = "Admin")] attribute to all endpoints
```

### Variation 3: GraphQL Instead of REST

Use different prompt template for GraphQL resolvers.

---

## Next Steps

- Deploy to dev environment
- Test endpoints with Postman/Swagger
- Request code review
- Merge to main after approval

---

## Related Examples

- [Integration Test Example](../integration-test/README.md)
- [CI Failure Diagnosis Example](../ci-failure-diagnosis/README.md)

---

## Feedback

Was this example helpful? What could be improved? File feedback in this repository.
