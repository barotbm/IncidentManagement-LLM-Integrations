# PR Readiness Agent Skill

**Category:** Code Quality  
**Maturity Level:** 4 (Integrating)  
**Owner:** Staff Engineers  
**Last Updated:** January 2026

---

## Purpose

Automatically validate pull requests against quality standards before human review.

---

## Overview

The PR Readiness Agent Skill analyzes pull requests and provides a structured quality checklist. It does NOT auto-merge — it provides advisory feedback to help engineers ensure PRs are ready for human review.

### Benefits

- **Faster PR cycle time**: Catch issues before human reviewer sees them
- **Consistent standards**: Same checks applied to every PR
- **Learning tool**: Junior engineers see what "ready" looks like
- **Reviewer efficiency**: Human reviewers focus on logic, not formatting

---

## Inputs

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| `pr_diff` | string | Full diff of PR changes | Yes |
| `target_branch` | string | Branch being merged into (e.g., `main`) | Yes |
| `file_paths` | array | List of changed files | Yes |
| `pr_description` | string | PR title and description | Yes |
| `project_conventions` | object | Coding standards, naming, patterns | No |
| `test_coverage_threshold` | number | Minimum coverage % (default 80) | No |

---

## Quality Checks

### 1. Code Compilation

**Check:** Does the code compile without errors?

**Implementation:**
- Run `dotnet build` (or equivalent) on PR branch
- Fail if compilation errors exist
- Warn if compilation warnings exist

**Output:**
```markdown
✅ Code compiles without errors
⚠️ 3 compilation warnings detected (see details)
```

### 2. Test Coverage

**Check:** Do tests exist and cover new code adequately?

**Implementation:**
- Run code coverage tool (Coverlet for .NET)
- Calculate coverage for changed lines only
- Compare to threshold (default 80%)

**Output:**
```markdown
✅ Test coverage: 85% (threshold: 80%)
❌ Test coverage: 45% (threshold: 80%) - Missing tests for:
  - OrdersController.CreateAsync
  - OrdersController.UpdateAsync
```

### 3. All Tests Passing

**Check:** Do all unit and integration tests pass?

**Implementation:**
- Run test suite
- Report failures with stack traces

**Output:**
```markdown
✅ All 127 tests passed
❌ 2 tests failed:
  - OrdersServiceTests.GetById_InvalidId_ReturnsNull
  - OrdersControllerTests.Create_ValidInput_Returns201
```

### 4. Coding Conventions

**Check:** Does code follow team conventions?

**Criteria:**
- Naming conventions (PascalCase for public, camelCase for private)
- Async methods end in `Async`
- Controllers use DTOs, not domain models
- Services injected via constructor DI
- No hardcoded strings/magic numbers

**Output:**
```markdown
✅ Naming conventions followed
❌ Convention violations:
  - Line 45: Method GetOrder should be GetOrderAsync (async method)
  - Line 78: Hardcoded connection string (use configuration)
```

### 5. Security Scan

**Check:** No security vulnerabilities introduced?

**Implementation:**
- Run SAST tool (SonarQube, CodeQL)
- Check for common patterns:
  - SQL injection risk
  - XSS vulnerabilities
  - Hardcoded secrets
  - Insecure crypto

**Output:**
```markdown
✅ No security vulnerabilities detected
❌ 1 high-severity issue:
  - Line 92: Potential SQL injection (parameterize query)
```

### 6. Documentation

**Check:** Public APIs and complex methods documented?

**Criteria:**
- Public classes have XML doc comments
- Public methods have summary, params, returns
- Complex logic has inline comments

**Output:**
```markdown
⚠️ Missing documentation:
  - CustomerController: Missing XML docs
  - CustomerService.ProcessOrder: Missing <returns> tag
```

### 7. No Large Files

**Check:** Are any files excessively large?

**Threshold:** 500 lines per file (configurable)

**Output:**
```markdown
⚠️ Large files detected:
  - OrdersController.cs: 650 lines (consider splitting)
```

### 8. PR Description Quality

**Check:** Does PR have adequate description?

**Criteria:**
- Title is descriptive (not "fix" or "update")
- Description explains what/why
- References issue/ticket if applicable

**Output:**
```markdown
✅ PR description adequate
❌ PR description issues:
  - Title too vague: "fix bug" → Suggest: "fix: null check in OrdersController.GetById"
  - No linked issue (add "Fixes #123")
```

### 9. Breaking Changes

**Check:** Does PR introduce breaking changes?

**Implementation:**
- Detect public API signature changes
- Check for removed methods/properties
- Warn if DB schema modified

**Output:**
```markdown
⚠️ Potential breaking changes:
  - OrderDto.CreatedDate renamed to CreatedAt (API contract change)
  - IOrderService.GetAll() signature changed (impacts consumers)
```

### 10. Dependency Updates

**Check:** Are new dependencies justified and secure?

**Implementation:**
- Detect new NuGet/npm packages
- Run vulnerability scan
- Warn if license incompatible

**Output:**
```markdown
✅ No new dependencies
⚠️ New dependency added:
  - Newtonsoft.Json 13.0.1 (consider built-in System.Text.Json)
  - License: MIT (compatible)
  - Vulnerabilities: None
```

---

## Output Format

### Summary Checklist

