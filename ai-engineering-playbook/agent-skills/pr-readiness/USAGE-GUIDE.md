# Using PR Readiness in Your Workflow

**Practical guide for integrating PR Readiness checks into PR creation and review**

---

## Quick Start: 3 Ways to Use PR Readiness

### Option 1: Automated (GitHub Actions) ⭐ RECOMMENDED

PR checks run automatically when you open/update a PR.

### Option 2: Manual (Before Opening PR)

Run checks locally before creating PR.

### Option 3: AI-Assisted (Interactive)

Use AI to validate your PR against readiness criteria.

---

## Option 1: Automated GitHub Actions Integration

### Setup (One-Time, by Tech Lead)

1. **Create workflow file:** `.github/workflows/pr-readiness.yml`

```yaml
name: PR Readiness Check

on:
  pull_request:
    types: [opened, synchronize, reopened]

permissions:
  contents: read
  pull-requests: write  # To post comments

jobs:
  pr-readiness:
    runs-on: ubuntu-latest
    
    steps:
      - name: Checkout code
        uses: actions/checkout@v3
        with:
          fetch-depth: 0  # Full history for better analysis
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        id: build
        run: dotnet build --no-restore
        continue-on-error: true
      
      - name: Run tests with coverage
        id: test
        run: |
          dotnet test --no-build --verbosity normal \
            --collect:"XPlat Code Coverage" \
            --results-directory ./coverage
        continue-on-error: true
      
      - name: Generate PR Readiness Report
        id: readiness
        uses: actions/github-script@v7
        with:
          script: |
            const fs = require('fs');
            
            // Gather data
            const buildSuccess = '${{ steps.build.outcome }}' === 'success';
            const testSuccess = '${{ steps.test.outcome }}' === 'success';
            
            // Read coverage report (simplified - use proper parser in production)
            let coverage = 'N/A';
            try {
              // Parse coverage XML/JSON here
              coverage = '85%';  // Placeholder
            } catch (e) {
              coverage = 'Unable to parse';
            }
            
            // Generate report
            let status = '🟢 READY';
            let issues = [];
            
            if (!buildSuccess) {
              status = '🔴 NOT READY';
              issues.push('❌ Build failed - fix compilation errors');
            }
            
            if (!testSuccess) {
              status = '🔴 NOT READY';
              issues.push('❌ Tests failed - fix failing tests');
            }
            
            if (coverage !== 'N/A' && parseInt(coverage) < 80) {
              status = '🟡 NEEDS WORK';
              issues.push(`⚠️ Coverage ${coverage} below 80% threshold`);
            }
            
            // Create comment
            const report = `## PR Readiness Report
            
**Status:** ${status}

### Checklist

| Check | Status |
|-------|--------|
| Code compiles | ${buildSuccess ? '✅' : '❌'} |
| Tests pass | ${testSuccess ? '✅' : '❌'} |
| Test coverage | ${coverage} ${parseInt(coverage) >= 80 ? '✅' : '⚠️'} |

${issues.length > 0 ? '### Issues to Address\n\n' + issues.join('\n') : '### ✅ All checks passed!'}

---
*Automated PR Readiness Check - [See criteria](../ai-engineering-playbook/agent-skills/pr-readiness/README.md)*
`;
            
            // Post comment
            github.rest.issues.createComment({
              issue_number: context.issue.number,
              owner: context.repo.owner,
              repo: context.repo.repo,
              body: report
            });
      
      - name: Check readiness status
        run: |
          if [[ "${{ steps.build.outcome }}" != "success" ]] || [[ "${{ steps.test.outcome }}" != "success" ]]; then
            echo "PR not ready - critical checks failed"
            exit 1
          fi
```

2. **Commit workflow file:**
```bash
git add .github/workflows/pr-readiness.yml
git commit -m "ci: add PR readiness automated checks"
git push
```

3. **Done!** All future PRs automatically get checked.

### Usage (Every PR)

When you create a PR:

1. **Open PR as usual:**
   ```bash
   git push origin feature/my-feature
   # Create PR via GitHub UI or gh CLI
   ```

2. **Wait 2-3 minutes** - GitHub Action runs automatically

3. **Review bot comment** on PR:
   ```
   ## PR Readiness Report
   
   **Status:** 🟡 NEEDS WORK
   
   ### Issues to Address
   ❌ Tests failed - fix failing tests
   ⚠️ Coverage 65% below 80% threshold
   ```

