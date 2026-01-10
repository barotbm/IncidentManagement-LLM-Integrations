# 🚀 QUICK START GUIDE
## .NET 9 Web API Reference Architecture

---

## ⚡ 60-Second Setup

```powershell
# 1. Navigate to project
cd c:\TFS\IncidentManagement-LLM-Integrations\IncidentManagement.Api

# 2. Run the application
dotnet run

# 3. Open Swagger UI
start https://localhost:5001/swagger
```

✅ **Build Status:** Compiles successfully  
✅ **No Errors:** All files verified  

---

## 🎯 Test the Key Features (Copy & Paste)

### 1️⃣ Test AI-Powered Incident Triage
```powershell
curl -X POST https://localhost:5001/incidents `
  -H "Content-Type: application/json" `
  -k `
  -d '{
    "userDescription": "Production database is experiencing severe performance issues - queries timing out after 30 seconds"
  }'
```

**✅ Expected:** 201 Created with AI-enriched response  
**Check:** Tags should include "database", "production", "performance"  

---

### 2️⃣ Test Fail-Fast Validation
```powershell
curl -X POST https://localhost:5001/incidents `
  -H "Content-Type: application/json" `
  -k `
  -d '{"userDescription": "too short"}'
```

**✅ Expected:** 400 Bad Request  
**Check:** Error message says "Description must be between 10 and 5000 characters"  

---

### 3️⃣ Test API Versioning (V1 - URL-based)
```powershell
curl -X POST https://localhost:5001/v1/orders `
  -H "Content-Type: application/json" `
  -k `
  -d '{
    "customerName": "John Doe",
    "productName": "Widget Pro",
    "quantity": 5
  }'
```

**✅ Expected:** 201 Created with `"apiVersion": "1.0"`  

---

### 4️⃣ Test API Versioning (V2 - Header-based)
```powershell
curl -X POST https://localhost:5001/orders `
  -H "Content-Type: application/json" `
  -H "X-Version: 2.0" `
  -k `
  -d '{
    "customerId": "123e4567-e89b-12d3-a456-426614174000",
    "productSKU": "ABC-1234",
    "quantity": 10,
    "shippingAddress": "123 Main Street"
  }'
```

**✅ Expected:** 201 Created with `"apiVersion": "2.0"`  

---

### 5️⃣ Test Correlation ID Tracing
```powershell
curl -X POST https://localhost:5001/incidents `
  -H "Content-Type: application/json" `
  -H "X-Correlation-Id: my-test-trace-123" `
  -k `
  -d '{"userDescription": "Testing distributed tracing with custom correlation ID"}'
```

**✅ Expected:** Response header `X-Correlation-Id: my-test-trace-123`  
**Check Logs:**
```powershell
Select-String -Path ".\logs\incident-api-*.txt" -Pattern "my-test-trace-123"
```

---

## 📂 Project Structure at a Glance

```
IncidentManagement.Api/
├── 📄 Program.cs                    ← MIDDLEWARE PIPELINE (read this first!)
├── 🔧 Controllers/
│   ├── IncidentsController.cs      ← AI-powered incident triage
│   ├── V1/OrdersV1Controller.cs    ← URL versioning (/v1/orders)
│   └── V2/OrdersV2Controller.cs    ← Header versioning (X-Version: 2.0)
├── 🛡️ Middleware/
│   └── CorrelationIdMiddleware.cs  ← Distributed tracing
├── 🎯 Filters/
│   ├── GlobalExceptionHandler.cs   ← Problem Details responses
│   └── ValidationActionFilter.cs   ← Fail-fast validation
├── 🔌 Interfaces/
│   └── IIncidentEnricher.cs        ← AI ABSTRACTION (swap for real LLM)
└── ⚙️ Services/
    └── MockIncidentEnricher.cs     ← Mock AI service
```

---

## 🧠 The 5 Key Patterns Explained

### 1. Middleware Order (WHY it matters)
```csharp
app.UseExceptionHandler();    // ← MUST BE FIRST (catch downstream errors)
app.UseAuthentication();      // ← WHO is the user?
app.UseAuthorization();       // ← WHAT can they do? (needs WHO first)
app.UseCorrelationId();       // ← Trace ID (AFTER auth, BEFORE logging)
```

