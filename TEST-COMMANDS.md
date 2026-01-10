# API Testing Commands

## Prerequisites
```powershell
# Start the API
cd IncidentManagement.Api
dotnet run
```

Navigate to: https://localhost:5001/swagger

---

## Incident Management API (AI-Powered)

### 1. Create Incident (Valid)
```bash
curl -X POST https://localhost:5001/incidents \
  -H "Content-Type: application/json" \
  -d '{
    "userDescription": "Users are experiencing timeout errors when trying to login to the production application. This started about 30 minutes ago and is affecting multiple users."
  }'
```

**Expected Response (201 Created):**
```json
{
  "id": "guid",
  "userDescription": "Users are experiencing timeout...",
  "structuredSummary": "[MOCK AI SUMMARY] Severity: High. Issue: Users are experiencing...",
  "severity": "High",
  "tags": ["authentication", "production", "performance"],
  "createdAt": "2026-01-10T...",
  "correlationId": "unique-guid"
}
```

### 2. Create Incident with Manual Severity Override
```bash
curl -X POST https://localhost:5001/incidents \
  -H "Content-Type: application/json" \
  -d '{
    "userDescription": "Minor UI glitch in the settings page",
    "manualSeverity": 4
  }'
```

### 3. Create Incident (Validation Failure - Too Short)
```bash
curl -X POST https://localhost:5001/incidents \
  -H "Content-Type: application/json" \
  -d '{
    "userDescription": "short"
  }'
```

**Expected Response (400 Bad Request):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "UserDescription": ["Description must be between 10 and 5000 characters."]
  },
  "correlationId": "unique-guid"
}
```

### 4. Get Incident by ID
```bash
curl https://localhost:5001/incidents/PASTE-GUID-HERE
```

### 5. List All Incidents
```bash
curl https://localhost:5001/incidents
```

### 6. Filter Incidents by Severity
```bash
curl https://localhost:5001/incidents?severity=High
curl https://localhost:5001/incidents?severity=Critical
```

### 7. Test Custom Correlation ID
```bash
curl -X POST https://localhost:5001/incidents \
  -H "Content-Type: application/json" \
  -H "X-Correlation-Id: my-test-correlation-123" \
  -d '{
    "userDescription": "Testing custom correlation ID in distributed tracing"
  }'
```

**Check Response Headers:**
```
X-Correlation-Id: my-test-correlation-123
```

---

## Orders API - V1 (URL-Based Versioning)

### 1. Create Order V1
```bash
curl -X POST https://localhost:5001/v1/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "John Doe",
    "productName": "Premium Widget",
    "quantity": 5
  }'
```

**Expected Response (201 Created):**
```json
{
  "orderId": "guid",
  "status": "Pending",
  "createdAt": "2026-01-10T...",
  "apiVersion": "1.0"
}
```

### 2. Get Order V1
```bash
curl https://localhost:5001/v1/orders/PASTE-GUID-HERE
```

### 3. Create Order V1 (Validation Failure)
```bash
curl -X POST https://localhost:5001/v1/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "A",
    "productName": "Widget",
    "quantity": 9999
  }'
```

---

## Orders API - V2 (Header-Based Versioning)

### 1. Create Order V2
```bash
curl -X POST https://localhost:5001/orders \
  -H "Content-Type: application/json" \
  -H "X-Version: 2.0" \
  -d '{
    "customerId": "123e4567-e89b-12d3-a456-426614174000",
    "productSKU": "ABC-1234",
    "quantity": 10,
    "shippingAddress": "123 Main Street, Anytown, USA"
  }'
```

**Expected Response (201 Created):**
```json
{
  "orderId": "guid",
  "status": "Pending Shipment",
  "createdAt": "2026-01-10T...",
  "apiVersion": "2.0"
}
```

### 2. Get Order V2
```bash
curl https://localhost:5001/orders/PASTE-GUID-HERE \
  -H "X-Version: 2.0"
```

### 3. Create Order V2 (Invalid SKU Format)
```bash
curl -X POST https://localhost:5001/orders \
  -H "Content-Type: application/json" \
  -H "X-Version: 2.0" \
  -d '{
    "customerId": "123e4567-e89b-12d3-a456-426614174000",
    "productSKU": "INVALID-SKU",
    "quantity": 10,
    "shippingAddress": "123 Main Street"
  }'
```

**Expected Error:**
```json
{
  "errors": {
    "ProductSKU": ["SKU must match format: ABC-1234"]
  }
}
```

### 4. Test Default Version (Should use V1)
```bash
curl -X POST https://localhost:5001/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Jane Smith",
    "productName": "Standard Widget",
    "quantity": 3
  }'
```

**Note:** Without version header, should default to V1 contract.

---

## PowerShell Alternative (Windows)

### Incident Creation
```powershell
$body = @{
    userDescription = "Critical database outage affecting all production services"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/incidents" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body `
    -SkipCertificateCheck
```

### Order Creation V2
```powershell
$headers = @{
    "X-Version" = "2.0"
    "Content-Type" = "application/json"
}

$body = @{
    customerId = "123e4567-e89b-12d3-a456-426614174000"
    productSKU = "XYZ-5678"
    quantity = 15
    shippingAddress = "456 Oak Avenue, Springfield"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/orders" `
    -Method Post `
    -Headers $headers `
    -Body $body `
    -SkipCertificateCheck
```

---

## Testing Exception Handling

### 1. Trigger Not Found (404)
```bash
curl https://localhost:5001/incidents/00000000-0000-0000-0000-000000000000
```

**Expected Response:**
```json
{
  "type": "about:blank",
  "title": "Incident Not Found",
  "status": 404,
  "detail": "No incident exists with ID: 00000000-0000-0000-0000-000000000000",
  "instance": "/incidents/00000000-0000-0000-0000-000000000000",
  "correlationId": "unique-guid"
}
```

---

## Observability Testing

### 1. Check Correlation ID in Logs
After creating an incident, check the logs:

```powershell
# View latest log file
Get-Content .\logs\incident-api-*.txt -Tail 50
```

You should see entries like:
```
[12:34:56.789 INF] abc-123-def Request started: POST /incidents
[12:34:56.890 INF] abc-123-def AI enrichment completed. Severity: High, Duration: 234ms
[12:34:56.950 INF] abc-123-def Incident created successfully. IncidentId: xyz-789, TotalDuration: 345ms
```

### 2. Filter Logs by Correlation ID
```powershell
Select-String -Path ".\logs\incident-api-*.txt" -Pattern "your-correlation-id-here"
```

---

## API Version Discovery

### Check Supported Versions
```bash
curl -I https://localhost:5001/v1/orders/00000000-0000-0000-0000-000000000000
```

**Response Headers:**
```
api-supported-versions: 1.0, 2.0
```

---

## Load Testing (Optional)

### Create 100 Incidents
```bash
for i in {1..100}; do
  curl -X POST https://localhost:5001/incidents \
    -H "Content-Type: application/json" \
    -d "{\"userDescription\":\"Test incident number $i for load testing AI enrichment latency\"}" &
done
```

Then analyze logs for:
- Average enrichment duration
- P95/P99 latency
- Any errors or timeouts

---

## Notes

1. **HTTPS Certificate Warning**: Use `-k` (curl) or `-SkipCertificateCheck` (PowerShell) for local development
2. **GUID Placeholders**: Replace `PASTE-GUID-HERE` with actual GUIDs from previous responses
3. **Correlation ID**: All responses include `X-Correlation-Id` header - use this for tracing
4. **Swagger UI**: For interactive testing, use https://localhost:5001/swagger
