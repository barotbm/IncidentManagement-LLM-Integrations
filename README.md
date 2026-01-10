# .NET 9 Web API Reference Architecture
## Request Pipeline Mastery & AI Extensibility

This solution demonstrates **staff-level .NET 9 Web API architecture** with focus on:
- ✅ **Middleware Pipeline Ordering** (with detailed explanations)
- ✅ **Global Filters (AOP)** for cross-cutting concerns
- ✅ **Fail-Fast Validation Strategy**
- ✅ **API Versioning** (URL-based & Header-based)
- ✅ **AI/LLM Integration Readiness** via abstractions
- ✅ **Production-Ready Observability** (structured logging, correlation IDs)

---

## 📁 Project Structure

```
IncidentManagement.Api/
├── Controllers/
│   ├── V1/
│   │   └── OrdersV1Controller.cs      # URL-based versioning (/v1/orders)
│   ├── V2/
│   │   └── OrdersV2Controller.cs      # Header-based versioning (X-Version: 2.0)
│   └── IncidentsController.cs         # AI-powered incident triage
├── Middleware/
│   └── CorrelationIdMiddleware.cs     # Unique ID per request for tracing
├── Filters/
│   ├── GlobalExceptionHandler.cs      # IExceptionHandler for Problem Details
│   └── ValidationActionFilter.cs      # Fail-fast model validation
├── Models/
│   ├── IncidentTicket.cs              # Domain model (AI-enriched)
│   └── IncidentSeverity.cs            # Severity enum
├── DTOs/
│   ├── IncidentDTOs.cs                # Request/Response DTOs
│   └── OrderDTOs.cs                   # Versioned DTOs (V1 vs V2)
├── Interfaces/
│   └── IIncidentEnricher.cs           # AI integration abstraction
├── Services/
│   └── MockIncidentEnricher.cs        # Mock AI service (swap for LLM later)
├── Program.cs                          # Pipeline configuration (HEAVILY COMMENTED)
└── appsettings.json                    # Serilog configuration
```

---

## 🚀 Quick Start

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 / VS Code / Rider

### Run the Application
```powershell
cd IncidentManagement.Api
dotnet restore
dotnet run
```

Navigate to: **https://localhost:5001/swagger**

---

## 🔑 Key Architectural Patterns

