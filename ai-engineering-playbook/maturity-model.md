# AI Usage Maturity Model

**Version:** 1.0  
**Last Updated:** January 2026

This maturity model defines five levels of AI usage in engineering. The gap between Level 1 and Level 5 is exponential, not linear.

---

## Overview

| Level | Name | Repeatability | Productivity Gain | Time Investment |
|-------|------|---------------|-------------------|-----------------|
| **1** | **Chatting** | None | 5-10% | None (ad-hoc) |
| **2** | **Prompting** | Low | 15-25% | 1-2 weeks |
| **3** | **Configuring** | Medium | 30-50% | 1-2 months |
| **4** | **Integrating** | High | 50-80% | 3-6 months |
| **5** | **Operating** | Continuous | 80%+ | 6+ months |

**Key Insight:** Most engineers are at Level 1. Real productivity gains start at Level 3.

---

## Level 1: Chatting

### Description

Using AI as a search engine or Q&A tool. Questions are ad-hoc, answers are ephemeral.

### What It Looks Like

- "How do I parse JSON in C#?"
- "What's the difference between `await` and `Task.Run`?"
- "Explain this error message to me."
- Copy-pasting stack traces into ChatGPT

### Characteristics

- **Repeatability:** 0% – Every interaction is one-off
- **Sharing:** None – Knowledge stays with individual
- **Consistency:** Low – Different answers to same questions
- **Value:** Marginal – Slightly faster than Google/Stack Overflow

### Example Workflow

1. Engineer encounters unfamiliar concept
2. Opens ChatGPT or Copilot Chat
3. Types question, gets answer
4. Closes chat window
5. Next engineer asks the same question tomorrow

### Limitations

- Knowledge is not captured
- Prompts are not refined
- Quality varies wildly
- No organizational learning

### Time to Next Level

**1-2 weeks** of intentional practice with structured prompts.

---

## Level 2: Prompting

### Description

Using refined, specific prompts to generate code or documentation. Outputs are better, but still not saved.

### What It Looks Like

```
Generate a C# ASP.NET Core controller for managing orders.

Requirements:
- POST /api/orders (create)
- GET /api/orders/{id} (get by ID)
- GET /api/orders (list all)
- Use DTOs (OrderCreateDto, OrderDto)
- Include validation
- Return proper HTTP status codes

Framework: .NET 9
Pattern: See Controllers/IncidentsController.cs
```

### Characteristics

- **Repeatability:** 20-30% – Can recreate similar prompts
- **Sharing:** Ad-hoc – Shared in Slack or email
- **Consistency:** Medium – Similar prompts produce similar results
- **Value:** Noticeable – 15-25% faster on repetitive tasks

### Example Workflow

1. Engineer needs to create a new API endpoint
2. Crafts detailed prompt with context
3. AI generates controller code
4. Engineer reviews, adjusts, commits
5. Prompt is lost unless manually saved

### Improvements Over Level 1

- More accurate outputs
- Less rework
- Faster iteration
- Context-aware suggestions

### Limitations

- Prompts still not reusable
- Knowledge sharing is manual
- No standardization across team

### Time to Next Level

**1-2 months** of team collaboration and prompt sharing.

---

## Level 3: Configuring

### Description

Prompts and configurations are saved, versioned, and shared. Teams build prompt libraries.

### What It Looks Like

- `.copilot-instructions.md` in repository root
- `prompts/` directory with reusable templates
- Team-shared prompt collection in Git
- Workspace-specific AI configuration

### Characteristics

- **Repeatability:** 70-80% – Prompts are reused regularly
- **Sharing:** Structured – Prompts reviewed via PR
- **Consistency:** High – Entire team uses same patterns
- **Value:** Significant – 30-50% faster on standard tasks

### Example Workflow

1. Engineer needs to create an API controller
2. Uses `prompts/dev/api-controller-template.md`
3. Fills in parameters (entity name, endpoints)
4. AI generates code following team conventions
5. Engineer reviews, commits
6. Next engineer uses same prompt for different entity

### Example: Saved Prompt

**File:** `prompts/dev/api-controller-template.md`

