# CI/CD Build Failure Diagnosis

**Category:** CI/CD  
**Use Case:** Diagnose and suggest fixes for failed builds  
**Maturity Level:** 3-4  
**Last Updated:** January 2026

---

## Purpose

Quickly identify root cause of CI/CD pipeline failures and suggest actionable fixes.

---

## Prompt Template

```markdown
Analyze this CI/CD build failure and provide diagnosis and fix recommendations.

## Build Information

**Pipeline:** {PIPELINE_NAME}  
**Branch:** {BRANCH_NAME}  
**Commit:** {COMMIT_SHA}  
**Trigger:** {PUSH | PR | SCHEDULED}  
**Failed Step:** {STEP_NAME}

## Build Log

```
{PASTE_BUILD_LOG_HERE}
```

## Analysis Required

### 1. Root Cause Identification

Identify the primary failure reason. Common categories:

- **Compilation errors** (syntax, missing references, type mismatches)
- **Test failures** (unit, integration, E2E)
- **Dependency issues** (missing packages, version conflicts)
- **Environment issues** (missing tools, permissions, configuration)
- **Infrastructure issues** (network timeouts, service unavailability)
- **Linting/code quality failures** (SonarQube, ESLint)

### 2. Error Classification

Classify the error:

- **Transient** (flaky test, network timeout) → Retry may fix
- **Introduced in this commit** (breaking change) → Fix in code
- **Environment drift** (dependency updated, config changed) → Fix environment
- **Pre-existing** (broken on main branch) → Escalate

### 3. Suggested Fix

Provide:

- **Immediate action:** What to do right now (rerun, revert, fix code)
- **Code changes:** Specific files and lines to modify (if applicable)
- **Configuration changes:** Environment variables, pipeline config
- **Dependency changes:** Package versions to update/rollback

### 4. Prevention

Suggest how to prevent this failure in the future:

- Pre-commit hooks
- Additional tests
- Dependency pinning
- Environment validation

## Output Format

Provide analysis in this structure:

### 🔴 Root Cause
{One sentence summary of what failed and why}

### 📋 Error Details
- **Error Type:** {category from list above}
- **Failed Component:** {service/project/test that failed}
- **Likely Introduced By:** {commit, dependency update, config change, unknown}

### ✅ Recommended Fix

**Immediate Action:**
{What to do right now}

**Code Changes** (if applicable):
```{language}
// File: {filename}
// Line: {line number}
{suggested code fix}
```

**Configuration Changes** (if applicable):
```yaml
# File: {config file}
{suggested config changes}
```

**Dependencies** (if applicable):
```xml
<!-- Update to: -->
<PackageReference Include="{package}" Version="{version}" />
```

### 🔒 Prevention Strategy
{How to prevent this in the future}

### ⏱️ Estimated Fix Time
{5 minutes | 30 minutes | 2 hours | unknown}

## Context

**CI/CD Platform:** {GitHub Actions | Azure DevOps | Jenkins}  
**Build Environment:** {Ubuntu 22.04 | Windows Server 2022}  
**Runtime:** {.NET 9 | Node.js 20 | Python 3.11}  
**Previous Build Status:** {SUCCESS | FAILURE}

## Constraints

- Focus on actionable fixes, not generic advice
- Prioritize likely causes over exhaustive possibilities
- If multiple potential causes, rank by probability
- Include specific file paths, line numbers, commands when possible
```

---

## Usage Example

**Fill in the template:**

```markdown
Analyze this CI/CD build failure and provide diagnosis and fix recommendations.

## Build Information

**Pipeline:** PR Validation  
**Branch:** feature/add-customer-api  
**Commit:** a3f5e2d  
**Trigger:** PR  
**Failed Step:** Build and Test

## Build Log

```
Run dotnet build
  Building IncidentManagement.Api.csproj
  /workspace/Controllers/V2/CustomerController.cs(45,13): error CS1061: 'ICustomerService' does not contain a definition for 'GetByIdAsync' and no accessible extension method 'GetByIdAsync' accepting a first argument of type 'ICustomerService' could be found
  Build FAILED.
  ##[error]Process completed with exit code 1.
```

## Analysis Required
[... rest of template ...]
```

---

## Expected AI Output

