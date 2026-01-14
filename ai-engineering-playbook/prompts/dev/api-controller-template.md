# API Controller Template

**Category:** Development  
**Use Case:** Generate ASP.NET Core Web API controllers  
**Maturity Level:** 2-3  
**Last Updated:** January 2026

---

## Purpose

Generate a production-ready REST API controller following company conventions.

---

## Prompt Template

```markdown
Generate a C# ASP.NET Core Web API controller for managing {ENTITY_NAME}.

## Requirements

### Namespace and Location
- Namespace: IncidentManagement.Api.Controllers.{VERSION}
- File: Controllers/{VERSION}/{Entity}Controller.cs
- Version: {V1|V2} (specify which API version)

### Endpoints

Implement the following HTTP endpoints:

- **POST** /api/{version}/{entity-plural}
  - Create new {entity}
  - Accept {Entity}CreateDto
  - Return 201 Created with Location header
  - Return 400 Bad Request if validation fails

- **GET** /api/{version}/{entity-plural}/{id}
  - Retrieve {entity} by ID
  - Return 200 OK with {Entity}Dto
  - Return 404 Not Found if not exists

- **GET** /api/{version}/{entity-plural}
  - List all {entity-plural} with pagination
  - Query params: page (default 1), pageSize (default 20)
  - Return 200 OK with PagedResult<{Entity}Dto>

- **PUT** /api/{version}/{entity-plural}/{id}
  - Update existing {entity}
  - Accept {Entity}UpdateDto
  - Return 204 No Content on success
  - Return 404 Not Found if not exists
  - Return 400 Bad Request if validation fails

- **DELETE** /api/{version}/{entity-plural}/{id}
  - Soft delete {entity}
  - Return 204 No Content on success
  - Return 404 Not Found if not exists

### Dependencies

- Use constructor dependency injection
- Inject I{Entity}Service interface
- Inject ILogger<{Entity}Controller>

### Validation

- Use [ApiController] attribute for automatic ModelState validation
- Add data annotations to DTOs
- Return ValidationProblemDetails for 400 responses

### Documentation

- Add XML documentation comments to controller and all actions
- Document HTTP methods, routes, parameters, and response codes
- Use <summary>, <param>, <returns>, and <response> tags

### Error Handling

- Return ProblemDetails for errors (handled by global exception middleware)
- Use appropriate HTTP status codes
- Do NOT catch exceptions (let middleware handle)

### Async/Await

- All actions must be async (return Task<IActionResult>)
- Use await for service calls
- Use CancellationToken parameter

## Constraints

**Do NOT:**
- Add authentication/authorization attributes (handled by middleware)
- Access database directly (use service layer)
- Add business logic (keep in service layer)
- Use synchronous code
- Hardcode configuration values

**DO:**
- Follow existing controller patterns in Controllers/V1/
- Use DTOs, never expose domain models directly
- Follow RESTful conventions
- Return appropriate status codes
- Use route templates consistently

## Context

**Framework:** .NET 9  
**Existing Example:** Controllers/V1/OrdersV1Controller.cs  
**DTO Pattern:** See DTOs/OrderDTOs.cs  
**Service Pattern:** Injected via DI, returns domain models  
**Mapping:** Controller maps domain models to DTOs

## Example DTO Structure

```csharp
public record {Entity}CreateDto(
    string Property1,
    int Property2
    // ... properties
);

public record {Entity}UpdateDto(
    string Property1,
    int Property2
    // ... properties
);

public record {Entity}Dto(
    int Id,
    string Property1,
    int Property2,
    DateTime CreatedAt
    // ... properties
);
```

## Output Format

Provide:
1. Full controller class code
2. Required using statements
3. Suggested DTO definitions (if not provided)

Do NOT include:
- Service implementation (out of scope)
- Database models (separate concern)
- Test code (separate prompt)
```

---

## Usage Example

**Fill in the placeholders:**

- `{ENTITY_NAME}`: Customer
- `{VERSION}`: V2
- `{entity-plural}`: customers

**Resulting prompt:**

> Generate a C# ASP.NET Core Web API controller for managing Customer.
> 
> Requirements:
> - Namespace: IncidentManagement.Api.Controllers.V2
> - File: Controllers/V2/CustomerController.cs
> - Version: V2
> 
> Endpoints:
> - POST /api/v2/customers (create)
> - GET /api/v2/customers/{id} (get by ID)
> - GET /api/v2/customers (list with pagination)
> - PUT /api/v2/customers/{id} (update)
> - DELETE /api/v2/customers/{id} (soft delete)
> 
> [... rest of template ...]

---

## Expected Output Quality

AI should generate:
- ✅ Compilable code with correct namespaces
- ✅ All endpoints with proper routing
- ✅ Constructor DI for service and logger
- ✅ Async/await throughout
- ✅ XML documentation comments
- ✅ Proper HTTP status codes
- ✅ CancellationToken usage

Common issues to watch for:
- ❌ Missing using statements
- ❌ Synchronous code
- ❌ Direct database access
- ❌ Exception handling (should be omitted)
- ❌ Hardcoded values

---

## After Generation

1. **Review** the generated code
2. **Compile** to check for errors
3. **Add business logic** to service layer (not in controller)
4. **Create tests** (use testing prompt template)
5. **Commit** with message: `feat: add {entity} API controller (AI-assisted)`

---

## Variations

### Minimal Controller (GET only)

Remove POST/PUT/DELETE sections, keep only GET endpoints.

### Admin Controller

Add authorization attribute:
```markdown
Add [Authorize(Roles = "Admin")] to controller class
```

### With Search

Add search endpoint:
```markdown
- GET /api/{version}/{entity-plural}/search?query={query}
  - Full-text search across {entity} fields
  - Return 200 OK with IEnumerable<{Entity}Dto>
```

---

## Related Prompts

- [dto-template.md](dto-template.md) - Generate DTOs
- [../testing/unit-test-template.md](../testing/unit-test-template.md) - Generate controller tests
- [service-template.md](service-template.md) - Generate service layer

---

## Ownership

- **Created by:** Engineering Team
- **Maintained by:** Tech Leads
- **Last reviewed:** January 2026
- **Usage count:** TBD (track in metrics)

---

## Feedback

If this prompt produces poor results, file an issue or submit a PR with improvements.