```markdown
# API Controller Template

Generate a C# ASP.NET Core controller for {ENTITY_NAME}.

## Endpoints
- POST /api/{entity} – Create new {entity}
- GET /api/{entity}/{id} – Get by ID
- GET /api/{entity} – List all with pagination
- PUT /api/{entity}/{id} – Update existing
- DELETE /api/{entity}/{id} – Soft delete

## Requirements
- Namespace: IncidentManagement.Api.Controllers.V2
- Use DTO pattern ({Entity}CreateDto, {Entity}UpdateDto, {Entity}Dto)
- Constructor DI for I{Entity}Service
- ModelState validation via [ApiController]
- XML documentation comments
- Return 200/201/204/400/404 status codes

## Constraints
- Do NOT add auth (handled by middleware)
- Do NOT access database directly (use service layer)
- Follow existing patterns in Controllers/V1/

## Context
- Framework: .NET 9
- Existing example: Controllers/IncidentsController.cs
```

### Improvements Over Level 2

- Prompts are reusable
- New engineers onboard faster
- Consistency across team
- Knowledge is preserved

### Limitations

- Still requires manual invocation
- Engineers must remember to use prompts
- No automation in workflows

### Time to Next Level

**3-6 months** of CI/CD integration work.

---

## Level 4: Integrating

### Description

AI is embedded in automated workflows. CI/CD pipelines, PR checks, and deployment processes use AI.

### What It Looks Like

- GitHub Actions run AI-powered PR readiness checks
- Build failures auto-analyzed by AI, diagnosis posted to PR
- Test coverage gaps identified and surfaced
- Architecture conformance validated on every commit

### Characteristics

- **Repeatability:** 95%+ – Fully automated, no manual steps
- **Sharing:** Automatic – Everyone benefits from workflows
- **Consistency:** Very High – Applied uniformly to all code
- **Value:** Transformative – 50-80% faster on integrated tasks

### Example Workflow

1. Engineer creates PR
2. GitHub Action triggers:
   - PR Readiness Agent Skill runs
   - Analyzes code quality, test coverage, conventions
   - Posts comment with checklist and issues
3. Engineer addresses issues flagged by AI
4. Re-runs check, sees green checkmark
5. Human reviewer focuses on high-level concerns

### Example: PR Readiness Check

**Trigger:** PR opened or updated

**Agent Skill:** `agent-skills/pr-readiness/`

**Outputs:**
```markdown
## PR Readiness Check

✅ Code compiles without warnings
✅ All tests pass
✅ Code coverage ≥80%
✅ No new security vulnerabilities
⚠️ Missing XML docs on 3 public methods
❌ Controller missing async/await pattern

### Recommendations
- Add XML comments to OrdersController.CreateOrder()
- Convert GetOrderById() to async Task<IActionResult>
```

**Action:** Posted as PR comment, does NOT auto-merge.

### Improvements Over Level 3

- No manual invocation needed
- Instant feedback
- 100% coverage (every PR checked)
- Scalable across entire organization

### Limitations

- Requires maintenance as codebase evolves
- Agent skills can have false positives
- Needs tuning based on team feedback

### Time to Next Level

**6-12 months** of observability and feedback loops.

---

## Level 5: Operating

### Description

AI is operated as infrastructure. Feedback loops continuously improve prompts and agent skills. Metrics drive optimization.

### What It Looks Like

- Failed AI suggestions analyzed and fed back into prompt refinement
- A/B testing of prompt variations
- Agent skill performance metrics dashboarded
- Quarterly reviews of AI impact vs. investment
- Dedicated platform team maintaining AI infrastructure

### Characteristics

- **Repeatability:** Continuous – Self-improving over time
- **Sharing:** Systemic – Knowledge embedded in platform
- **Consistency:** Adaptive – Evolves with codebase
- **Value:** Compounding – 80%+ productivity gains that increase over time

### Example Workflow

1. AI agent skill runs on 1000 PRs/month
2. Track acceptance rate (how often engineers accept AI suggestions)
3. Identify low-scoring prompts (e.g., test generation only 40% accepted)
4. Analyze rejected outputs for patterns
5. Refine prompt to address common failure modes
6. Re-deploy updated agent skill
7. Measure improvement (acceptance rate increases to 65%)
8. Repeat cycle monthly

### Metrics Dashboard

