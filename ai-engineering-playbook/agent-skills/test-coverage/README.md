# Test Coverage Analysis Agent Skill

**Category:** Quality Assurance  
**Maturity Level:** 4 (Integrating)  
**Owner:** Senior Engineers  
**Last Updated:** January 2026

---

## Purpose

Analyze code test coverage, identify gaps, and suggest specific tests to add.

---

## Overview

The Test Coverage Analysis Agent Skill goes beyond simple coverage percentage. It identifies:
- Which code paths are untested
- Which edge cases are missing
- What tests should be added (with suggestions)

### Benefits

- **Targeted testing**: Focus effort on highest-risk untested code
- **Quality improvement**: Reduce defects by testing critical paths
- **Learning tool**: Junior engineers see what comprehensive testing looks like
- **Coverage trends**: Track coverage improvements over time

---

## Inputs

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| `project_path` | string | Path to project or solution | Yes |
| `coverage_report` | string | Path to coverage XML/JSON report | Yes |
| `target_coverage` | number | Target coverage % (default 80) | No |
| `file_filter` | array | Files to analyze (default: all) | No |
| `exclude_patterns` | array | Patterns to exclude (e.g., Migrations) | No |
| `context` | string | Additional context (e.g., "focus on Controllers") | No |

---

## Analysis Steps

### Step 1: Parse Coverage Report

Extract from coverage report:
- Total line coverage %
- Branch coverage %
- Coverage by file
- Uncovered lines per file
- Untested methods

### Step 2: Identify Critical Gaps

Prioritize untested code by:

**Priority 1 (Critical):**
- Public API endpoints (controllers, services)
- Security-sensitive code (auth, validation)
- Error handling code (catch blocks, exception handling)
- Business logic (domain services)

**Priority 2 (High):**
- Data access layer
- Integration points (external APIs)
- Complex algorithms (sorting, filtering)

**Priority 3 (Medium):**
- Helper methods
- Mappers and converters
- Configuration code

**Priority 4 (Low):**
- Auto-generated code
- Simple getters/setters
- Logging statements

### Step 3: Suggest Specific Tests

For each gap, generate test suggestion:
- Test name
- Scenario to test
- Expected outcome
- Priority

---

## Output Format

### Coverage Summary

```markdown
## Test Coverage Analysis

**Project:** IncidentManagement.Api  
**Total Coverage:** 68% (Target: 80%)  
**Status:** 🔴 BELOW TARGET

### Coverage by Category

| Category | Coverage | Target | Status |
|----------|----------|--------|--------|
| **Controllers** | 55% | 80% | 🔴 |
| **Services** | 75% | 80% | 🟡 |
| **Repositories** | 90% | 80% | ✅ |
| **DTOs/Models** | 95% | 80% | ✅ |
| **Overall** | 68% | 80% | 🔴 |

### Coverage Trend

| Date | Coverage | Change |
|------|----------|--------|
| Jan 13 | 68% | +3% |
| Jan 6 | 65% | +2% |
| Dec 30 | 63% | - |

🟢 Trending up (+5% over 2 weeks)
```

### Critical Gaps

```markdown
## Critical Coverage Gaps

### 🔴 Priority 1: Controllers (55% coverage)

#### File: Controllers/V2/OrdersController.cs

**Uncovered Lines:** 45-52, 78-85, 101-110  
**Uncovered Methods:**
- `CreateAsync` (0% coverage)
- `UpdateAsync` (30% coverage)
- `DeleteAsync` (0% coverage)

**Impact:** High (public API, customer-facing)

---

### Suggested Tests for OrdersController.CreateAsync

#### Test 1: Happy Path
**Name:** `CreateAsync_ValidOrder_Returns201Created`  
**Priority:** P0  
**Scenario:**
- Arrange: Valid OrderCreateDto
- Act: Call CreateAsync
- Assert: Returns 201, Location header set, order ID returned

**Lines Covered:** 45-48

#### Test 2: Validation Failure
**Name:** `CreateAsync_InvalidOrder_Returns400BadRequest`  
**Priority:** P0  
**Scenario:**
- Arrange: OrderCreateDto with missing required fields
- Act: Call CreateAsync
- Assert: Returns 400, ValidationProblemDetails in response

**Lines Covered:** 49-52

#### Test 3: Service Exception
**Name:** `CreateAsync_ServiceThrows_Returns500`  
**Priority:** P1  
**Scenario:**
- Arrange: Mock service to throw exception
- Act: Call CreateAsync
- Assert: Returns 500 (or lets middleware handle)

**Lines Covered:** (error path)

#### Test 4: Duplicate Order
**Name:** `CreateAsync_DuplicateOrderId_Returns409Conflict`  
**Priority:** P1  
**Scenario:**
- Arrange: Order with existing ID
- Act: Call CreateAsync
- Assert: Returns 409 Conflict

**Lines Covered:** (validation path)

---

### 🟡 Priority 2: Services (75% coverage)

#### File: Services/OrderService.cs

**Uncovered Lines:** 125-132  
**Uncovered Methods:**
- `CalculateTotalAsync` (partial coverage)

**Impact:** Medium (business logic)

**Suggested Tests:**
1. `CalculateTotalAsync_WithDiscount_AppliesCorrectly` (P1)
2. `CalculateTotalAsync_WithTax_CalculatesCorrectly` (P1)
3. `CalculateTotalAsync_NegativeQuantity_ThrowsException` (P2)

---
```

### Quick Wins

