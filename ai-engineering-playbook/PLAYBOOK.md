# AI Engineering Playbook

**Version:** 1.0  
**Audience:** Senior Engineers, Tech Leads, Engineering Leadership  
**Last Updated:** January 2026

---

## 1. Purpose & Philosophy

### Why This Playbook Exists

Engineers at this company already have access to AI tools like GitHub Copilot. Most use AI to answer questions, generate snippets, or explain code. This is **Level 1** usage.

This playbook exists to move the organization from **Level 1** to **Level 5**—from chatting with AI to operating AI as engineering infrastructure.

**The gap between casual AI users and true operators will continue to widen.**

### AI as Infrastructure, Not Chat

Treating AI like infrastructure means:

- **Versioning**: Prompts are stored in Git, not lost in chat history
- **Review**: Prompts and agent skills go through PR review
- **Ownership**: Senior engineers define and maintain agent skills
- **Repeatability**: Configurations are reusable, not recreated each time
- **Observability**: We measure what AI does and the value it provides

AI is not a productivity hack. It's a layer in the engineering stack.

---

## 2. AI Usage Maturity Model

### The Five Levels

| Level | Name | Behavior | Repeatability | Productivity Gain |
|-------|------|----------|---------------|-------------------|
| **1** | **Chatting** | Ad-hoc questions in chat interface | None | 5-10% |
| **2** | **Prompting** | Refined, specific prompts | Low | 15-25% |
| **3** | **Configuring** | Saved prompts, workspace files | Medium | 30-50% |
| **4** | **Integrating** | AI in CI/CD, PR checks, workflows | High | 50-80% |
| **5** | **Operating** | Continuous tuning, feedback loops | Continuous | 80%+ |

### Detailed Progression

#### Level 1: Chatting
- **What it looks like**: "How do I parse JSON in C#?" typed into ChatGPT or Copilot Chat
- **Value**: Quick answers, code snippets
- **Problem**: Not repeatable. Every engineer re-asks the same questions.
- **Time to next level**: 1-2 weeks of intentional practice

#### Level 2: Prompting
- **What it looks like**: "Generate a C# controller for managing incident tickets with POST, GET by ID, and GET all endpoints. Use ASP.NET Core minimal APIs. Include validation for required fields."
- **Value**: More accurate, context-rich outputs
- **Problem**: Still one-off. Prompts are not saved or shared.
- **Time to next level**: 1-2 months with team sharing

#### Level 3: Configuring
- **What it looks like**: Saved `.copilot-instructions.md` files in repository, team-shared prompt templates
- **Value**: Repeatable. New engineers use the same prompts.
- **Problem**: Still manual invocation. Engineers have to remember to use them.
- **Time to next level**: 3-6 months with CI integration

#### Level 4: Integrating
- **What it looks like**: GitHub Actions run AI-powered PR readiness checks. Kubernetes manifest generation triggered on infrastructure changes.
- **Value**: Automatic, embedded in workflows
- **Problem**: Requires maintenance and tuning as codebases evolve
- **Time to next level**: 6-12 months with observability

#### Level 5: Operating
- **What it looks like**: AI-generated metrics feed back into prompt tuning. Failed AI suggestions are analyzed and used to refine agent skills.
- **Value**: Continuous improvement, compounding productivity
- **Problem**: Requires dedicated ownership and platform investment
- **Time to next level**: Ongoing

### Where Should Teams Be?

- **All engineers**: Level 2 (Prompting) within 90 days
- **Senior engineers**: Level 3 (Configuring) within 6 months
- **Tech Leads**: Level 4 (Integrating) within 12 months
- **Staff/Principal Engineers**: Level 5 (Operating) as strategic capability

---

## 3. Core Principles

### 3.1 Humans Retain Full Ownership

AI is **advisory and assistive**, never autonomous.

**Humans own:**
- Architectural decisions
- Business logic and product requirements
- Security and compliance
- Production deployments
- Incident response strategy

**AI provides:**
- Code generation for human review
- Test suggestions
- Diagnostic insights
- Pattern recognition
- Automation of repetitive tasks

### 3.2 Guardrails Are Non-Negotiable

AI must NEVER:
- Auto-merge pull requests
- Auto-deploy to production or staging
- Mutate production databases or infrastructure
- Override security policies or access controls
- Make breaking changes without human review

See [governance/guardrails.md](governance/guardrails.md) for complete list.

### 3.3 Observability Is Required

If you can't measure it, you can't improve it.

