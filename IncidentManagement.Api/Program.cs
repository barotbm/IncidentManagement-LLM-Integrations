using Asp.Versioning;
using IncidentManagement.Api.Filters;
using IncidentManagement.Api.Interfaces;
using IncidentManagement.Api.Middleware;
using IncidentManagement.Api.Services;
using Serilog;

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * .NET 9 WEB API REFERENCE ARCHITECTURE
 * Demonstrates: Request Pipeline Mastery & AI Extensibility
 * 
 * Staff-Level Architectural Decisions:
 * 1. Middleware ordering is critical for correct behavior
 * 2. Global filters implement AOP to avoid controller pollution
 * 3. Fail-fast validation reduces boilerplate and improves UX
 * 4. API versioning supports backward compatibility
 * 5. Abstractions enable LLM integration without tight coupling
 * 6. Structured logging + correlation IDs enable distributed tracing
 * ═══════════════════════════════════════════════════════════════════════════════
 */

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════════════════════
// LOGGING CONFIGURATION
// ═══════════════════════════════════════════════════════════════════════════════
// Configure Serilog for structured logging
// CRITICAL: CorrelationId enrichment is configured in appsettings.json template
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// ═══════════════════════════════════════════════════════════════════════════════
// DEPENDENCY INJECTION CONFIGURATION
// ═══════════════════════════════════════════════════════════════════════════════

// Register global exception handler (IExceptionHandler pattern - .NET 8+)
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Required for Problem Details responses

// Register global validation filter for "Fail Fast" strategy
builder.Services.AddControllers(options =>
{
    // Add global filter to validate models before controller actions execute
    // This eliminates the need for "if (!ModelState.IsValid)" in every action
    options.Filters.Add<ValidationActionFilter>();
});

// ═══════════════════════════════════════════════════════════════════════════════
// API VERSIONING CONFIGURATION
// ═══════════════════════════════════════════════════════════════════════════════
builder.Services.AddApiVersioning(options =>
{
    // Default to V1 if no version is specified
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true; // Add "api-supported-versions" header to responses

    // Configure multiple versioning strategies for demonstration
    // V1: URL-based versioning (/v1/orders)
    // V2: Header-based versioning (X-Version: 2.0)
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),    // Reads version from URL: /v1/orders
        new HeaderApiVersionReader("X-Version") // Reads version from header
    );
}).AddApiExplorer(options =>
{
    // Format version as "v{major}" for URL-based versioning
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ═══════════════════════════════════════════════════════════════════════════════
// BUSINESS SERVICES REGISTRATION
// ═══════════════════════════════════════════════════════════════════════════════
// Register IIncidentEnricher abstraction
// CURRENT: Mock implementation for development
// FUTURE: Replace with actual LLM service:
//   builder.Services.AddScoped<IIncidentEnricher, SemanticKernelIncidentEnricher>();
//   builder.Services.AddScoped<IIncidentEnricher, LangChainIncidentEnricher>();
builder.Services.AddScoped<IIncidentEnricher, MockIncidentEnricher>();

// ═══════════════════════════════════════════════════════════════════════════════
// SWAGGER / OPENAPI CONFIGURATION
// ═══════════════════════════════════════════════════════════════════════════════
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Incident Management API", Version = "v1" });
    options.SwaggerDoc("v2", new() { Title = "Incident Management API", Version = "v2" });
});

// ═══════════════════════════════════════════════════════════════════════════════
// MIDDLEWARE PIPELINE CONFIGURATION
// ═══════════════════════════════════════════════════════════════════════════════
var app = builder.Build();

// Configure Swagger (development only for security)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "V2");
    });
}

/*
 * ═══════════════════════════════════════════════════════════════════════════════
 * CRITICAL: MIDDLEWARE ORDER MATTERS
 * ═══════════════════════════════════════════════════════════════════════════════
 * The order of middleware in the pipeline is CRUCIAL for correct behavior.
 * Middleware executes in the order it's added during request processing,
 * and in REVERSE order during response processing.
 * 
 * Current Order Explanation:
 * 
 * 1. ExceptionHandler (MUST BE FIRST)
 *    - Catches all exceptions from downstream middleware
 *    - Translates to Problem Details responses
 *    - If placed after other middleware, some exceptions won't be caught
 * 
 * 2. HTTPS Redirection
 *    - Redirects HTTP → HTTPS
 *    - Placed early to secure all requests
 * 
 * 3. Authentication (BEFORE Authorization)
 *    - Identifies WHO the user is
 *    - Populates HttpContext.User
 *    - MUST run before Authorization checks
 * 
 * 4. Authorization (AFTER Authentication)
 *    - Determines WHAT the authenticated user can do
 *    - Depends on HttpContext.User being populated
 *    - Returns 401 if not authenticated, 403 if not authorized
 * 
 * 5. CorrelationId (AFTER Auth, BEFORE Logging)
 *    - Placed AFTER auth so correlation ID includes authenticated user context
 *    - Placed BEFORE logging middleware so logs include correlation ID
 *    - WHY HERE: We want to trace requests AFTER security checks pass
 *    - This prevents correlation ID pollution from unauthorized requests
 *    - In high-security scenarios, you might place this BEFORE auth to trace all attempts
 * 
 * 6. MapControllers (LAST)
 *    - Routes requests to controller actions
 *    - All middleware above this will process before controllers execute
 * 
 * ALTERNATIVE ORDERING CONSIDERATIONS:
 * - If you need to trace ALL requests (including auth failures), move CorrelationId before Auth
 * - If you add logging middleware, place it AFTER CorrelationId so logs are enriched
 * - If you add rate limiting, place it EARLY (after HTTPS, before Auth) to fail fast
 * 
 * PRODUCTION ADDITIONS (Not implemented here to keep focus):
 * - app.UseRateLimiter() - After HTTPS, before Auth
 * - app.UseCors() - After HTTPS, before Auth
 * - app.UseResponseCaching() - After Auth
 * - app.UseResponseCompression() - After Auth
 * ═══════════════════════════════════════════════════════════════════════════════
 */

// Step 1: Global exception handling (MUST BE FIRST)
app.UseExceptionHandler();

// Step 2: HTTPS redirection
app.UseHttpsRedirection();

// Step 3: Authentication (identifies the user)
// NOTE: No authentication is configured in this demo, but middleware is registered
// to demonstrate correct ordering. In production, add:
//   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//       .AddJwtBearer(...);
app.UseAuthentication();

// Step 4: Authorization (checks user permissions)
// Depends on Authentication running first to populate HttpContext.User
app.UseAuthorization();

// Step 5: Correlation ID middleware
// Placed AFTER auth so we only trace authorized requests
// Placed BEFORE any logging middleware (if added) so logs include correlation ID
// 
// ARCHITECTURAL DECISION:
// This placement means unauthorized requests won't get correlation IDs.
// If you need to trace auth failures, move this BEFORE UseAuthentication().
// 
// RATIONALE for current placement:
// - Reduces log volume from bots/scanners hitting auth endpoints
// - Correlation IDs represent "legitimate" request flows
// - For security investigations, auth middleware has its own logging
app.UseCorrelationId();

// EXTENSION POINT: Add structured logging middleware here
// Example:
//   app.UseMiddleware<RequestLoggingMiddleware>();
// This would log every request with correlation ID, performance metrics, etc.

// Step 6: Map controller endpoints (LAST)
app.MapControllers();

// ═══════════════════════════════════════════════════════════════════════════════
// APPLICATION STARTUP
// ═══════════════════════════════════════════════════════════════════════════════
try
{
    Log.Information("Starting Incident Management API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
