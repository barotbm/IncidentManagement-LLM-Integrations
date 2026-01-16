# Context Engineering v2 - Usage Guide

**Audience:** Engineers and Tech Leads  
**Last Updated:** January 2026

---

## 1. Overview

This guide explains how to configure a repository and GitHub Copilot for:

- **Custom instructions**
- **Reusable prompts**
- **Custom agents**

The goal is consistent, repeatable Copilot outputs that match your team’s standards.

---

## 2. Repository Setup (Required)

### 2.1 Add repository instructions

Create `.github/copilot-instructions.md` in the target repo. Use the template in:

- [ai-eng-playbook-v2/templates/custom-instructions/.github/copilot-instructions.md](templates/custom-instructions/.github/copilot-instructions.md)

**Minimum required sections:**

- Tech stack
- Coding standards
- Error handling and logging
- Testing expectations
- Constraints

### 2.2 Add task-specific instructions

Create task instruction files under `.github/instructions/`.

Example:

- [ai-eng-playbook-v2/templates/custom-instructions/.github/instructions/api-controller.instructions.md](templates/custom-instructions/.github/instructions/api-controller.instructions.md)

Recommended tasks:

- API controllers
- Service layer changes
- Tests
- Documentation

### 2.3 Add reusable prompts

Create prompt files under `.github/prompts/`.

Example prompt:

- [ai-eng-playbook-v2/templates/prompts/.github/prompts/api-controller.prompt.md](templates/prompts/.github/prompts/api-controller.prompt.md)

Prompts should be:

- Narrow in scope
- Parameterized (inputs listed clearly)
- Reviewed in PR

### 2.4 Add a custom agents catalog

Create an agents catalog file in your repo to document approved agents.

Template:

- [ai-eng-playbook-v2/templates/agents/agents.md](templates/agents/agents.md)

---

## 3. GitHub Copilot Configuration (Required)

### 3.1 Enable Copilot instructions

Ensure GitHub Copilot is enabled for the repo and that the following files exist:

- `.github/copilot-instructions.md`
- `.github/instructions/*.instructions.md`

Copilot will automatically use these instructions when present.

### 3.2 Use reusable prompts

Reusable prompts are stored in `.github/prompts/` and can be invoked from Copilot Chat by referencing the prompt file and providing required inputs.

**Example usage pattern:**

- Select the prompt from the prompt library
- Provide the required inputs from the prompt header
- Review and refine output before accepting

### 3.3 Keep prompts and instructions versioned

- All updates must go through PR review
- Assign ownership to a team or lead
- Review quarterly for drift

---

## 4. Using Custom Instructions (Engineer Workflow)

1. Confirm `.github/copilot-instructions.md` exists
2. If you’re doing a specific task (e.g., controller or tests), confirm there is a matching instruction file in `.github/instructions/`
3. In Copilot Chat, reference the task explicitly
4. Validate output against repository patterns

**Example:**

- “Generate a controller for incidents using the API controller instructions.”

---

## 5. Using Reusable Prompts (Engineer Workflow)

1. Open Copilot Chat
2. Choose the relevant prompt from `.github/prompts/`
3. Provide all required inputs
4. Review output for correctness
5. Adjust prompt content if output is inconsistent

---

## 6. Custom Agents (Detailed Guide)

Custom agents are defined for specialized tasks and are **advisory only**. They do not make changes on their own; engineers review and apply recommendations.

### 6.1 When to use agents

- API design review
- Security review
- Documentation normalization

### 6.2 How to add a custom agent (step-by-step)

1. Create or update your repo’s agents catalog at `.github/agents/agents.md`.
2. Copy the template from:
	- [ai-eng-playbook-v2/templates/agents/agents.md](templates/agents/agents.md)
3. Add a new agent section with **Scope**, **Inputs**, **Outputs**, and **Constraints**.
4. Add a **Usage** block showing the exact input format engineers must provide.
5. Submit via PR and assign a senior engineer or lead for review.

### 6.3 Example: Add an API Design Reviewer agent

**Agents catalog entry (example):**

```
## Agent: API Design Reviewer

### Scope
Review API surface for consistency, versioning, and REST conventions.

### Inputs
- Endpoint list (route, verb, description)
- DTO definitions
- Existing API standards or examples

### Outputs
- Compatibility checklist
- Violations and recommended changes
- Risk assessment (low/medium/high)

### Constraints
- Advisory only; no direct code changes
- Do not introduce new endpoints

### Usage
Provide inputs in this format:

Endpoints:
- GET /api/v2/incidents/{id} — Fetch incident by id
- POST /api/v2/incidents — Create incident

DTOs:
- IncidentCreateDto: { title, severity, description }

Standards:
- Use versioned routes under /api/v2/
- Error responses follow ProblemDetails
```

### 6.4 How to use a custom agent (engineer workflow)

1. Open Copilot Chat in your IDE.
2. Paste the agent **Usage** block and fill in required inputs.
3. Ask Copilot to act as the agent and follow the constraints.
4. Review the output and apply changes manually.

**Example usage prompt:**

```
Act as the “API Design Reviewer” agent.

Endpoints:
- GET /api/v2/incidents/{id} — Fetch incident by id
- POST /api/v2/incidents — Create incident

DTOs:
- IncidentCreateDto: { title, severity, description }

Standards:
- Use versioned routes under /api/v2/
- Error responses follow ProblemDetails

Return a checklist, violations, and a risk rating.
```

### 6.5 Guardrails

- No autonomous code changes
- No deployments or merges
- No security policy overrides

---

## 7. Minimum Adoption Checklist

- [ ] Repo has `.github/copilot-instructions.md`
- [ ] Repo has at least 2 task instruction files
- [ ] Repo has at least 3 prompt files
- [ ] Repo has a custom agents catalog
- [ ] Prompt and instruction owners are assigned

---

## 8. Troubleshooting

**Outputs don’t match conventions**

- Check instructions for missing constraints
- Add or update examples
- Make sure instructions are explicit

**Prompts aren’t being reused**

- Shorten prompt scope
- Add clear input fields
- Share a quick demo in team channel

**Agent recommendations are noisy**

- Tighten scope
- Add exclusion criteria
- Add stronger output format requirements

---

## 9. Governance and Review

- Review prompts and instructions quarterly
- Retire obsolete prompts
- Track acceptance rate and revision count

---

## 10. References

- [PLAYBOOK.md](PLAYBOOK.md)
- [INDEX.md](INDEX.md)
- [templates/](templates/)