Track:
- AI-generated code acceptance rate
- Time saved on repetitive tasks
- Test coverage improvements from AI-generated tests
- PR cycle time with vs. without AI assistance
- Defect rates in AI-assisted vs. manual code

See [governance/metrics.md](governance/metrics.md) for measurement framework.

### 3.4 Prompting Is a First-Class Engineering Skill

Good engineers write good code. Great engineers write good prompts.

**Prompting skills are developed through:**
- Practice and iteration
- Peer review of prompts
- Sharing what works across teams
- Learning from failures (when AI produces bad outputs)

**Prompting is not magic.** It's a learnable, improvable skill.

---

## 4. Where AI Is Encouraged / Restricted / Prohibited

### ✅ Encouraged (High Value, Low Risk)

| Activity | AI Role | Example |
|----------|---------|---------|
| **Code generation** | Draft implementations | Generate CRUD API controller |
| **Test generation** | Create unit/integration tests | Generate xUnit tests for service layer |
| **Documentation** | Write/update docs | Generate XML doc comments |
| **Code review** | Surface issues, suggest improvements | Check for null handling, async patterns |
| **CI/CD diagnostics** | Analyze build/test failures | Parse stack trace, suggest fix |
| **Refactoring** | Suggest modernization | Convert to async/await, apply LINQ |
| **Boilerplate** | Generate repetitive code | DTOs, mappers, validators |

### ⚠️ Restricted (High Value, Moderate Risk)

Requires senior engineer review:

| Activity | AI Role | Review Required |
|----------|---------|-----------------|
| **Security-sensitive code** | Suggest implementation | Security engineer approval |
| **Database migrations** | Generate migration scripts | DBA review |
| **Infrastructure as Code** | Generate Terraform/K8s manifests | Platform engineer review |
| **Performance-critical paths** | Optimize algorithms | Staff engineer review |
| **Public APIs** | Design endpoints | Architect approval |

### 🚫 Prohibited (High Risk)

AI must NOT be used for:

| Activity | Reason |
|----------|--------|
| **Production deployments** | Humans deploy. Period. |
| **Access control decisions** | Security policy requires human judgment |
| **Customer data handling** | Privacy and compliance risk |
| **Financial calculations** | Regulatory and audit requirements |
| **Incident escalation** | Business judgment required |
| **Auto-merging PRs** | Code quality and accountability |

---

## 5. Prompting as a First-Class Engineering Skill

### What Makes a Good Prompt?

A good prompt is:
1. **Specific**: Clearly states the desired output
2. **Contextual**: Provides relevant background (language, framework, patterns)
3. **Constrained**: Defines boundaries (what NOT to do)
4. **Verifiable**: Output can be tested or validated

### Anatomy of a Great Prompt

```markdown
# Bad Prompt
Create a controller for orders.

# Good Prompt
Generate a C# ASP.NET Core Web API controller for managing orders.

Requirements:
- Name: OrdersController
- Namespace: IncidentManagement.Api.Controllers.V2
- Endpoints:
  - POST /api/v2/orders (create new order)
  - GET /api/v2/orders/{id} (get by ID)
  - GET /api/v2/orders (list all, with pagination)
- Use DTO pattern (separate OrderCreateDto, OrderDto)
- Include ModelState validation
- Use constructor dependency injection for IOrderService
- Return appropriate HTTP status codes (201, 200, 404)
- Add XML documentation comments

Constraints:
- Do NOT add authentication logic (handled by middleware)
- Do NOT connect directly to database (use service layer)
- Follow existing controller patterns in Controllers/V1/

Context:
- Framework: .NET 9
- Existing patterns: See Controllers/V1/OrdersV1Controller.cs
```

### Prompt Iteration

Prompts improve through iteration:

1. **Draft**: Initial attempt (often too vague)
2. **Refine**: Add constraints and context after reviewing output
3. **Validate**: Test the output, identify gaps
4. **Iterate**: Adjust prompt, regenerate
5. **Save**: Store working prompt for reuse

**Example**: See [prompts/dev/api-controller-template.md](prompts/dev/api-controller-template.md)

### Sharing Prompts

Prompts should be:
- Stored in `prompts/` directory of this repository
- Submitted via PR for team review
- Tagged with use case (e.g., `#api-generation`, `#testing`)
- Owned by a specific team or engineer

---

## 6. Agent Skills: Encoding Engineering Intent

### What Are Agent Skills?