```markdown
## PR Readiness Report

**PR:** #{PR_NUMBER}  
**Branch:** {SOURCE_BRANCH} → {TARGET_BRANCH}  
**Status:** 🟢 READY | 🟡 NEEDS WORK | 🔴 NOT READY

### Checklist

| Check | Status | Details |
|-------|--------|---------|
| Code compiles | ✅ | No errors |
| Tests pass | ✅ | 127/127 passed |
| Test coverage | ✅ | 85% (threshold: 80%) |
| Coding conventions | ❌ | 2 violations (see below) |
| Security scan | ✅ | No issues |
| Documentation | ⚠️ | Missing XML docs on 1 method |
| File size | ✅ | All files <500 lines |
| PR description | ✅ | Adequate |
| Breaking changes | ⚠️ | 1 potential break (review) |
| Dependencies | ✅ | No new dependencies |

### Issues to Address

#### ❌ Coding Conventions (2)

**File:** OrdersController.cs  
**Line:** 45  
**Issue:** Method `GetOrder` should be `GetOrderAsync` (async convention)  
**Fix:** Rename method to `GetOrderAsync`

**File:** OrdersService.cs  
**Line:** 78  
**Issue:** Hardcoded connection string  
**Fix:** Use `IConfiguration` or environment variable

#### ⚠️ Documentation (1)

**File:** CustomerController.cs  
**Issue:** Missing XML documentation comments  
**Fix:** Add `/// <summary>` tags to public methods

### Recommendations

- Address ❌ issues before requesting review
- Consider addressing ⚠️ warnings
- All ✅ checks passed

### Next Steps

1. Fix coding convention violations
2. Add missing documentation
3. Re-run PR Readiness check
4. Request human review once all checks pass
```

---

## Integration

### GitHub Actions

```yaml
name: PR Readiness Check

on:
  pull_request:
    types: [opened, synchronize]

jobs:
  pr-readiness:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Run PR Readiness Agent
        id: pr-check
        run: |
          # Invoke agent skill (pseudo-code)
          dotnet run --project Tools/PRReadiness -- \
            --pr-diff "${{ github.event.pull_request.diff_url }}" \
            --target-branch "${{ github.base_ref }}" \
            --coverage-threshold 80
      
      - name: Comment PR
        uses: actions/github-script@v6
        with:
          script: |
            const report = require('./pr-readiness-report.json');
            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: report.markdown
            });
      
      - name: Set Status
        if: failure()
        run: exit 1  # Block merge if critical issues
```

### Slack Integration

Post summary to #pr-reviews channel:

```
🟡 PR #456 by @engineer needs work

Issues to fix:
- 2 coding convention violations
- Missing XML docs

View full report: https://github.com/...
```

---

## Configuration

**File:** `.pr-readiness.yml` (in repository root)

```yaml
pr-readiness:
  coverage:
    threshold: 80
    exclude_files:
      - "**/Migrations/**"
      - "**/Tests/**"
  
  conventions:
    async_suffix: true
    public_docs_required: true
    max_file_lines: 500
  
  security:
    block_on_high_severity: true
    scan_dependencies: true
  
  breaking_changes:
    warn_on_api_changes: true
    warn_on_schema_changes: true
  
  pr_description:
    min_length: 50
    require_issue_link: true
```

---

## Ownership and Maintenance

### Skill Owner

**Staff/Principal Engineers** own this skill definition.

### Maintenance Cadence

- **Weekly:** Review false positives, tune thresholds
- **Monthly:** Analyze acceptance rate (how often engineers fix flagged issues)
- **Quarterly:** Update checks based on team feedback

### Success Metrics

| Metric | Target |
|--------|--------|
| **Adoption:** % of PRs analyzed | >90% |
| **Acceptance:** % of flagged issues fixed | >70% |
| **False positive rate** | <10% |
| **Time saved per PR** | 15 minutes (less rework) |

---

## Evolution Roadmap

### V1 (Current)

- Automated checks: compilation, tests, coverage
- Manual review required for merge

### V2 (Next 3 months)

- AI-powered code review (suggest improvements)
- Auto-fix minor issues (formatting, imports)
- Learning mode (track which suggestions are accepted)

### V3 (6-12 months)

- Contextual recommendations based on past PRs
- Team-specific patterns learned over time
- Integration with code review bot

---

## Related Resources

- [Prompt: PR Review](../../prompts/dev/pr-review-template.md)
- [Governance: Guardrails](../../governance/guardrails.md)
- [Metrics: PR Cycle Time](../../governance/metrics.md#pr-cycle-time)

---

## FAQs

### Q: Will this auto-merge PRs?

**No.** This skill only provides advisory feedback. Humans make the final merge decision.

### Q: Can I override the checks?

**Yes.** Engineers can add `[skip pr-readiness]` to PR description if absolutely necessary (e.g., hotfix). Overrides are logged and reviewed monthly.

### Q: What if the skill gives false positives?

File an issue with details. We tune the skill to reduce false positives over time.

### Q: Do all checks need to pass?

**Critical checks (❌)** should be fixed. **Warnings (⚠️)** are advisory and can be addressed at engineer's discretion.

---

## Feedback

Submit improvements via PR to this file. Include:
- What check is missing or inaccurate
- Example PR where skill failed
- Suggested improvement

**This skill is a living document. Continuous improvement is expected.**