4. **Fix issues:**
   ```bash
   # Fix test failures
   # Add missing tests
   git add .
   git commit -m "test: add missing tests for coverage"
   git push
   ```

5. **Check runs again** automatically on new push

6. **Once green (✅), request human review**

---

## Option 2: Manual Pre-PR Check (Local)

### Setup (One-Time)

Create a script: `scripts/check-pr-readiness.ps1`

```powershell
# check-pr-readiness.ps1
# Run PR readiness checks locally before creating PR

Write-Host "🔍 Running PR Readiness Checks..." -ForegroundColor Cyan

$errors = @()
$warnings = @()

# 1. Check if code compiles
Write-Host "`n1️⃣ Checking compilation..." -ForegroundColor Yellow
$buildResult = dotnet build --no-restore
if ($LASTEXITCODE -ne 0) {
    $errors += "❌ Build failed"
} else {
    Write-Host "✅ Build successful" -ForegroundColor Green
}

# 2. Run tests
Write-Host "`n2️⃣ Running tests..." -ForegroundColor Yellow
$testResult = dotnet test --no-build
if ($LASTEXITCODE -ne 0) {
    $errors += "❌ Tests failed"
} else {
    Write-Host "✅ All tests passed" -ForegroundColor Green
}

# 3. Check test coverage
Write-Host "`n3️⃣ Checking test coverage..." -ForegroundColor Yellow
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage-local
# Parse coverage (simplified)
# In production, use reportgenerator or similar
Write-Host "⚠️ Coverage check - implement parser" -ForegroundColor Yellow
$warnings += "⚠️ Coverage check needs manual review"

# 4. Check PR description (if PR already created)
Write-Host "`n4️⃣ Checking PR description..." -ForegroundColor Yellow
if (Test-Path .git/PULL_REQUEST_EDITMSG) {
    $prDesc = Get-Content .git/PULL_REQUEST_EDITMSG
    if ($prDesc.Length -lt 50) {
        $warnings += "⚠️ PR description too short (add context)"
    }
}

# Summary
Write-Host "`n📊 Summary:" -ForegroundColor Cyan
Write-Host "Errors: $($errors.Count)" -ForegroundColor $(if ($errors.Count -eq 0) { "Green" } else { "Red" })
Write-Host "Warnings: $($warnings.Count)" -ForegroundColor Yellow

if ($errors.Count -gt 0) {
    Write-Host "`n🔴 NOT READY - Fix these issues:" -ForegroundColor Red
    $errors | ForEach-Object { Write-Host $_ -ForegroundColor Red }
    exit 1
} elseif ($warnings.Count -gt 0) {
    Write-Host "`n🟡 NEEDS WORK - Consider addressing:" -ForegroundColor Yellow
    $warnings | ForEach-Object { Write-Host $_ -ForegroundColor Yellow }
    Write-Host "`nYou can proceed, but consider fixing warnings." -ForegroundColor Yellow
} else {
    Write-Host "`n🟢 READY - PR looks good!" -ForegroundColor Green
}
```

### Usage (Before Each PR)

```powershell
# Before creating PR:
git add .
git commit -m "feat: add new feature"

# Run readiness check
.\scripts\check-pr-readiness.ps1

# If green, create PR:
git push origin feature/my-feature
gh pr create --title "feat: add new feature" --body "Description..."

# If red, fix issues first:
# Fix errors, then re-run check
```

---

## Option 3: AI-Assisted Interactive Check

### Method A: GitHub Copilot with Workspace Context (⭐ EASIEST)

**GitHub Copilot can automatically read the PR readiness .md files in your workspace!**

**Step 1: Open your changed files in VS Code**

Just have your modified files open or staged.

**Step 2: Ask Copilot directly**

In Copilot Chat, simply ask:

```
@workspace Review my current changes against the PR readiness criteria in agent-skills/pr-readiness/README.md
```

**OR even simpler:**

```
Review my PR against our PR readiness checklist
```

**Copilot will:**
- ✅ Find the `agent-skills/pr-readiness/README.md` file
- ✅ Read the 10 quality checks
- ✅ Analyze your staged/modified files
- ✅ Report which criteria pass/fail
- ✅ Give specific line numbers and fixes

**Example Copilot response:**

```markdown
Based on the PR readiness criteria in agent-skills/pr-readiness/README.md:

🟡 NEEDS WORK