An **agent skill** is a reusable, parameterized AI capability that encodes engineering judgment.

**Examples:**
- PR Readiness Checker
- Test Coverage Analyzer
- Architecture Conformance Validator
- Security Vulnerability Scanner

### Agent Skill vs. Prompt

| Aspect | Prompt | Agent Skill |
|--------|--------|-------------|
| **Scope** | Single task | Multi-step workflow |
| **Inputs** | Context + instructions | Structured parameters |
| **Outputs** | Code or text | Structured report + recommendations |
| **Ownership** | Any engineer | Senior+ engineers |
| **Review** | Optional | Required |

### Example: PR Readiness Skill

See [agent-skills/pr-readiness/](agent-skills/pr-readiness/) for full implementation.

**Purpose**: Validate PR meets quality standards before human review

**Inputs:**
- PR diff
- Target branch
- Project conventions

**Outputs:**
- Checklist of quality criteria (✅ / ❌)
- Specific line-level issues
- Recommendations for improvement

**Integration:**
- Runs as GitHub Action on PR creation
- Posts comment with results
- Does NOT auto-merge (human reviews final decision)

---

## 7. AI-Assisted Development

### 7.1 Code Generation

**When to use AI:**
- Boilerplate (controllers, DTOs, mappers)
- CRUD operations
- Standard patterns already established in codebase

**How to use AI:**
1. Provide context: existing code patterns, naming conventions
2. Generate draft implementation
3. Review for correctness, security, performance
4. Refactor if needed
5. Write tests (AI-generated or manual)

**Example workflow:**
```
Engineer: Use prompts/dev/api-controller-template.md
AI: Generates OrdersController.cs
Engineer: Reviews, adjusts validation logic, adds custom business rules
Engineer: Commits with message "feat: add orders API (AI-assisted)"
```

### 7.2 Code Review

AI can assist with:
- Null reference checks
- Async/await patterns
- Exception handling completeness
- Naming convention adherence
- Code duplication detection

AI cannot replace:
- Business logic validation
- Architectural fit
- Security threat modeling
- Performance under load assessment

**Recommended approach:**
1. AI performs automated checks (linting, pattern matching)
2. Results surface in PR as comments
3. Human reviewer focuses on high-level concerns

### 7.3 Refactoring

AI excels at mechanical refactoring:
- Convert synchronous to asynchronous
- Apply LINQ or modern C# patterns
- Extract methods or classes
- Rename symbols consistently

AI struggles with:
- Determining WHEN to refactor
- Balancing readability vs. performance
- Assessing business impact of changes

**Guardrail:** Refactoring PRs with >500 lines of AI-generated changes require architect review.

---

## 8. AI-Assisted Testing

### 8.1 Unit Test Generation

**High-value use case.**

AI can generate comprehensive unit tests given:
- Method signature
- Expected behavior
- Edge cases to cover

**Prompt template**: [prompts/testing/unit-test-template.md](prompts/testing/unit-test-template.md)

**Example:**
```
Input: Service method GetIncidentById(int id)
Output: 10+ xUnit test cases covering:
- Valid ID returns incident
- Invalid ID returns null
- Negative ID throws exception
- Database connection failure handling
- etc.
```

**Engineer responsibility:**
- Validate test correctness
- Ensure tests actually fail when code is broken
- Add domain-specific edge cases AI might miss

### 8.2 Integration Test Generation

AI can scaffold integration tests:
- API endpoint tests (HTTP requests/responses)
- Database integration tests (with test containers)
- External service mocking

**Requires more human validation** than unit tests due to:
- Environment setup complexity
- State management between tests
- Timing/race conditions

### 8.3 Test Coverage Analysis

Use AI to:
- Identify untested code paths
- Suggest tests for low-coverage areas
- Prioritize testing efforts

See [agent-skills/test-coverage/](agent-skills/test-coverage/) for automated skill.

---

## 9. AI in CI/CD

### 9.1 Build Failure Diagnosis

**High-value, low-risk use case.**

When CI build fails:
1. AI ingests build log
2. AI identifies root cause (dependency version conflict, syntax error, missing file, etc.)
3. AI suggests fix
4. Human applies fix and re-runs

**Prompt template**: [prompts/cicd/diagnose-build-failure.md](prompts/cicd/diagnose-build-failure.md)

**Integration:**
- GitHub Action comments on PR with diagnosis
- Slack bot posts summary to #engineering channel
- Engineer applies fix manually

### 9.2 Test Failure Analysis