| Metric | Current | Target | Trend |
|--------|---------|--------|-------|
| **Prompt acceptance rate** | 62% | 70% | ⬆️ +5% |
| **PR cycle time** | 18 hours | 12 hours | ⬇️ -20% |
| **AI-assisted commits** | 45% | 60% | ⬆️ +10% |
| **False positive rate (agent skills)** | 12% | <8% | ⬇️ -3% |

### Example: Feedback Loop

**Problem:** Unit test generation accepted only 40% of the time.

**Analysis:**
- AI-generated tests often don't compile (missing using statements)
- Tests check implementation details, not behavior
- Edge cases are generic, not domain-specific

**Solution:**
- Update `prompts/testing/unit-test-template.md`:
  - Add: "Include all necessary using statements"
  - Add: "Test behavior, not implementation"
  - Add: "Use domain-specific edge cases from {context}"
  
**Result:** Acceptance rate increases to 68%.

### Improvements Over Level 4

- Self-improving system
- Data-driven optimization
- Compounding productivity gains
- Platform-level investment

### Sustainability

- Dedicated team maintains AI infrastructure
- Regular reviews and tuning
- Experimentation budget for new capabilities
- Cross-team knowledge sharing

---

## Maturity Progression

### Individual Contributor Path

| Role | Expected Level | Timeline |
|------|----------------|----------|
| **Junior Engineer** | Level 2 (Prompting) | Within 3 months |
| **Mid-Level Engineer** | Level 3 (Configuring) | Within 6 months |
| **Senior Engineer** | Level 4 (Integrating) | Within 12 months |
| **Staff+ Engineer** | Level 5 (Operating) | Strategic capability |

### Team Path

| Phase | Activities | Duration |
|-------|------------|----------|
| **Phase 1: Foundation** | Train engineers, build prompt library | 3 months |
| **Phase 2: Standardization** | Adopt configurations, share prompts | 3 months |
| **Phase 3: Automation** | Integrate into CI/CD, deploy agent skills | 6 months |
| **Phase 4: Optimization** | Feedback loops, continuous improvement | Ongoing |

---

## Self-Assessment

### Where Are You Today?

Answer these questions honestly:

1. **Do you use AI in your daily work?**
   - Never → Level 0
   - Occasionally for questions → Level 1
   - Regularly for code generation → Level 2

2. **Are your prompts saved and reusable?**
   - No → Level 1-2
   - Yes, personally → Level 3
   - Yes, team-wide → Level 3+

3. **Is AI integrated into your workflows?**
   - No → Level 3 or below
   - Some (e.g., PR checks) → Level 4
   - Fully (CI/CD, deployment, ops) → Level 4+

4. **Do you measure and optimize AI usage?**
   - No → Level 4 or below
   - Yes, with feedback loops → Level 5

### Advancement Checklist

#### To reach Level 2:
- ✅ Use AI for code generation at least weekly
- ✅ Write prompts with context and constraints
- ✅ Review and edit AI outputs before committing

#### To reach Level 3:
- ✅ Save prompts in version control
- ✅ Share prompts with team via PR
- ✅ Use `.copilot-instructions.md` in projects

#### To reach Level 4:
- ✅ Integrate AI into CI/CD pipeline
- ✅ Deploy at least one agent skill
- ✅ Automate PR readiness checks

#### To reach Level 5:
- ✅ Track AI performance metrics
- ✅ Implement feedback loops
- ✅ Continuously refine prompts and skills
- ✅ Measure productivity impact

---

## The Exponential Gap

**Why Level 5 matters:**

A Level 1 engineer spends 40 hours/month doing repetitive work.

A Level 5 engineer spends 10 hours/month on the same work, with better quality.

**That's 30 hours/month saved, per engineer.**

Across a 50-engineer team:
- **1500 hours/month saved**
- **18,000 hours/year saved**
- Equivalent to **9 full-time engineers**

**This is not hype. This is operational leverage.**

---

## Recommended Next Steps

1. **Assess your current level** using the self-assessment above
2. **Set a target level** for next quarter
3. **Identify one skill to develop** (e.g., write better prompts, save configurations)
4. **Measure progress** monthly
5. **Share learnings** with team

**The gap between operators and casual users will widen every quarter. Start climbing now.**

---

*For implementation guidance, see [PLAYBOOK.md](PLAYBOOK.md).*
