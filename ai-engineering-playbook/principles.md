# Core Principles

**Version:** 1.0  
**Last Updated:** January 2026

These principles govern all AI usage across the engineering organization. They are non-negotiable.

---

## 1. Humans Retain Full Ownership

**AI is advisory and assistive. Humans make decisions.**

### What Humans Own

- **Architectural Decisions**: System design, service boundaries, data models
- **Business Logic**: Product requirements, domain rules, edge case handling
- **Security**: Threat modeling, access control, compliance
- **Production Operations**: Deployments, rollbacks, incident response
- **Quality Standards**: What constitutes "good enough" for production

### What AI Provides

- **Code Generation**: Draft implementations for human review
- **Test Suggestions**: Scenarios and edge cases to consider
- **Diagnostic Insights**: Pattern recognition in logs, failures, metrics
- **Automation Opportunities**: Identification of repetitive tasks
- **Learning Acceleration**: Explanations of unfamiliar code or concepts

### Why This Matters

AI cannot understand business context, customer impact, or organizational constraints. It sees patterns in code but not strategy.

**An engineer who blindly accepts AI output is not doing their job.**

---

## 2. Guardrails Are Non-Negotiable

AI must NEVER autonomously:

| Prohibited Action | Reason | Governance |
|-------------------|--------|------------|
| **Auto-merge pull requests** | Code quality and accountability require human review | [guardrails.md](governance/guardrails.md) |
| **Auto-deploy to production** | Deployment risk requires human judgment | [guardrails.md](governance/guardrails.md) |
| **Mutate production systems** | Infrastructure changes require approval | [guardrails.md](governance/guardrails.md) |
| **Override security policies** | Security cannot be compromised for speed | [security.md](governance/security.md) |
| **Make breaking API changes** | Customer impact must be assessed | [guardrails.md](governance/guardrails.md) |
| **Delete data or resources** | Data loss prevention is critical | [guardrails.md](governance/guardrails.md) |
| **Escalate incidents** | Business judgment required | [guardrails.md](governance/guardrails.md) |

### Enforcement

- CI/CD pipelines enforce manual approval gates
- AI tools configured to prevent autonomous actions
- Regular audits of AI usage logs
- Immediate investigation of policy violations

---

## 3. Prompting Is a First-Class Engineering Skill

**Writing effective prompts is as important as writing effective code.**

### Why Prompting Matters

A vague prompt produces vague output. A precise prompt produces precise output.

**Example:**

| Prompt Quality | Output Quality | Time Spent |
|----------------|----------------|------------|
| "Create a controller" | Wrong framework, missing validation, not idiomatic | 45 min rework |
| [See api-controller-template.md](prompts/dev/api-controller-template.md) | Correct pattern, validation included, minimal edits needed | 5 min review |

### Characteristics of Good Prompts

1. **Specific**: Clearly define desired output format and structure
2. **Contextual**: Provide language, framework, existing patterns
3. **Constrained**: Define what NOT to do or include
4. **Verifiable**: Output can be tested or validated objectively
5. **Reusable**: Applicable to similar future tasks

### Developing Prompting Skills

- **Practice**: Iterate on prompts, refine based on output quality
- **Share**: Contribute working prompts to team library
- **Review**: Have peers critique your prompts before saving
- **Learn**: Study prompts that produce high-quality output

**Investment in prompting skills compounds over time.**

---

## 4. Configuration Beats Conversation

**One-off chats don't scale. Saved configurations do.**

### The Problem with Chat-Based AI

- Every engineer re-asks the same questions
- Knowledge is lost when chat history is cleared
- No consistency across team members
- No version control or review

### The Solution: Configuration

- **Prompts stored in Git**: Versioned, reviewable, shareable
- **Agent skills as code**: Parameterized, reusable workflows
- **Team conventions documented**: New engineers use established patterns
- **Continuous improvement**: Prompts refined based on outcomes

### Repeatability Drives Value

| Approach | Repeatability | Value Per Use | Cumulative Value |
|----------|---------------|---------------|------------------|
| **Ad-hoc chat** | 0% | Low | Low |
| **Saved prompt** | 70% | Medium | Medium |
| **Agent skill** | 95% | High | High |
| **CI/CD integration** | 100% | Very High | Compounding |

**Time spent configuring AI pays dividends for months or years.**

---

## 5. Observability Is Required

**If you can't measure it, you can't improve it.**

### What to Measure

#### Productivity Metrics
- Time to first draft (AI-assisted vs. manual)
- PR cycle time (submission to merge)
- Lines of code written per engineer-day
- Rework rate (major revisions required)

#### Quality Metrics
- Defect rate (bugs per 1000 lines of code)
- Test coverage (AI-assisted vs. manual code)
- Code review feedback volume
- Production incidents attributed to AI-assisted code

#### Adoption Metrics
- % of engineers using AI daily/weekly
- % of PRs containing AI-generated code
- Prompt library usage (invocations per month)
- Agent skill utilization (runs per workflow)

See [governance/metrics.md](governance/metrics.md) for full framework.

### Why Observability Matters

Without metrics:
- Can't prove AI provides value
- Can't identify what's working vs. what's not
- Can't justify continued investment
- Can't improve over time

**Measure early, measure often.**

---

## 6. Security and Privacy Are Paramount

**AI tools are a potential data exfiltration vector.**

### Data Handling Rules