✅ Code compiles
✅ Naming conventions
❌ Missing async suffix:
    - Line 45 in OrdersController.cs: GetOrder() should be GetOrderAsync()
⚠️ Missing XML docs:
    - Line 12: CustomerController class needs XML summary
✅ DTOs used (not domain models)
✅ Proper HTTP status codes

Recommendations:
1. Rename GetOrder to GetOrderAsync
2. Add /// <summary> to CustomerController
```

**Pro tips:**

```
# Be specific about what to check
"Check if my changes follow the async naming convention from our PR readiness guide"

# Ask about specific files
"Does Controllers/OrdersController.cs meet our PR readiness standards?"

# Get test suggestions
"What tests are missing based on our test coverage criteria?"

# Reference multiple docs
"@workspace Review against PR readiness and coding conventions"
```

**Why this works:**
- Copilot has access to all files in your workspace
- `@workspace` explicitly tells it to search workspace files
- The .md files are treated as context/instructions
- Copilot can cross-reference code against documentation

---

### Method B: Manual Prompt with Diff (Traditional)

**Step 1: Generate PR diff**

```powershell
# Get your changes
git diff main...HEAD > my-pr-diff.txt
```

**Step 2: Create AI prompt using PR Readiness criteria**

```markdown
Review this PR for readiness using the PR Readiness checklist:

**PR Context:**
- Branch: feature/add-customer-api
- Target: main
- Description: Add REST API for customer management

**PR Diff:**
```
[Paste contents of my-pr-diff.txt]
```

**Review against PR Readiness criteria:**

1. ✅ Code compiles without errors
2. ✅ All tests pass
3. ✅ Test coverage ≥80%
4. ✅ Coding conventions followed (async suffix, naming, DI)
5. ✅ No security vulnerabilities
6. ✅ Public APIs documented (XML comments)
7. ✅ No files >500 lines
8. ✅ PR description adequate
9. ✅ No breaking changes (or documented if necessary)
10. ✅ No new vulnerable dependencies

**For each criterion, indicate:**
- ✅ Pass
- ⚠️ Warning (with details)
- ❌ Fail (with specific issues and line numbers)

**Output format:**
- Status: 🟢 READY | 🟡 NEEDS WORK | 🔴 NOT READY
- List of issues to fix
- Recommendations
```

**Step 3: Submit to GitHub Copilot Chat or Copilot**

Paste the prompt above.

**Step 4: Review AI feedback**

AI responds:
```markdown
## PR Readiness Review

**Status:** 🟡 NEEDS WORK

### Issues Found

❌ **Coding Conventions (2 issues)**

1. Line 45: `GetCustomer` should be `GetCustomerAsync` (async naming)
2. Line 78: Hardcoded connection string (use IConfiguration)

⚠️ **Documentation (1 issue)**

Line 12: Missing XML doc comment on `CustomerController` class

### Recommendations

1. Rename method to follow async convention
2. Move connection string to appsettings.json
3. Add XML documentation

After fixing, re-run this review.
```

**Step 5: Fix issues, re-review**

```powershell
# Fix issues
# Re-generate diff
git diff main...HEAD > my-pr-diff.txt
# Re-run AI review with new diff
```

---

## GitHub Copilot Workspace Integration

### How Copilot Finds and Uses PR Readiness Files

**Yes! GitHub Copilot can automatically identify and follow the .md files in this repository.**

#### What Copilot Can Do

✅ **Automatic file discovery:**
```
You: "Review my changes against our PR standards"
Copilot: [Finds agent-skills/pr-readiness/README.md automatically]
         [Applies the 10 criteria to your code]
```

✅ **Multi-file context:**
```
You: "@workspace Check PR readiness and security policies"
Copilot: [Reads pr-readiness/README.md + governance/security.md]
         [Cross-references both documents]
```

✅ **Prompt templates:**
```
You: "Use prompts/dev/api-controller-template.md to review this controller"
Copilot: [Reads the template]
         [Checks if your code follows the template requirements]
```

✅ **Learning from examples:**
```
You: "Show me how to fix this based on examples/api-controller/"
Copilot: [Reads the example]
         [Suggests fixes matching the pattern]