```markdown
## Quick Wins (Easy Coverage Improvements)

These files are almost fully tested — small effort, big coverage gain:

| File | Current | Missing | Effort | Impact |
|------|---------|---------|--------|--------|
| CustomerService.cs | 78% | 2 methods | 15 min | +5% |
| IncidentValidator.cs | 85% | 1 edge case | 10 min | +3% |
| OrderMapper.cs | 92% | 1 null check | 5 min | +2% |

**Total potential gain: +10% coverage with ~30 minutes of work**

**Recommended action:** Start here for fast progress toward 80% target.
```

### Test Generation Prompts

```markdown
## AI-Generated Test Stubs

Use these prompts to generate tests for gaps:

### For OrdersController.CreateAsync

```markdown
Generate xUnit tests for OrdersController.CreateAsync method.

Test scenarios:
1. Valid input returns 201 Created
2. Invalid input returns 400 Bad Request
3. Duplicate order returns 409 Conflict
4. Service exception returns 500

Use Moq for IOrderService dependency.
Use FluentAssertions for assertions.

Code context:
[paste OrdersController.cs lines 40-60]
```

**Estimated output:** 4 test methods covering lines 45-52

---

### For OrderService.CalculateTotalAsync

```markdown
Generate xUnit tests for OrderService.CalculateTotalAsync.

Test scenarios:
1. Order with items calculates correct total
2. Order with discount applies discount correctly
3. Order with tax adds tax correctly
4. Empty order returns 0
5. Negative quantity throws ArgumentException

Code context:
[paste OrderService.cs lines 120-140]
```

**Estimated output:** 5 test methods covering lines 125-132
```

---

## Integration

### CI/CD Pipeline

```yaml
name: Test Coverage Analysis

on:
  pull_request:
  schedule:
    - cron: '0 0 * * 0'  # Weekly on Sunday

jobs:
  coverage-analysis:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Run Tests with Coverage
        run: |
          dotnet test --collect:"XPlat Code Coverage" \
            --results-directory ./coverage
      
      - name: Analyze Coverage
        id: analyze
        run: |
          dotnet run --project Tools/CoverageAnalysis -- \
            --report ./coverage/coverage.cobertura.xml \
            --target 80 \
            --output ./coverage-report.md
      
      - name: Comment on PR
        uses: actions/github-script@v6
        with:
          script: |
            const fs = require('fs');
            const report = fs.readFileSync('./coverage-report.md', 'utf8');
            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: report
            });
      
      - name: Fail if Below Target
        run: |
          # Extract coverage % from report
          # Exit 1 if below threshold (optional enforcement)
```

### Dashboard

Display coverage trends in team dashboard:

```
📊 Test Coverage Dashboard

Current: 68% (🔴 Target: 80%)
Trend: 🟢 +5% over 2 weeks
Quick Wins Available: +10% (30 min effort)

Top Gaps:
1. OrdersController (55%)
2. PaymentService (62%)
3. InventoryController (70%)

Top Performers:
1. IncidentController (98%)
2. UserRepository (95%)
3. AuthService (92%)
```

---

## Configuration

**File:** `.coverage-analysis.yml`

```yaml
coverage-analysis:
  target:
    overall: 80
    by_category:
      controllers: 80
      services: 80
      repositories: 75
      models: 90
  
  exclude:
    - "**/Migrations/**"
    - "**/Program.cs"
    - "**/Startup.cs"
  
  prioritize:
    critical:
      - "**/Controllers/**"
      - "**/Services/**"
      - "**/Security/**"
    high:
      - "**/Repositories/**"
      - "**/Validators/**"
    low:
      - "**/DTOs/**"
      - "**/Models/**"
  
  suggestions:
    max_per_file: 5
    include_code_snippets: true
    generate_test_stubs: true
```

---

## Ownership and Maintenance

### Skill Owner

**Senior Engineers + QA Team**

### Maintenance Cadence

- **Weekly:** Review coverage trends
- **Monthly:** Update prioritization rules
- **Quarterly:** Validate suggested tests are helpful

### Success Metrics

| Metric | Target |
|--------|--------|
| **Overall coverage** | 80% |
| **Coverage trend** | +2% per month |
| **Suggested tests added** | >50% |
| **Quick wins completed** | >70% |

---

## Evolution Roadmap

### V1 (Current)

- Parse coverage reports
- Identify gaps
- Suggest test scenarios (text)

### V2 (Next 3 months)

- Auto-generate test code (not just suggestions)
- Mutation testing integration (verify tests actually catch bugs)
- Historical trend analysis (coverage over 6 months)

### V3 (6-12 months)

- Predictive analysis (which gaps lead to bugs in production)
- Smart prioritization (focus on high-churn, high-risk code)
- Integration with PR workflow (suggest tests inline)

---

## Related Resources

- [Prompt: Unit Test Template](../../prompts/testing/unit-test-template.md)
- [Agent Skill: PR Readiness](../pr-readiness/README.md)
- [Metrics: Test Coverage](../../governance/metrics.md#test-coverage)

---

## FAQs

### Q: Will this auto-generate and commit tests?

**No.** It suggests what tests to write. Engineers write and commit tests.

### Q: What if I disagree with a suggestion?

Suggestions are advisory. Use engineering judgment. If a suggestion is consistently unhelpful, file feedback to improve the skill.

### Q: Does 80% coverage mean 80% bug-free?

**No.** Coverage measures execution, not correctness. High coverage + good assertions = quality.

### Q: Can I exclude files from analysis?

**Yes.** Configure exclusions in `.coverage-analysis.yml`.

---

## Feedback

Submit improvements via PR to this file. Include:
- What suggestions were unhelpful
- What gaps were missed
- What metrics should be added

**This skill improves through usage feedback.**
