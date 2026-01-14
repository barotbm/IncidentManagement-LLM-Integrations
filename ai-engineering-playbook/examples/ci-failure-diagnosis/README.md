# Example: Diagnose and Fix CI Pipeline Failure

**Scenario:** Pull request triggers CI pipeline failure, engineer needs to diagnose and fix quickly  
**Maturity Level Demonstrated:** Level 4 (Integrating)  
**Time to Complete:** ~10 minutes (vs. 30-45 minutes manual)

---

## Context

Engineer submits PR #789 to add a new feature. GitHub Actions CI pipeline fails during the "Build and Test" step.

**Error Message:**
```
##[error]Process completed with exit code 1.
```

**Traditional approach:** Scroll through 500+ lines of build log, search for error, Google error message, try fixes, push, wait 5 minutes, repeat.

---

## Scenario Timeline

### 10:15 AM - PR Opened

```
[feature/add-shipping-calculator] → [main]
PR #789: Add shipping cost calculator

Changes:
- Controllers/V2/ShippingController.cs
- Services/ShippingCalculator.cs
- DTOs/ShippingDTOs.cs
```

### 10:17 AM - CI Pipeline Triggered

GitHub Actions starts:
1. Checkout code
2. Restore dependencies
3. **Build project** ← FAILS HERE
4. Run tests (not reached)

### 10:18 AM - Failure Notification

Slack notification:
```
🔴 PR #789 build failed
Branch: feature/add-shipping-calculator
Author: @engineer
Duration: 2m 15s
```

---

## Without AI (Traditional Diagnosis)

### Step 1: Open Build Log

Click "Details" link in PR, navigate to failed job.

### Step 2: Scroll Through Log

500+ lines of output:
```
Restoring packages...
  Restored 147 packages
  ...
  Building project...
  [200 lines of compilation output]
  ...
  ERROR: Build failed
```

### Step 3: Find Error

Eventually locate:
```
Controllers/V2/ShippingController.cs(42,35): error CS0246: The type or namespace name 'ShippingRateDto' could not be found
```

### Step 4: Investigate

- Check if ShippingRateDto exists → It does
- Check namespace → Using statement missing
- Add using statement
- Push commit
- Wait 5 minutes for rebuild

### Step 5: Iterate if Still Fails

**Total time:** 30-45 minutes (including multiple iterations)

---

## With AI (Playbook Approach)

### Step 1: Copy Build Log

Navigate to failed build, copy log (or last 100 lines if full log is too large).

### Step 2: Use CI Diagnosis Prompt

Open: [prompts/cicd/diagnose-build-failure.md](../../prompts/cicd/diagnose-build-failure.md)

**Fill in template:**

```markdown
Analyze this CI/CD build failure and provide diagnosis and fix recommendations.

## Build Information

**Pipeline:** PR Validation  
**Branch:** feature/add-shipping-calculator  
**Commit:** d7f3e9a  
**Trigger:** PR #789  
**Failed Step:** Build and Test

## Build Log

```
[Paste last 100 lines including error]

Run dotnet build
  ...
  Controllers/V2/ShippingController.cs(42,35): error CS0246: The type or namespace name 'ShippingRateDto' could not be found (are you missing a using directive or an assembly reference?)
  Controllers/V2/ShippingController.cs(58,20): error CS0246: The type or namespace name 'ShippingMethodDto' could not be found
  Build FAILED.
  2 Error(s)
  ##[error]Process completed with exit code 1.
```

## Context

**CI/CD Platform:** GitHub Actions  
**Build Environment:** Ubuntu 22.04  
**Runtime:** .NET 9  
**Previous Build Status:** SUCCESS (on main branch)
```

### Step 3: Submit to AI

Paste into GitHub Copilot Chat or use as dedicated prompt.

**AI Output:**

```markdown
### 🔴 Root Cause
Compilation errors: Missing using directives for ShippingRateDto and ShippingMethodDto in ShippingController.cs.

### 📋 Error Details
- **Error Type:** Compilation error (missing namespace)
- **Failed Component:** ShippingController.cs
- **Likely Introduced By:** This commit (new controller added)
- **Impact:** Build blocked, cannot proceed to tests

### ✅ Recommended Fix

**Immediate Action:**
Add missing using directives to ShippingController.cs

**Code Changes:**

File: Controllers/V2/ShippingController.cs
Line: Top of file (add to using block)

```csharp
using IncidentManagement.Api.Controllers.V2;
using IncidentManagement.Api.DTOs;  // ← Add this if missing
using IncidentManagement.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

// Verify these DTOs are in the DTOs namespace:
// - ShippingRateDto
// - ShippingMethodDto
```

**Verification Steps:**
1. Check that DTOs/ShippingDTOs.cs contains ShippingRateDto and ShippingMethodDto
2. If not, create them (see errors indicate they're referenced but not defined)
3. Add `using IncidentManagement.Api.DTOs;` to ShippingController.cs
4. Run `dotnet build` locally to verify

**Alternative Issue:**
If DTOs exist in different namespace, use correct namespace in using statement.

### 🔒 Prevention Strategy
- Run `dotnet build` locally before pushing
- Add pre-commit hook:
  ```bash
  #!/bin/bash
  dotnet build || (echo "Build failed. Fix errors before committing." && exit 1)
  ```
- Enable IDE import suggestions (Roslyn analyzer)

### ⏱️ Estimated Fix Time
**2 minutes** (add using statement, verify locally, push)

### 📝 Additional Notes
This is a common error when creating new controllers that reference DTOs. Always verify:
1. DTOs exist and are in expected namespace
2. Using directives include DTO namespace
3. Local build passes before pushing
```