```

#### Effective Copilot Queries

**Basic PR review:**
```
@workspace Review my staged changes for PR readiness
```

**Specific criteria:**
```
Check if my controller follows the async naming convention from our playbook
```

**With file reference:**
```
@workspace Does this code meet the standards in agent-skills/pr-readiness/README.md?
```

**Multiple documents:**
```
@workspace Review against:
- PR readiness criteria
- Security policies (governance/security.md)
- API controller template (prompts/dev/api-controller-template.md)
```

**Generate code with standards:**
```
@workspace Create a new API controller following our templates and PR readiness standards
```

#### Example Conversation

**You:**
```
@workspace I'm about to create a PR. Check if my changes are ready.
```

**Copilot:**
```
I found your PR readiness criteria in agent-skills/pr-readiness/README.md.
Reviewing your 3 modified files against the 10 checks:

✅ 1. Code compiles (no syntax errors detected)
✅ 2. Tests exist (found ProductsControllerTests.cs)
⚠️ 3. Test coverage unclear (run coverage tool to verify ≥80%)
❌ 4. Coding conventions:
   - ProductsController.cs line 45: GetProduct should be GetProductAsync
   - ProductService.cs line 78: Hardcoded string "DefaultConnection"
✅ 5. Security: No obvious vulnerabilities
⚠️ 6. Documentation:
   - ProductsController.cs line 12: Missing XML doc summary
✅ 7. File sizes (all under 500 lines)
❌ 8. PR description: Not checked (create PR first)
✅ 9. No breaking changes detected
✅ 10. No new dependencies

Status: 🟡 NEEDS WORK

Fix these 3 issues:
1. Rename GetProduct → GetProductAsync
2. Move "DefaultConnection" to configuration
3. Add XML doc to controller class

Would you like me to show you how to fix these?
```

**You:**
```
Yes, show me the fixes.
```

**Copilot:**
```
1. Async naming (ProductsController.cs line 45):
   Change:
   public async Task<IActionResult> GetProduct(int id)
   
   To:
   public async Task<IActionResult> GetProductAsync(int id)

2. Hardcoded string (ProductService.cs line 78):
   Change:
   var connString = "DefaultConnection";
   
   To:
   var connString = _configuration.GetConnectionString("DefaultConnection");

3. XML docs (ProductsController.cs line 12):
   Add:
   /// <summary>
   /// API controller for managing products
   /// </summary>
   [ApiController]
   [Route("api/v2/products")]
   public class ProductsController : ControllerBase
```

#### Setup Tips

**1. Keep playbook files in workspace root or accessible path:**
```
your-repo/
├── ai-engineering-playbook/  ← Copilot can find this
│   ├── agent-skills/
│   ├── prompts/
│   └── governance/
└── src/
```

**2. Use `.copilot-instructions.md` for project-specific overrides:**
```markdown
<!-- .copilot-instructions.md in your project root -->

When reviewing code or generating code for this project:

1. Follow PR readiness criteria: ../ai-engineering-playbook/agent-skills/pr-readiness/README.md
2. Use API templates: ../ai-engineering-playbook/prompts/dev/
3. Apply security policies: ../ai-engineering-playbook/governance/security.md
4. Follow existing patterns in: src/Controllers/V1/

Project-specific rules:
- All API endpoints must have [Authorize] unless explicitly public
- Use structured logging (ILogger) not Console.WriteLine
- DTOs must be in DTOs/ folder
```

**3. Reference docs explicitly when needed:**
```
@workspace Using the pr-readiness checklist, review my changes

# Copilot knows to look in agent-skills/pr-readiness/
```

#### Limitations

❌ **Copilot cannot:**
- Run your tests or build
- Measure actual code coverage %
- Execute static analysis tools (SonarQube)
- Check if code actually compiles

✅ **Copilot can:**
- Review code against documented standards
- Identify convention violations
- Suggest improvements
- Find missing documentation
- Detect common security patterns
- Cross-reference multiple documents

**Solution:** Combine Copilot review (fast, contextual) with automated checks (builds, tests, coverage).

---

## Hybrid Approach (BEST PRACTICE)

Combine all three:

### Pre-Commit (Local)
```powershell
# Add to .git/hooks/pre-commit
dotnet build && dotnet test
```

### Pre-Push (AI-Assisted)
```powershell
# Before pushing:
.\scripts\check-pr-readiness.ps1

# If warnings, use AI to review:
# "Review my changes against PR readiness criteria"
```

### Post-PR (Automated)
- GitHub Action runs on PR open
- Posts results as comment
- Blocks merge if critical failures

---

## Integration with AI Prompts

### When Generating Code with AI

**Include PR readiness criteria in your prompt:**

```markdown
Generate a C# API controller for managing Products.

