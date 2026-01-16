# Context Engineering Playbook (v2)

**Version:** 2.0  
**Audience:** Engineers, Tech Leads, Engineering Leadership  
**Last Updated:** January 2026

---

## Purpose

This playbook operationalizes **context engineering** for GitHub Copilot. It focuses on three practical levers highlighted in the GitHub blog post “Want better AI outputs? Try context engineering”:

1. **Custom instructions**
2. **Reusable prompts**
3. **Custom agents**

Context engineering is not about clever prompts. It is about **bringing the right information, in the right format, at the right time** so Copilot produces outputs aligned with your architecture, conventions, and standards.

## What’s Inside

| Document | Purpose |
|----------|---------|
| [PLAYBOOK.md](PLAYBOOK.md) | The full context engineering playbook and rollout plan |
| [INDEX.md](INDEX.md) | Quick navigation and role-based entry points |
| [USAGE-GUIDE.md](USAGE-GUIDE.md) | Engineer setup and workflow guide |
| templates/ | Ready-to-use templates for instructions, prompts, and agents |

## Quick Start

### Individual Contributors

1. Read the short plan in [PLAYBOOK.md](PLAYBOOK.md) (sections 2–4)
2. Use templates under templates/custom-instructions/
3. Try a reusable prompt from templates/prompts/

### Tech Leads

1. Review the rollout plan in [PLAYBOOK.md](PLAYBOOK.md) section 6
2. Choose 3–5 reusable prompts to standardize
3. Define at least one custom agent in templates/agents/agents.md

### Engineering Leadership

1. Align on metrics in [PLAYBOOK.md](PLAYBOOK.md) section 8
2. Approve governance constraints in section 5
3. Track adoption and outcomes quarterly

## Design Principles

- Context beats conversation.
- Prompts are code: versioned, reviewed, owned.
- Agents are scoped, not autonomous.
- Humans retain full accountability.

## Related Foundations

This v2 playbook complements the existing AI Engineering Playbook at [ai-engineering-playbook/README.md](../ai-engineering-playbook/README.md).

---

**Owner:** Engineering Leadership