**Time to get diagnosis:** 15 seconds

### Step 4: Apply Fix

Based on AI analysis, check if DTOs exist:

```bash
# Verify DTOs exist
grep -r "ShippingRateDto" DTOs/
# Output: DTOs/ShippingDTOs.cs:public record ShippingRateDto(...)

# DTOs exist! Missing using directive confirmed.
```

Add using statement:

```csharp
// Controllers/V2/ShippingController.cs
using IncidentManagement.Api.Controllers.V2;
using IncidentManagement.Api.DTOs;  // ← ADDED
using IncidentManagement.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
```

### Step 5: Verify Locally

```bash
dotnet build
# Build succeeded.
```

### Step 6: Commit and Push

```bash
git add Controllers/V2/ShippingController.cs
git commit -m "fix: add missing using directive for ShippingDTOs (AI-diagnosed)"
git push origin feature/add-shipping-calculator
```

### Step 7: CI Passes

10:22 AM - New build triggered  
10:24 AM - ✅ Build succeeded

**Total time from failure to fix:** 7 minutes

---

## Results Comparison

| Metric | Manual | With AI | Improvement |
|--------|--------|---------|-------------|
| **Diagnosis Time** | 10-15 min | 15 sec | **98% faster** |
| **Total Fix Time** | 30-45 min | 7 min | **84% faster** |
| **Iterations** | 2-3 | 1 | **First-time fix** |
| **Context Switches** | Many (log, Google, docs) | Minimal (prompt, apply) | **Fewer interruptions** |

---

## Automated Integration (Level 4)

### GitHub Action: Auto-Diagnose Failures

Add to `.github/workflows/ci.yml`:

```yaml
name: CI

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      
      - name: Build
        id: build
        run: dotnet build
        continue-on-error: true
      
      - name: Diagnose Failure (if failed)
        if: failure()
        id: diagnose
        run: |
          # Capture build log
          echo "Running AI diagnosis..."
          # Call AI service or script with build log
          # Post diagnosis as PR comment
          
      - name: Comment Diagnosis on PR
        if: failure() && github.event_name == 'pull_request'
        uses: actions/github-script@v6
        with:
          script: |
            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: `## 🔴 Build Failed - AI Diagnosis\n\n${process.env.DIAGNOSIS}`
            });
      
      - name: Fail Job if Build Failed
        if: steps.build.outcome == 'failure'
        run: exit 1
```

**Result:** Engineers get instant diagnosis in PR comments without manual prompting.

---

## Common CI Failure Patterns

### Pattern 1: Missing Package Reference

**Log signature:**
```
error CS0246: The type or namespace name 'X' could not be found
```

**AI diagnosis:**
```markdown
Missing NuGet package reference.
Fix: dotnet add package {PackageName}
```

### Pattern 2: Test Timeout

**Log signature:**
```
Test 'OrderServiceTests.ProcessLargeOrder' exceeded timeout of 5000ms
```

**AI diagnosis:**
```markdown
Test timeout - likely performance issue or infinite loop.
Immediate: Increase timeout (workaround)
Long-term: Profile test, optimize code
```

### Pattern 3: Flaky Test

**Log signature:**
```
Test failed 1/3 runs (passed on retry)
```

**AI diagnosis:**
```markdown
Flaky test detected (timing/ordering dependency).
Investigate: Shared state, race condition, external dependency
Action: Quarantine test, fix root cause
```

---

## Key Takeaways

### What AI Did Well

✅ Identified error type instantly (missing using)  
✅ Pinpointed exact file and line  
✅ Suggested specific fix (not generic advice)  
✅ Provided prevention strategy  
✅ Estimated fix time accurately

### What Human Did

🧠 Gathered build log (context)  
🧠 Verified AI diagnosis was correct  
🧠 Applied fix and tested locally  
🧠 Decided to commit and push  
🧠 Monitored CI to confirm resolution

### Lessons Learned

1. **Structured prompts get better results**: Template ensures all context is provided
2. **AI excels at pattern matching**: Error patterns in logs are well-suited for AI
3. **Local verification is critical**: Always test fix locally before pushing
4. **Tag commits for tracking**: "AI-diagnosed" helps measure impact

---

## Variations

### Variation 1: Test Failure (not build failure)

Use same prompt template, paste test output instead of build log.

### Variation 2: Deployment Failure

Adapt prompt for Kubernetes deployment errors, Docker build failures, etc.

### Variation 3: Performance Degradation

AI analyzes metrics/logs to identify performance regressions.

---

## ROI Calculation

**For this specific incident:**
- Time saved: 30 min (manual) - 7 min (AI) = **23 minutes**
- At $100/hour engineer cost: **$38 saved**

**Extrapolated across team:**
- Assume 20 CI failures per week (team of 50 engineers)
- 20 × 23 min = **460 minutes (7.7 hours) saved per week**
- Per year: 400 hours = **$40,000 saved**

**From a single prompt template.**

---

## Next Steps

- Add auto-diagnosis to all CI pipelines
- Track diagnosis accuracy (were AI suggestions correct?)
- Refine prompt based on failure patterns
- Share learnings with team

---

## Related Examples

- [API Controller Example](../api-controller/README.md)
- [Incident Summary Example](../incident-summary/README.md)

---

## Feedback

Did this example help you diagnose CI failures faster? Share your experience.