[... your requirements ...]

**Ensure PR Readiness:**
- Use async/await throughout
- Add XML documentation to all public methods
- Follow naming conventions (Async suffix)
- Include proper HTTP status codes
- No hardcoded values

This code will be reviewed against:
- ../ai-engineering-playbook/agent-skills/pr-readiness/README.md
```

AI will generate code that's already compliant with your criteria.

### When Reviewing AI-Generated Code

**Use PR readiness as review checklist:**

```markdown
Review this AI-generated code against PR Readiness criteria:

[Paste code]

Check:
1. Compilation ✅
2. Tests (none yet - suggest tests needed) ⚠️
3. Conventions ✅
4. Security ✅
5. Documentation ✅

What's missing? What should I fix before committing?
```

---

## Quick Reference

| Stage | Tool | Time | Benefit |
|-------|------|------|---------|
| **Pre-commit** | Local script | 30 sec | Catch errors before committing |
| **Pre-push** | AI review | 2 min | Get feedback before PR |
| **Post-PR** | GitHub Action | 3 min | Automated, consistent checks |
| **During review** | Manual checklist | 5 min | Human validation |

---

## PR Readiness vs. SonarQube

### How They Differ

| Aspect | SonarQube | PR Readiness |
|--------|-----------|--------------|
| **Focus** | Code quality, technical debt, vulnerabilities | Overall PR preparedness for review |
| **Scope** | Static code analysis only | Build + Tests + Coverage + Conventions + Documentation + SonarQube |
| **Analysis Type** | Rule-based, deterministic | Checklist-based, can include AI context |
| **Output** | Quality gates, code smells, bugs, vulnerabilities | Go/No-Go decision for human review |
| **Timing** | Usually post-commit | Pre-commit, pre-push, AND post-PR |
| **Customization** | Rules and quality profiles | Team-specific conventions and standards |

### How They Complement Each Other

**PR Readiness includes SonarQube as one check among many:**

```
PR Readiness Checklist:
├── 1. Code compiles ✅
├── 2. Tests pass ✅
├── 3. Test coverage ≥80% ✅
├── 4. Coding conventions ✅
├── 5. Security scan (SonarQube) ← HERE
├── 6. Documentation ✅
├── 7. File size ✅
├── 8. PR description ✅
├── 9. Breaking changes ✅
└── 10. Dependencies ✅
```

**In practice:**

```yaml
# GitHub Action: PR Readiness Check
steps:
  - name: Build
    run: dotnet build
  
  - name: Test
    run: dotnet test --collect:"XPlat Code Coverage"
  
  - name: SonarQube Scan  # ← SonarQube is ONE step
    uses: SonarSource/sonarqube-scan-action@master
    env:
      SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
  
  - name: Check SonarQube Quality Gate
    run: |
      # Wait for SonarQube analysis
      # Fail if quality gate fails
  
  - name: Generate PR Readiness Report  # ← Combines ALL checks
    run: |
      # Check build: ✅
      # Check tests: ✅
      # Check coverage: ✅
      # Check SonarQube: ❌ (3 code smells)
      # Overall status: 🟡 NEEDS WORK
```

### What SonarQube Does Well

✅ **Static Analysis:**
- Code smells (complexity, duplicates)
- Bug patterns (null pointer, resource leaks)
- Security vulnerabilities (OWASP Top 10)
- Technical debt quantification
- Code coverage analysis

✅ **Language Support:**
- 25+ languages
- Deep language-specific rules
- Framework-specific checks (ASP.NET, Spring)

✅ **Trending:**
- Track quality over time
- New code vs. overall code
- Quality gate history

### What PR Readiness Adds

✅ **Broader Context:**
- "Did you write tests?" (not just coverage %)
- "Is PR description adequate?"
- "Are files too large?"
- "Did you document the API?"

✅ **Team Conventions:**
- "Async methods end in Async" (your team's rule)
- "Controllers use DTOs" (architectural decision)
- "No hardcoded strings" (project-specific)

✅ **Human Readiness:**
- "Is this ready for a human to review?"
- "What will save reviewer time?"
- "What's missing that will cause rework?"

✅ **AI-Powered Insights:**
- Context-aware suggestions
- Learning from past PRs
- Natural language explanations

### Example: Combined Analysis

**SonarQube says:**
```
❌ Quality Gate Failed
- 3 Code Smells (cognitive complexity)
- 1 Bug (potential null pointer)
- Coverage: 85% (passes)
```

**PR Readiness says:**
```
🟡 NEEDS WORK