**💡 Key Insight:** Order = behavior. See [Program.cs line 96-160](IncidentManagement.Api/Program.cs#L96-L160) for full explanation.

---

### 2. AI Abstraction (Future-proof)
```csharp
// Interface for AI enrichment
public interface IIncidentEnricher
{
    Task<EnrichmentResult> EnrichAsync(string description, string correlationId);
}

// Current: Mock implementation
builder.Services.AddScoped<IIncidentEnricher, MockIncidentEnricher>();

// Future: Swap for real LLM (ONE LINE CHANGE)
builder.Services.AddScoped<IIncidentEnricher, SemanticKernelEnricher>();
```

**💡 Key Insight:** Controllers never change when you swap AI providers.

---

### 3. Global Filters (DRY)
```csharp
// ❌ OLD: Repetitive code in every controller
if (!ModelState.IsValid)
    return BadRequest(ModelState);

// ✅ NEW: Global filter handles automatically
// (ValidationActionFilter registered in Program.cs)
```

**💡 Key Insight:** Cross-cutting concerns handled once, applied everywhere.

---

### 4. Correlation IDs (Observability)
```
REQUEST → CorrelationIdMiddleware → Controller → AI Service → RESPONSE
   ↓              ↓                      ↓            ↓            ↓
 Header      Generate ID            Pass to       Log with     Return in
              & Store              enricher     CorrelationId   Header
```

**💡 Key Insight:** Trace a request end-to-end across services/AI calls.

---

### 5. API Versioning (Backward Compatibility)
```csharp
// V1: Legacy clients (NEVER REMOVE)
[Route("v{version:apiVersion}/orders")]
[ApiVersion("1.0")]

// V2: New clients with breaking changes
[Route("orders")]
[ApiVersion("2.0")]
```

**💡 Key Insight:** Maintain V1 indefinitely for clients that can't update.

---

## 📊 What Makes This "Staff-Level"?

| Aspect | Junior Approach | Staff Approach (This Project) |
|--------|----------------|------------------------------|
| **Middleware** | Random order, no comments | Explicit order + WHY explanations |
| **Error Handling** | Try-catch in every controller | Global IExceptionHandler |
| **Validation** | `if (!ModelState.IsValid)` everywhere | Global ValidationActionFilter |
| **AI Integration** | Directly call OpenAI API | IIncidentEnricher abstraction |
| **Versioning** | Change contracts, break clients | Multiple versions, backward compat |
| **Logging** | `Console.WriteLine()` | Serilog with correlation IDs |
| **Documentation** | README only | README + ARCHITECTURE + TEST-COMMANDS |

---

## 🔍 Explore the Code

### Start Here (Recommended Order)
1. **[Program.cs](IncidentManagement.Api/Program.cs)** - See the middleware pipeline with extensive comments
2. **[IIncidentEnricher.cs](IncidentManagement.Api/Interfaces/IIncidentEnricher.cs)** - Understand the AI abstraction
3. **[IncidentsController.cs](IncidentManagement.Api/Controllers/IncidentsController.cs)** - See the enrichment workflow
4. **[CorrelationIdMiddleware.cs](IncidentManagement.Api/Middleware/CorrelationIdMiddleware.cs)** - Distributed tracing
5. **[GlobalExceptionHandler.cs](IncidentManagement.Api/Filters/GlobalExceptionHandler.cs)** - Error handling

### Deep Dive
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Request pipeline flow, design patterns, decisions
- **[TEST-COMMANDS.md](TEST-COMMANDS.md)** - Comprehensive testing guide

---

## 🎓 Learning Path

### Beginner
1. Run the API
2. Test endpoints via Swagger
3. Read inline comments in Program.cs

### Intermediate
1. Modify MockIncidentEnricher logic
2. Add a new DTO with validation
3. Create a V3 controller with new features

### Advanced
1. Replace MockIncidentEnricher with Semantic Kernel
2. Add EF Core persistence
3. Implement retry policies (Polly) for AI calls

---

## 📖 Documentation Index

| File | Purpose | Time to Read |
|------|---------|--------------|
| **[README.md](README.md)** | User guide, quick start | 5 min |
| **[PROJECT-SUMMARY.md](PROJECT-SUMMARY.md)** | Complete deliverables list | 10 min |
| **[ARCHITECTURE.md](ARCHITECTURE.md)** | Deep dive, patterns, flow diagrams | 20 min |
| **[TEST-COMMANDS.md](TEST-COMMANDS.md)** | cURL/PowerShell examples | 5 min |
| **THIS FILE** | Quick reference, cheat sheet | 3 min |

---

## ✅ Pre-Flight Checklist

Before running:
- [x] .NET 9 SDK installed (`dotnet --version`)
- [x] Project restored (`dotnet restore`)
- [x] Project builds (`dotnet build`)
- [x] No compilation errors

To verify:
```powershell
cd c:\TFS\IncidentManagement-LLM-Integrations\IncidentManagement.Api
dotnet build
# Should output: "Build succeeded in X.Xs"
```

---

## 🚨 Common Issues & Solutions

### Issue: "Unable to configure HTTPS endpoint"
**Solution:** Trust the dev certificate
```powershell
dotnet dev-certs https --trust
```

### Issue: "Port 5001 already in use"
**Solution:** Change port in [launchSettings.json](IncidentManagement.Api/Properties/launchSettings.json)

### Issue: "Swagger UI not loading"
**Solution:** Check you're in Development mode
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
```

---

## 🎯 Next Steps After Running

1. **Test via Swagger** - https://localhost:5001/swagger
2. **Check Logs** - See `IncidentManagement.Api/logs/` folder
3. **Modify & Experiment** - Change MockIncidentEnricher logic
4. **Read ARCHITECTURE.md** - Understand the WHY behind decisions

---

## 💡 Pro Tips

1. **Search for "CRITICAL"** in code comments - highlights key architectural decisions
2. **Search for "FUTURE"** - shows AI integration migration path
3. **Check logs folder** - correlation IDs make debugging easy
4. **Use Swagger's "Try it out"** - interactive testing without cURL

---

## 🎉 Success Indicators

You'll know it's working when:
- ✅ Swagger UI loads at https://localhost:5001/swagger
- ✅ POST /incidents returns 201 with AI-enriched data
- ✅ Validation errors return 400 with detailed messages
- ✅ Logs folder populates with structured logs
- ✅ Response headers include X-Correlation-Id

---

**Ready to run?**
```powershell
cd c:\TFS\IncidentManagement-LLM-Integrations\IncidentManagement.Api
dotnet run
```

**Then open:** https://localhost:5001/swagger 🚀
