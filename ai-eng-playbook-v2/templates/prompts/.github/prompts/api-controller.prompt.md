# Prompt: Generate an API Controller

## Objective

Generate a new API controller that matches repo conventions and produces clean, testable code.

## Input

Provide:
- Controller name
- Route prefix
- DTO names
- Service interface name
- Required endpoints

## Output

- Controller file content
- DTO usage (no new DTOs unless requested)
- XML documentation comments
- Example request/response shapes (as comments)

## Constraints

- Do NOT add authentication or authorization logic
- Do NOT connect directly to the database
- Follow the repository’s error handling and logging patterns

## Example Invocation

“Create OrdersV2Controller with POST /api/v2/orders, GET /api/v2/orders/{id}, GET /api/v2/orders (paginated). Use IOrderService. Reference Controllers/V1/OrdersV1Controller.cs for patterns.”