Issues:
❌ SonarQube quality gate failed (see details)
⚠️ Missing XML docs on 2 public methods
⚠️ PR description too brief (add context)
✅ All other checks passed

Recommendation:
1. Fix SonarQube issues (null pointer, refactor complex method)
2. Add documentation
3. Expand PR description
Then request human review.
```

### When to Use Each

| Scenario | Tool | Reason |
|----------|------|--------|
| **Continuous quality monitoring** | SonarQube | Track quality trends over time |
| **Pre-PR self-check** | PR Readiness | "Am I ready to ask for review?" |
| **Automated PR gate** | Both | SonarQube for code quality, PR Readiness for overall prep |
| **Team-specific rules** | PR Readiness | Custom conventions beyond SonarQube rules |
| **Security compliance** | SonarQube | Deep vulnerability scanning |
| **Documentation check** | PR Readiness | SonarQube doesn't validate docs adequacy |

### Integration Strategy

**Best Practice: Use Both**

```yaml
# .github/workflows/pr-validation.yml
name: PR Validation

on:
  pull_request:

jobs:
  quality-checks:
    runs-on: ubuntu-latest
    steps:
      # Step 1: Build and test
      - name: Build
        run: dotnet build
      
      - name: Test with coverage
        run: dotnet test --collect:"XPlat Code Coverage"
      
      # Step 2: SonarQube scan
      - name: SonarQube Scan
        uses: SonarSource/sonarqube-scan-action@master
        env:
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
      
      - name: SonarQube Quality Gate
        uses: SonarSource/sonarqube-quality-gate-action@master
        timeout-minutes: 5
        env:
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
      
      # Step 3: PR Readiness (includes SonarQube result)
      - name: PR Readiness Check
        run: |
          # Gather results from all previous steps
          # Include SonarQube quality gate status
          # Generate holistic readiness report
          # Post to PR as comment
```

### Summary

**SonarQube:**
- "Is this code high quality?" ✅
- Focused on code analysis
- Industry-standard rules
- Great for compliance and trending

**PR Readiness:**
- "Is this PR ready for human review?" ✅
- Focused on review efficiency
- Team-specific standards
- Great for reducing rework and cycle time

**Together:**
- SonarQube ensures code quality
- PR Readiness ensures review efficiency
- Neither replaces the other — they're complementary

**Think of it this way:**
- **SonarQube** = "Your code has technical issues"
- **PR Readiness** = "Your PR has issues that will slow down review"

Both are important, but solve different problems.

---

## FAQs

### Q: Should I run all three checks?

**Best practice:**
- Always: Automated GitHub Action (post-PR)
- Frequently: Local script (pre-push)
- As needed: AI review (for complex changes)

### Q: Can I skip checks for hotfixes?

Add `[skip pr-readiness]` to PR description. Use sparingly, all skips are logged.

### Q: What if checks fail but I disagree?

Request review from senior engineer. If justified, they can approve despite warnings.

### Q: How do I customize criteria for my team?

Edit `.pr-readiness.yml` config file (see main README).

---

## Next Steps

1. **Set up automated checks** (GitHub Actions)
2. **Create local pre-push script**
3. **Try AI-assisted review** on your next PR
4. **Measure impact** (PR cycle time before/after)

---

## Cross-Repository Usage: Using the Playbook Across All Projects

**Instead of copying .md files to every repo, use these approaches to access the playbook globally:**

### Approach 1: VS Code Multi-Root Workspace (⭐ EASIEST)

Add the playbook as a second folder in your workspace. Copilot can then access it from any project.

**Setup:**

1. **Clone playbook once to a central location:**
```powershell
# Clone to a central location (one time)
cd C:\Dev\
git clone <your-playbook-repo-url> ai-engineering-playbook
```

2. **Add to any project workspace:**

In VS Code: **File → Add Folder to Workspace...** → Select `C:\Dev\ai-engineering-playbook`

Or create a workspace file `my-project.code-workspace`:
```json
{
  "folders": [
    {
      "name": "My Project",
      "path": "C:\\TFS\\MyProject"
    },
    {
      "name": "AI Playbook (Reference)",
      "path": "C:\\Dev\\ai-engineering-playbook"
    }
  ],
  "settings": {
    "github.copilot.chat.codeGeneration.instructions": [
      {
        "file": "C:\\Dev\\ai-engineering-playbook\\.copilot-instructions.md"
      }
    ]
  }
}
```

**Usage:**
```
@workspace Review my changes for PR readiness

