# Context Engineering Playbook (v2)

**Version:** 2.0  
**Audience:** Engineers, Tech Leads, Engineering Leadership  
**Last Updated:** January 2026

---

## 1. Summary (Why this matters)

Context engineering is the evolution of prompt engineering. It focuses on delivering **the right information, in the right format**, so GitHub Copilot generates code aligned with your architecture, standards, and expectations.

In practice, context engineering is implemented through three levers:

1. **Custom instructions** (global and task-specific rules)
2. **Reusable prompts** (standardized workflows)
3. **Custom agents** (task-specific personas)

These levers reduce back-and-forth prompting, increase consistency, and keep engineers in flow.

---

## 2. Scope and Outcomes

### Outcomes we want

- Higher-quality AI outputs with fewer revisions
- Consistent coding and documentation standards
- Faster onboarding and more repeatable workflows
- Clear governance for AI usage

### What this playbook is (and isn’t)

- **Is:** A structured plan for operationalizing context engineering
- **Is not:** A prompt list or generic AI tips

---

## 3. Custom Instructions

### What they are

Rules that guide Copilot’s behavior at repository scope or task scope.

### Where they live

- Repository-wide instructions: `.github/copilot-instructions.md`
- Task-specific instructions: `.github/instructions/*.instructions.md`

### What to include

- Coding conventions and naming standards
- Architecture patterns and boundaries
- Error handling and logging expectations
- Testing requirements
- Documentation and comment style

### Implementation checklist

- [ ] Create repository-wide instructions
- [ ] Add task-specific instructions for top workflows (API, tests, docs)
- [ ] Review instructions in PRs
- [ ] Update quarterly

**Template:** See [templates/custom-instructions/.github/copilot-instructions.md](templates/custom-instructions/.github/copilot-instructions.md)

---

## 4. Reusable Prompts

### What they are

Versioned prompt files that standardize common tasks and ensure consistent output.

### Where they live

- `.github/prompts/*.prompt.md`

### Examples of high-value prompts

- API controller scaffolding
- Unit test generation
- CI failure diagnosis
- Incident summary templates

### Implementation checklist

- [ ] Identify top 5 repetitive workflows
- [ ] Create prompt files for each
- [ ] Validate outputs and refine
- [ ] Publish via PR and review cycle

**Template:** See [templates/prompts/.github/prompts/api-controller.prompt.md](templates/prompts/.github/prompts/api-controller.prompt.md)

---

## 5. Custom Agents

### What they are

Specialized AI personas with a focused scope and explicit constraints. Agents encode expert judgment and produce structured outputs.

### Examples

- API design reviewer
- Security review assistant
- Documentation formatter

### Guardrails

- Agents are advisory, not autonomous
- Humans approve all changes
- Agents do not deploy, merge, or execute high-risk actions

### Implementation checklist

- [ ] Define agent scope and limits
- [ ] Specify inputs and outputs
- [ ] Add evaluation rubric
- [ ] Review in PR and publish

**Template:** See [templates/agents/agents.md](templates/agents/agents.md)

---

## 6. Rollout Plan (90 days)

### Phase 1 (Weeks 1–3): Foundation

- Publish repository-wide custom instructions
- Create 2 reusable prompts
- Train a pilot group (5–10 engineers)

### Phase 2 (Weeks 4–7): Standardization

- Add task-specific instruction files
- Expand prompt library to 5–8 prompts
- Review outputs and refine

### Phase 3 (Weeks 8–13): Specialization

- Introduce 1–2 custom agents
- Measure improvements in PR cycle time and revision count
- Establish quarterly review cadence

---

## 7. Governance

### Ownership

- **Engineering Leadership:** Policy and metrics
- **Tech Leads:** Prompt library ownership
- **Senior Engineers:** Instruction/agent reviews
- **All Engineers:** Feedback and adoption

### Non-negotiables

- No auto-merge or auto-deploy actions
- No AI-driven security policy overrides
- No autonomous production changes

---

## 8. Metrics and Validation

### Metrics to track

- Prompt reuse rate (weekly)
- AI output acceptance rate
- Average revisions per AI-generated artifact
- PR cycle time with and without Copilot context

### Validation methods

- Output review checklists
- A/B comparisons on selected workflows
- Quarterly prompt and agent audits

---

## 9. References

- GitHub Blog: “Want better AI outputs? Try context engineering”
- GitHub Copilot customization docs
- Existing foundation: [ai-engineering-playbook/README.md](../ai-engineering-playbook/README.md)

---

**Status:** Draft ready for rollout