AI can parse test output and:
- Group related failures
- Identify flaky tests (pass/fail inconsistency)
- Suggest root cause (e.g., "All failures in OrdersControllerTests suggest database connection issue")

**Human still:**
- Fixes the actual bug
- Decides whether to rerun or investigate
- Determines if failure blocks release

### 9.3 Release Notes Generation

AI can draft release notes from:
- Merged PR titles and descriptions
- Commit messages
- Issue tracker references

**Engineer must:**
- Review for accuracy
- Add customer-facing context
- Remove internal implementation details
- Approve before publishing

---

## 10. AI for Containerization & Kubernetes

### 10.1 Dockerfile Generation

AI can generate Dockerfiles given:
- Application type (ASP.NET Core API, Node.js app, etc.)
- Base image preferences
- Security requirements (non-root user, minimal image)

**Prompt template**: [prompts/containers/dockerfile-template.md](prompts/containers/dockerfile-template.md)

**Review required:**
- Image size optimization
- Security hardening (vulnerability scanning)
- Multi-stage build correctness

### 10.2 Kubernetes Manifest Generation

AI can scaffold:
- Deployments
- Services
- ConfigMaps / Secrets
- Ingress rules

**Restricted use case:** Requires platform engineer review before applying to cluster.

**Why:** Misconfigured K8s resources can cause outages, resource exhaustion, or security breaches.

**Workflow:**
1. AI generates manifests
2. Engineer reviews
3. Apply to dev cluster first
4. Monitor for issues
5. Promote to staging/prod with approval

---

## 11. AI in Production Operations

### 11.1 Observability & Log Analysis

AI can:
- Summarize log streams during incidents
- Identify error patterns across services
- Correlate failures with recent deployments

**Prompt template**: [prompts/observability/analyze-logs.md](prompts/observability/analyze-logs.md)

**Critical constraint:** AI NEVER takes action (restart pods, roll back deployments, etc.). It provides analysis only.

### 11.2 Incident Response

AI assists with:
- Incident timeline construction (from logs, alerts, commits)
- Summarizing incident for postmortem
- Suggesting similar past incidents

**Human responsibilities:**
- Declaring incident severity
- Coordinating response
- Making rollback/fix decisions
- Communicating to stakeholders

**Prompt template**: [prompts/incidents/incident-summary.md](prompts/incidents/incident-summary.md)

### 11.3 Capacity Planning

AI can analyze:
- Historical resource usage trends
- Predicted growth based on traffic patterns
- Cost optimization opportunities (right-sizing instances)

**Outputs are recommendations, not actions.** Infrastructure changes require human approval.

---

## 12. Governance, Security, and Risk Management

### 12.1 Data Handling

**Customer Data:**
- ❌ NEVER send customer PII to AI tools
- ❌ NEVER paste production data into prompts
- ✅ Use synthetic data or anonymized samples

**Proprietary Code:**
- ✅ GitHub Copilot (enterprise license, data not retained)
- ⚠️ Third-party AI services: review data retention policies
- ❌ Public AI tools (ChatGPT free tier): do NOT paste internal code

See [governance/security.md](governance/security.md) for detailed policies.

### 12.2 Bias and Fairness

AI-generated code may reflect biases in training data.

**Risks:**
- Naming conventions that assume US-centric contexts
- Date/time handling that ignores timezones
- Validation logic that excludes valid international formats

**Mitigation:**
- Human review for internationalization concerns
- Explicit prompt constraints (e.g., "support ISO 8601 dates")
- Diverse review teams

### 12.3 Compliance and Audit

**For regulated workloads (PCI, HIPAA, SOC2):**
- AI-generated code undergoes same review as human-written code
- Audit logs must capture AI tool usage
- Compliance officer approval required for AI in critical systems

### 12.4 Liability and Accountability

**Key principle:** Engineer who commits the code is accountable, regardless of who (or what) wrote it.

- No "the AI did it" defenses
- Engineer must understand and validate AI-generated code
- PR approvers are equally accountable

---

## 13. Measuring Productivity & Quality

### 13.1 What to Measure

| Metric | How to Measure | Target |
|--------|----------------|--------|
| **AI code acceptance rate** | Lines of AI code kept vs. discarded | >60% |
| **Time to PR** | Time from task start to PR submission | -20% |
| **PR cycle time** | Time from PR open to merge | -15% |
| **Test coverage** | % coverage on AI-assisted code | ≥ manual code |
| **Defect rate** | Bugs per 1000 lines (AI vs. manual) | ≤ manual code |
| **Rework rate** | PRs requiring major revision | ≤ manual code |