# Copilot sees both folders:
# - Your project code
# - The playbook .md files
```

**Benefits:**
- ✅ One playbook copy for all projects
- ✅ Updates immediately available everywhere
- ✅ No duplication
- ✅ Works with @workspace command

---

### Approach 2: Global Copilot Instructions File

Create a **global** `.copilot-instructions.md` that references your central playbook.

**Setup:**

1. **Create global instructions file:**

In your user profile: `C:\Users\<YourName>\.copilot-instructions.md`

```markdown
# Global AI Engineering Standards

When working in ANY repository, follow these standards:

## PR Readiness Criteria

Before creating a PR, ensure:
1. ✅ Code compiles with no errors
2. ✅ Tests exist and pass (≥80% coverage)
3. ✅ Async methods end with "Async"
4. ✅ No hardcoded secrets/connection strings
5. ✅ Public APIs have XML documentation
6. ✅ Files are ≤500 lines (split if larger)
7. ✅ PR description explains what/why
8. ✅ No breaking changes without discussion
9. ✅ Dependencies reviewed and approved
10. ✅ SonarQube quality gate passes

Full criteria: C:\Dev\ai-engineering-playbook\agent-skills\pr-readiness\README.md

## Code Generation Templates

- **API Controllers:** C:\Dev\ai-engineering-playbook\prompts\dev\api-controller-template.md
- **Unit Tests:** C:\Dev\ai-engineering-playbook\prompts\testing\unit-test-template.md
- **Dockerfiles:** C:\Dev\ai-engineering-playbook\prompts\containers\dockerfile-template.md

## Security Policies

Never include in AI prompts:
- Credentials, API keys, tokens
- PII or customer data
- Production database connection strings

See: C:\Dev\ai-engineering-playbook\governance\security.md
```

2. **Configure VS Code to use it:**

In VS Code User Settings (`Ctrl+,` → search "copilot instructions"):

```json
{
  "github.copilot.chat.codeGeneration.instructions": [
    {
      "file": "C:\\Users\\YourName\\.copilot-instructions.md"
    }
  ]
}
```

**Benefits:**
- ✅ Works in all repos automatically
- ✅ No workspace setup needed
- ✅ Enforces standards everywhere
- ❌ **Note:** As of Jan 2026, global `.copilot-instructions.md` support may vary by Copilot version

---

### Approach 3: VS Code User Snippets (For Prompt Templates)

Convert prompt templates to **global snippets** that work in all projects.

**Setup:**

1. **Open User Snippets:**
   - `Ctrl+Shift+P` → "Preferences: Configure User Snippets"
   - Select language (e.g., "csharp.json" for C#)

2. **Add playbook prompt as snippet:**

`C:\Users\<YourName>\AppData\Roaming\Code\User\snippets\csharp.json`:

```json
{
  "Generate API Controller (Playbook Standard)": {
    "prefix": "ai-controller",
    "description": "Copilot prompt to generate API controller following playbook standards",
    "body": [
      "// @copilot Generate a REST API controller with these requirements:",
      "// - Entity: ${1:EntityName}",
      "// - Endpoints: GET all, GET by ID, POST, PUT, DELETE",
      "// - Async/await throughout (methods end with 'Async')",
      "// - Constructor injection for services",
      "// - XML documentation on all public methods",
      "// - FluentValidation for input validation",
      "// - Structured logging (ILogger)",
      "// - Return appropriate status codes (200, 201, 400, 404, 500)",
      "// - Use DTOs for request/response (not domain models)",
      "// ",
      "// Follow patterns in C:\\Dev\\ai-engineering-playbook\\prompts\\dev\\api-controller-template.md",
      "$0"
    ]
  },
  "Generate Unit Tests (Playbook Standard)": {
    "prefix": "ai-test",
    "description": "Copilot prompt to generate unit tests following playbook standards",
    "body": [
      "// @copilot Generate xUnit tests for ${1:ClassName} with:",
      "// - AAA pattern (Arrange, Act, Assert)",
      "// - FluentAssertions for readable assertions",
      "// - Moq for dependencies",
      "// - Test naming: MethodName_Scenario_ExpectedResult",
      "// - Cover: happy path, edge cases, error conditions",
      "// - ≥80% code coverage target",
      "// ",
      "// Follow patterns in C:\\Dev\\ai-engineering-playbook\\prompts\\testing\\unit-test-template.md",
      "$0"
    ]
  }
}
```

3. **Usage in any project:**

Type `ai-controller` → Press `Tab` → Copilot generates controller following playbook standards

**Benefits:**
- ✅ Works in all repos
- ✅ Fast (type prefix + Tab)
- ✅ Consistent code generation
- ❌ Limited to code generation (not PR review)

---

### Approach 4: Git Submodule (For Teams)

Add playbook as a **Git submodule** in each repo (automated, version-controlled).

**Setup:**

1. **Add playbook as submodule:**

```powershell
# In your project repo
git submodule add <playbook-repo-url> .ai-playbook
git commit -m "Add AI Engineering Playbook as submodule"
```

2. **Team members clone with submodules:**

```powershell
git clone --recurse-submodules <your-repo-url>