| Data Type | AI Tool Usage | Enforcement |
|-----------|---------------|-------------|
| **Customer PII** | ❌ Prohibited | Automated scanning + manual review |
| **Production data** | ❌ Prohibited | Use synthetic data only |
| **Proprietary code** | ✅ GitHub Copilot (enterprise) only | Approved tools list |
| **Security credentials** | ❌ Prohibited | Secret scanning in PRs |
| **Architecture diagrams** | ⚠️ Review required | Security team approval |

### Approved AI Tools

- **GitHub Copilot** (Enterprise license): Code generation, chat
- **Internal AI services** (self-hosted): Custom agent skills
- **Third-party tools**: Pre-approved by Security team only

### Prohibited Tools

- Public ChatGPT, Claude, or other free-tier services
- Browser extensions not vetted by Security
- Any tool that retains or trains on our data

See [governance/security.md](governance/security.md) for complete policy.

---

## 7. Agent Skills Encode Engineering Intent

**Agent skills are reusable AI workflows that embed engineering judgment.**

### What Is an Agent Skill?

An agent skill is a parameterized, multi-step AI capability that solves a recurring problem.

**Example: PR Readiness Checker**

- **Inputs**: PR diff, target branch, coding standards
- **Process**: Analyze code, check conventions, identify issues
- **Outputs**: Checklist, line-level feedback, pass/fail status
- **Integration**: GitHub Action, runs automatically on PR

### Agent Skill vs. Prompt

| Aspect | Prompt | Agent Skill |
|--------|--------|-------------|
| **Complexity** | Single task | Multi-step workflow |
| **Parameters** | Context only | Structured inputs |
| **Ownership** | Any engineer | Senior+ engineers |
| **Review** | Optional | Required |
| **Reusability** | Medium | High |

### Why Agent Skills Matter

- **Scale engineering judgment**: Senior engineer's expertise encoded once, used thousands of times
- **Consistency**: Same standards applied to every PR, every time
- **Speed**: Instant feedback vs. waiting for human review
- **Learning**: Junior engineers learn from feedback

**Agent skills are force multipliers.**

---

## 8. Continuous Improvement Is Built In

**AI capabilities evolve. So should our usage.**

### Feedback Loops

1. **Track failures**: When AI produces bad output, log it
2. **Analyze patterns**: What prompts or skills fail most often?
3. **Refine**: Update prompts and skills based on learnings
4. **Re-deploy**: Push improvements to all users
5. **Measure**: Validate that changes improved outcomes

### Quarterly Review

Every quarter, Tech Leads review:
- Prompt usage statistics (which are used, which are ignored)
- Agent skill effectiveness (acceptance rate, false positives)
- Adoption trends (who's using AI, who's not)
- Productivity metrics (time saved, quality maintained)

**Recommendations feed into next quarter's priorities.**

### Innovation Budget

10% of AI-related effort should be experimental:
- Testing new AI models or tools
- Exploring novel use cases
- Rapid prototyping of new agent skills

**Fail fast, learn faster.**

---

## 9. Engineers Are Accountable

**The engineer who commits the code owns it, regardless of how it was created.**

### No "The AI Did It" Defenses

- AI-generated code undergoes same review as human code
- Engineers must understand and validate AI output
- Bugs in AI-generated code are engineer's responsibility
- PR approvers equally accountable

### Code Attribution

Tag AI-assisted commits:
```
feat: add orders API endpoint

AI-assisted: Copilot generated controller structure,
human added business validation and error handling.
```

**Purpose:**
- Transparency for reviewers
- Data for measuring AI impact
- Accountability for outcomes

### When to Override AI

If AI suggests:
- Insecure patterns (SQL injection risk, hardcoded secrets)
- Performance anti-patterns (N+1 queries, blocking I/O)
- Incorrect business logic
- Code that doesn't pass tests

**Stop. Fix it. Don't merge it.**

---

## 10. Adoption Is Opt-In, But Expected

**We don't mandate AI usage. But we do expect engineers to develop AI skills.**

### Why Opt-In?

- Forcing adoption creates resentment
- Individual workflows vary (some benefit more than others)
- Learning curve differs by experience level

### Why Expected?

- AI skills will be required for career progression
- Teams using AI will outpace teams that don't
- Industry is moving this direction; we need to lead

### Career Development

- **Entry-level engineers**: Expected to use prompts (Level 2)
- **Mid-level engineers**: Expected to configure workflows (Level 3)
- **Senior engineers**: Expected to create agent skills (Level 4)
- **Staff/Principal engineers**: Expected to operate AI infrastructure (Level 5)

**Performance reviews will assess AI skill development.**

---

## Summary

These ten principles form the foundation of AI usage at this company:

1. **Humans own decisions** – AI advises, humans decide
2. **Guardrails are enforced** – No auto-merge, auto-deploy, or autonomous actions
3. **Prompting is a skill** – Good prompts produce good results
4. **Configuration scales** – Saved configurations beat one-off chats
5. **Observability required** – Measure to improve
6. **Security is paramount** – No customer data in AI tools
7. **Agent skills encode intent** – Reusable workflows embed judgment
8. **Continuous improvement** – Feedback loops drive refinement
9. **Engineers are accountable** – You commit it, you own it
10. **Adoption is expected** – Opt-in, but required for growth

**Live these principles. They are not suggestions.**

---

*For detailed implementation, see [PLAYBOOK.md](PLAYBOOK.md).*