### 1. **Middleware Pipeline Ordering**
The order in [Program.cs](IncidentManagement.Api/Program.cs#L96-L160) is **critical**:

```csharp
app.UseExceptionHandler();      // 1. MUST BE FIRST (catch all exceptions)
app.UseHttpsRedirection();      // 2. Force HTTPS
app.UseAuthentication();        // 3. WHO is the user?
app.UseAuthorization();         // 4. WHAT can they do?
app.UseCorrelationId();         // 5. Attach tracing ID
app.MapControllers();           // 6. Route to endpoints
```

**Why CorrelationId is placed AFTER Auth?**
- Only trace legitimate, authorized requests
- Reduces log pollution from bots/scanners
- For security audits, auth failures have separate logging

### 2. **Global Exception Handler (AOP)**
[GlobalExceptionHandler.cs](IncidentManagement.Api/Filters/GlobalExceptionHandler.cs) implements `IExceptionHandler`:
- Catches **all** unhandled exceptions
- Translates to RFC 7807 Problem Details
- Includes **correlation ID** in response
- Maps exception types to HTTP status codes
- **Never leaks stack traces in production**

### 3. **Fail-Fast Validation**
[ValidationActionFilter.cs](IncidentManagement.Api/Filters/ValidationActionFilter.cs) eliminates:
```csharp
// ❌ OLD WAY (repeated in every action):
if (!ModelState.IsValid)
    return BadRequest(ModelState);

// ✅ NEW WAY: Global filter handles this automatically
// Controllers just handle business logic
```

### 4. **API Versioning Strategy**

#### V1: URL-Based (/v1/orders)
- Used by legacy clients that **cannot update**
- Easy to discover and test
- See [OrdersV1Controller.cs](IncidentManagement.Api/Controllers/V1/OrdersV1Controller.cs)

#### V2: Header-Based (X-Version: 2.0)
- Cleaner URLs for documentation
- Enterprise-friendly (API gateways route on headers)
- See [OrdersV2Controller.cs](IncidentManagement.Api/Controllers/V2/OrdersV2Controller.cs)

**Test V2 with curl:**
```bash
curl -X POST https://localhost:5001/orders \
  -H "X-Version: 2.0" \
  -H "Content-Type: application/json" \
  -d '{"customerId":"123e4567-e89b-12d3-a456-426614174000","productSKU":"ABC-1234","quantity":5,"shippingAddress":"123 Main St"}'
```

### 5. **AI/LLM Integration Abstraction**

The **critical pattern** for AI extensibility is the [IIncidentEnricher](IncidentManagement.Api/Interfaces/IIncidentEnricher.cs) interface:

```csharp
// Current: Mock implementation
builder.Services.AddScoped<IIncidentEnricher, MockIncidentEnricher>();

// Future: Swap with real LLM service WITHOUT changing controllers
builder.Services.AddScoped<IIncidentEnricher, SemanticKernelEnricher>();
```

**Why this matters:**
- ✅ Controllers don't depend on concrete LLM implementations
- ✅ Easy to A/B test different AI models
- ✅ Mockable for unit tests
- ✅ Correlation IDs flow through to track LLM latency

---

## 📊 API Endpoints

### Incidents API (AI-Powered)
```http
POST /incidents
GET  /incidents
GET  /incidents/{id}
GET  /incidents?severity=High
```

**Sample Request:**
```json
POST /incidents
{
  "userDescription": "Users cannot login to production app - getting timeout errors"
}
```

**Sample Response:**
```json
{
  "id": "guid",
  "userDescription": "Users cannot login...",
  "structuredSummary": "[MOCK AI SUMMARY] Severity: High. Issue: Users cannot...",
  "severity": "High",
  "tags": ["authentication", "production", "performance"],
  "createdAt": "2026-01-10T12:34:56Z",
  "correlationId": "abc-123-def"
}
```

### Orders API V1 (Backward Compatibility)
```http
POST /v1/orders
GET  /v1/orders/{id}
```

### Orders API V2 (Header Versioning)
```http
POST /orders (with header: X-Version: 2.0)
GET  /orders/{id} (with header: X-Version: 2.0)
```

---

## 🔍 Observability & Tracing

### Correlation IDs
Every request gets a unique correlation ID:
- **Injected** by [CorrelationIdMiddleware](IncidentManagement.Api/Middleware/CorrelationIdMiddleware.cs)
- **Logged** with every log statement (via Serilog scope)
- **Returned** in response header `X-Correlation-Id`
- **Flows** through AI enrichment calls

**Find all logs for a request:**
```powershell
# Check logs/incident-api-*.txt
grep "abc-123-def" logs/incident-api-2026-01-10.txt
```

### Structured Logging (Serilog)
Configured in [appsettings.json](IncidentManagement.Api/appsettings.json):
- Console output for development
- File output for production (`logs/` folder)
- All logs include correlation ID

**Query AI latency:**
```csharp
// Example log output:
[12:34:56 INF] abc-123 AI enrichment completed. Severity: High, EnrichmentDuration: 234ms
```

---

## 🧪 Testing Guide

### 1. Test Validation (Fail-Fast)
```bash
# Invalid request (description too short)
curl -X POST https://localhost:5001/incidents \
  -H "Content-Type: application/json" \
  -d '{"userDescription":"short"}'

# Expected: 400 Bad Request with ValidationProblemDetails
```

### 2. Test Exception Handling
```bash
# Trigger an exception (e.g., invalid GUID)
curl https://localhost:5001/incidents/invalid-guid

# Expected: 400 Bad Request with ProblemDetails + correlationId
```

### 3. Test API Versioning
```bash
# V1 (URL-based)
curl -X POST https://localhost:5001/v1/orders \
  -H "Content-Type: application/json" \
  -d '{"customerName":"John Doe","productName":"Widget","quantity":5}'

# V2 (Header-based)
curl -X POST https://localhost:5001/orders \
  -H "X-Version: 2.0" \
  -H "Content-Type: application/json" \
  -d '{"customerId":"123e4567-e89b-12d3-a456-426614174000","productSKU":"ABC-1234","quantity":5,"shippingAddress":"123 Main St"}'
```

### 4. Test Correlation ID
```bash
# Send custom correlation ID
curl https://localhost:5001/incidents \
  -H "X-Correlation-Id: my-custom-id-123"

# Check response headers - should echo back same ID
```

---

## 🔮 Future AI Integration Plan

### Step 1: Replace Mock with Semantic Kernel
```csharp
// Services/SemanticKernelIncidentEnricher.cs
public class SemanticKernelIncidentEnricher : IIncidentEnricher
{
    private readonly Kernel _kernel;
    
    public async Task<EnrichmentResult> EnrichAsync(string description, string correlationId)
    {
        // Use SK to call Azure OpenAI
        var prompt = $"Analyze this incident and return JSON with severity, tags, summary: {description}";
        var result = await _kernel.InvokePromptAsync(prompt);
        
        // Parse and return
        return JsonSerializer.Deserialize<EnrichmentResult>(result);
    }
}

// Program.cs - ONE LINE CHANGE:
builder.Services.AddScoped<IIncidentEnricher, SemanticKernelIncidentEnricher>();
```

### Step 2: Add Telemetry
- Track LLM latency with correlation ID
- Monitor confidence scores
- Alert on low-confidence results

### Step 3: Human-in-the-Loop
- Flag incidents with confidence < 0.7 for review
- A/B test different prompts/models

---

## 📚 Staff-Level Patterns Demonstrated

1. **Separation of Concerns**: Middleware → Filters → Controllers → Services
2. **Dependency Inversion**: Controllers depend on `IIncidentEnricher`, not concrete implementations
3. **Open/Closed Principle**: Add new AI enrichers without changing existing code
4. **AOP via Filters**: Cross-cutting concerns (validation, exceptions) handled globally
5. **Fail Fast**: Invalid requests never reach business logic
6. **Distributed Tracing**: Correlation IDs enable end-to-end observability
7. **Backward Compatibility**: API versioning prevents breaking legacy clients
8. **Production Readiness**: Structured logging, problem details, proper error handling

---

## 🛠️ Production Enhancements (Not Implemented)

- [ ] Add EF Core / Cosmos DB for persistence
- [ ] Implement Repository Pattern
- [ ] Add rate limiting (`app.UseRateLimiter()`)
- [ ] Configure CORS
- [ ] Add authentication (JWT/OAuth)
- [ ] Implement response caching
- [ ] Add health checks
- [ ] Configure retry policies (Polly) for LLM calls
- [ ] Add circuit breakers for fault tolerance
- [ ] Implement API key management for LLM services

---

## 📖 Key Files to Review

1. **[Program.cs](IncidentManagement.Api/Program.cs)** - Middleware pipeline with extensive comments
2. **[CorrelationIdMiddleware.cs](IncidentManagement.Api/Middleware/CorrelationIdMiddleware.cs)** - Distributed tracing
3. **[GlobalExceptionHandler.cs](IncidentManagement.Api/Filters/GlobalExceptionHandler.cs)** - Centralized error handling
4. **[IIncidentEnricher.cs](IncidentManagement.Api/Interfaces/IIncidentEnricher.cs)** - AI abstraction (CRITICAL)
5. **[IncidentsController.cs](IncidentManagement.Api/Controllers/IncidentsController.cs)** - AI workflow with observability

---

## 📞 Architecture Questions?

This solution is designed to answer:
- ❓ "Why does middleware order matter?"
- ❓ "How do I prepare my API for LLM integration?"
- ❓ "What's the best way to handle versioning?"
- ❓ "How do I trace requests across distributed systems?"
- ❓ "How do I avoid controller pollution with cross-cutting concerns?"

All answers are in the code comments! 🎯