# Or if already cloned:
git submodule update --init --recursive
```

3. **Update playbook across all repos:**

```powershell
# In any repo with the submodule
cd .ai-playbook
git pull origin main
cd ..
git add .ai-playbook
git commit -m "Update AI playbook to latest version"
```

**File structure:**
```
your-repo/
├── .ai-playbook/          ← Submodule (playbook content)
│   ├── agent-skills/
│   ├── prompts/
│   └── governance/
├── src/
├── tests/
└── .copilot-instructions.md  → References .ai-playbook/
```

**Benefits:**
- ✅ Version-controlled playbook reference
- ✅ Team gets same playbook version
- ✅ Easy to update across repos
- ❌ Requires Git submodule knowledge

---

### Approach 5: Symlink (Windows Advanced)

Create a **symbolic link** to the central playbook.

**Setup:**

```powershell
# Run as Administrator
cd C:\TFS\MyProject
New-Item -ItemType SymbolicLink -Path ".ai-playbook" -Target "C:\Dev\ai-engineering-playbook"
```

**Add to `.gitignore`:**
```
.ai-playbook/
```

**Benefits:**
- ✅ Instant updates (links to live playbook)
- ✅ No duplication
- ❌ Requires admin rights
- ❌ Doesn't work across machines (each dev sets up locally)

---

## Comparison: Which Approach to Use?

| Approach | Effort | Team Consistency | Auto-Updates | Best For |
|----------|--------|------------------|--------------|----------|
| **Multi-Root Workspace** | Low | Medium | ✅ Instant | Individual developers |
| **Global Copilot Instructions** | Low | High | ✅ Instant | Enforcing standards everywhere |
| **User Snippets** | Medium | Low | Manual | Fast code generation |
| **Git Submodule** | Medium | ✅ High | Manual (`git pull`) | Teams with version control needs |
| **Symlink** | Low | Low | ✅ Instant | Power users (local only) |

### Recommended Combination

**For individuals:**
- Multi-Root Workspace (easy setup)
- User Snippets (fast prompts)

**For teams:**
- Git Submodule (version control)
- Global Copilot Instructions (enforced standards)

---

## Example: Setting Up for a Team

**1. Tech Lead creates central playbook repo:**
```powershell
cd C:\Dev
git init ai-engineering-playbook
# Add all playbook .md files
git add .
git commit -m "Initial playbook"
git remote add origin https://github.com/yourorg/ai-playbook.git
git push -u origin main
```

**2. Each developer adds playbook to their VS Code:**

**Option A (Workspace):**
```powershell
# Add to current workspace
code --add "C:\Dev\ai-engineering-playbook"
```

**Option B (Global Instructions):**
```powershell
# Create global instructions
New-Item "C:\Users\$env:USERNAME\.copilot-instructions.md"
# Add content referencing C:\Dev\ai-engineering-playbook
```

**3. Use in any project:**
```
@workspace Review my PR against our playbook standards
```

**4. Update playbook (Tech Lead):**
```powershell
cd C:\Dev\ai-engineering-playbook
# Make changes
git commit -am "Update PR readiness criteria"
git push
```

**5. Pull updates (Developers):**
```powershell
cd C:\Dev\ai-engineering-playbook
git pull
# Changes immediately available in all workspaces
```

---

**See also:**
- [PR Readiness Agent Skill README](README.md) - Full specification
- [Prompts: API Controller](../../prompts/dev/api-controller-template.md) - Generate PR-ready code
- [Example: API Controller](../../examples/api-controller/README.md) - See it in action