```markdown
### 🔴 Root Cause
Compilation error: CustomerController references method GetByIdAsync that doesn't exist in ICustomerService interface.

### 📋 Error Details
- **Error Type:** Compilation error (missing method)
- **Failed Component:** CustomerController.cs
- **Likely Introduced By:** This commit (new controller added without updating interface)

### ✅ Recommended Fix

**Immediate Action:**
Add `GetByIdAsync` method to `ICustomerService` interface, or update `CustomerController` to use an existing method.

**Code Changes:**

Option 1 - Add method to interface:
```csharp
// File: Interfaces/ICustomerService.cs
public interface ICustomerService
{
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    // ... other methods
}
```

Option 2 - Fix controller to use existing method:
```csharp
// File: Controllers/V2/CustomerController.cs
// Line 45
// Change:
var customer = await _customerService.GetByIdAsync(id, cancellationToken);
// To:
var customer = await _customerService.GetById(id); // if this is the actual method name
```

**Most Likely Solution:** Option 1 (add method to interface), since this is a new controller.

### 🔒 Prevention Strategy
- Run `dotnet build` locally before pushing
- Add pre-commit hook to run build
- Enable IDE integration (Roslyn analyzers) to catch at write time

### ⏱️ Estimated Fix Time
5 minutes
```

---

## Common Failure Patterns

### Pattern 1: Dependency Conflict

**Log signature:**
```
error NU1107: Version conflict detected for PackageX
```

**Fix template:**
```markdown
Update all projects to use same version of PackageX:
- Project A: 5.0.1
- Project B: 5.0.2 ← Update to 5.0.1 (or both to 5.0.2)
```

### Pattern 2: Flaky Test

**Log signature:**
```
Test 'OrderServiceTests.GetAll_ReturnsOrders' failed
Expected: 10 items
Actual: 9 items
```

**Fix template:**
```markdown
Likely a flaky test due to timing/order dependency.
- Immediate: Rerun pipeline
- If persists: Investigate test for race condition or shared state
```

### Pattern 3: Missing Environment Variable

**Log signature:**
```
KeyNotFoundException: The given key 'DATABASE_CONNECTION' was not present
```

**Fix template:**
```markdown
Add DATABASE_CONNECTION to pipeline environment variables:
- GitHub Actions: Repository Settings → Secrets and Variables
- Azure DevOps: Pipeline Variables
```

---

## Integration with CI/CD

### GitHub Actions Integration

Add step to CI that auto-diagnoses failures:

```yaml
- name: Diagnose Failure
  if: failure()
  run: |
    echo "Build failed. Running AI diagnosis..."
    # Post log to AI tool
    # Comment diagnosis on PR
```

### Slack Notification

Post diagnosis to Slack channel:

```
🔴 Build Failed: PR #123 by @engineer
Root Cause: Missing NuGet package reference
Fix: Run `dotnet add package Microsoft.Extensions.Logging`
Details: https://github.com/.../actions/runs/...
```

---

## After Generation

1. **Review diagnosis** for accuracy
2. **Apply suggested fix**
3. **Rerun pipeline** to verify
4. **If fix doesn't work**, paste new error back into prompt
5. **Commit** with message: `fix: resolve build failure - {issue} (AI-diagnosed)`

---

## Advanced Usage

### Compare with Previous Successful Build

```markdown
## Previous Successful Build

**Commit:** {PREVIOUS_SHA}
**Log:** {snippet showing what changed}

Compare current failure with previous success to identify what changed.
```

### Include Recent Commits

```markdown
## Recent Commits Since Last Success

- a3f5e2d: feat: add customer controller
- b7e9a1c: chore: update dependencies
- c4d2f8e: refactor: extract service interface

Which commit likely introduced the failure?
```

---

## Related Prompts

- [diagnose-test-failure.md](diagnose-test-failure.md) - More detailed test failure analysis
- [../dev/refactor-template.md](../dev/refactor-template.md) - Fix code issues
- [../observability/analyze-logs.md](../observability/analyze-logs.md) - Runtime failure analysis

---

## Ownership

- **Created by:** DevOps Team
- **Maintained by:** Platform Engineers
- **Last reviewed:** January 2026
- **Usage count:** TBD

---

## Feedback

If diagnosis is incorrect or fix doesn't work, document the case for prompt improvement.