### 13.2 Leading vs. Lagging Indicators

**Leading (early signals):**
- Number of engineers using prompts/
- Prompt sharing activity (PRs to this repo)
- Agent skill adoption (invocations per week)

**Lagging (outcome):**
- Velocity (story points per sprint)
- Quality (production defects)
- Efficiency (cost per feature)

### 13.3 Qualitative Feedback

Quarterly survey:
- "How often do you use AI in your workflow?" (Daily / Weekly / Rarely / Never)
- "AI makes me more productive." (Strongly Agree ... Strongly Disagree)
- "I trust AI-generated code." (1-5 scale)
- "What's the biggest obstacle to using AI more?" (Free text)

See [governance/metrics.md](governance/metrics.md) for full measurement framework.

---

## 14. Rollout & Adoption Strategy

### Phase 1: Foundation (Months 1-3)

**Goal:** Establish baseline practices and train early adopters.

**Activities:**
- Publish this playbook
- Train senior engineers on Level 2-3 usage
- Create initial prompt library (5-10 templates)
- Run "Lunch & Learn" sessions on effective prompting

**Success criteria:**
- 50% of engineers have used prompts/ at least once
- 3+ agent skills defined and documented

### Phase 2: Integration (Months 4-6)

**Goal:** Embed AI into daily workflows.

**Activities:**
- Deploy PR readiness agent skill to CI/CD
- Expand prompt library to 20+ templates
- Measure acceptance rate and time savings
- Identify and coach low-adoption teams

**Success criteria:**
- 80% of PRs analyzed by AI agent skill
- 30% of code commits tagged as AI-assisted
- Measurable reduction in PR cycle time

### Phase 3: Optimization (Months 7-12)

**Goal:** Achieve Level 4-5 maturity for key workflows.

**Activities:**
- Implement feedback loops (failed AI suggestions analyzed)
- Tune agent skills based on real usage data
- Expand to advanced use cases (log analysis, incident response)
- Share case studies and wins

**Success criteria:**
- 90% engineer adoption
- 40%+ productivity gain in measured workflows
- 5+ agent skills in production use

### Phase 4: Continuous Improvement (Ongoing)

**Goal:** Operate AI as infrastructure.

**Activities:**
- Regular prompt/skill review and updates
- Experiment with new AI capabilities
- Share learnings across engineering org
- Contribute to industry best practices

**Success criteria:**
- AI usage is second nature, not a special initiative
- Compounding productivity gains year-over-year

---

## 15. Key Takeaways

1. **AI is infrastructure, not a chat tool.** Treat it accordingly.

2. **Prompts are code.** Version them. Review them. Own them.

3. **Agent skills encode engineering intent.** Senior engineers define them.

4. **Humans retain full ownership.** AI advises, humans decide.

5. **Guardrails are non-negotiable.** No auto-merge, no auto-deploy, no autonomous production changes.

6. **Prompting is a first-class skill.** Good engineers write good prompts.

7. **Configuration beats conversation.** Repeatable workflows beat one-off chats.

8. **Measure everything.** If you can't measure it, you can't improve it.

9. **The gap will widen.** Operators will outpace casual users exponentially.

10. **Start small, scale fast.** Begin with prompts, progress to agent skills, integrate into CI/CD.

---

## Appendix: Quick Reference

| Need | Resource |
|------|----------|
| **Create a new API controller** | [prompts/dev/api-controller-template.md](prompts/dev/api-controller-template.md) |
| **Generate unit tests** | [prompts/testing/unit-test-template.md](prompts/testing/unit-test-template.md) |
| **Diagnose CI failure** | [prompts/cicd/diagnose-build-failure.md](prompts/cicd/diagnose-build-failure.md) |
| **Review PR readiness** | [agent-skills/pr-readiness/](agent-skills/pr-readiness/) |
| **Analyze test coverage** | [agent-skills/test-coverage/](agent-skills/test-coverage/) |
| **Summarize incident** | [prompts/incidents/incident-summary.md](prompts/incidents/incident-summary.md) |
| **Security policies** | [governance/security.md](governance/security.md) |
| **Guardrails reference** | [governance/guardrails.md](governance/guardrails.md) |

---

**End of Playbook**

*For questions or feedback, contact Engineering Leadership or file an issue in this repository.*
