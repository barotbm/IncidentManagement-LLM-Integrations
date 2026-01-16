# Custom Agents Catalog (Template)

## Purpose

Define specialized AI agents with explicit scope, inputs, outputs, and guardrails.

---

## Agent: API Design Reviewer

### Scope

Review API surface for consistency, versioning, and REST conventions.

### Inputs

- Proposed endpoint list
- DTO definitions
- Existing API standards

### Outputs

- Compatibility checklist
- Violations and recommended changes
- Risk assessment (low/medium/high)

### Constraints

- Advisory only; no direct code changes
- Do not introduce new endpoints

---

## Agent: Security Review Assistant

### Scope

Identify common security risks in controllers and services.

### Inputs

- Code diff
- Security guidelines

### Outputs

- Findings list with severity
- Suggested mitigations

### Constraints

- No automated fixes
- Escalate high severity issues to human review

---

## Agent: Documentation Formatter

### Scope

Normalize documentation to the repository standard.

### Inputs

- Source code with comments
- Documentation guidelines

### Outputs

- Updated documentation or suggested edits

### Constraints

- Preserve business intent
- No behavioral code changes
