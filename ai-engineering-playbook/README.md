# AI Engineering Playbook

**Version:** 1.0  
**Last Updated:** January 2026  
**Owner:** Engineering Leadership

## Purpose

This repository operationalizes AI across the engineering lifecycle. It's not a collection of tips or best practices—it's infrastructure.

Engineers already have access to AI tools. This playbook shows how to move from **chatting with AI** to **configuring and integrating AI into repeatable workflows**.

## What's Inside

| Document | Purpose |
|----------|---------|
| [PLAYBOOK.md](PLAYBOOK.md) | Complete AI Engineering Playbook |
| [principles.md](principles.md) | Core principles and constraints |
| [maturity-model.md](maturity-model.md) | AI usage maturity progression |
| [governance/](governance/) | Security, risk management, and policies |
| [prompts/](prompts/) | Version-controlled prompt templates |
| [agent-skills/](agent-skills/) | Reusable agent skill definitions |
| [examples/](examples/) | Real-world implementation examples |

## Quick Start

### For Individual Contributors

1. Review the [maturity model](maturity-model.md) to understand where you are today
2. Browse [prompts/dev/](prompts/dev/) for immediate productivity gains
3. Apply [agent-skills/pr-readiness/](agent-skills/pr-readiness/) before your next PR

### For Tech Leads

1. Read the full [PLAYBOOK.md](PLAYBOOK.md)
2. Review [governance/guardrails.md](governance/guardrails.md) for organizational constraints
3. Define team-specific prompts and skills in your project repositories

### For Engineering Leadership

1. Understand the [maturity model](maturity-model.md) to assess team capabilities
2. Review [governance/](governance/) for risk management and compliance
3. Use [governance/metrics.md](governance/metrics.md) to measure adoption and impact

## Philosophy

**AI is infrastructure, not a chat tool.**

- Prompts are code. They should be versioned, reviewed, and tested.
- Agent skills encode engineering intent. They should be owned by senior engineers.
- Configuration beats conversation. Repeatability beats one-off interactions.
- Humans retain full ownership of design, business logic, security, and production decisions.

## Repository Structure

```
ai-engineering-playbook/
├── README.md                      # This file
├── PLAYBOOK.md                    # Complete playbook
├── principles.md                  # Core principles
├── maturity-model.md              # Maturity progression
├── governance/
│   ├── guardrails.md             # Hard constraints
│   ├── security.md               # Security requirements
│   ├── risk-management.md        # Risk assessment
│   └── metrics.md                # Productivity measurement
├── prompts/
│   ├── dev/                      # Development prompts
│   ├── testing/                  # Test generation
│   ├── cicd/                     # CI/CD diagnostics
│   ├── containers/               # Dockerfile and containerization
│   ├── k8s/                      # Kubernetes manifests
│   ├── observability/            # Logging and monitoring
│   └── incidents/                # Incident response
├── agent-skills/
│   ├── pr-readiness/             # PR quality checks
│   ├── test-coverage/            # Test coverage analysis
│   ├── quality-gates/            # Code quality validation
│   └── architecture-review/      # Architectural conformance
└── examples/
    ├── api-controller/           # Create a new REST API
    ├── integration-test/         # Generate integration tests
    ├── ci-failure-diagnosis/     # Debug CI pipeline
    └── incident-summary/         # Summarize production incident
```

## Core Constraints

AI must NEVER:
- Auto-merge pull requests
- Auto-deploy to production
- Mutate production systems without human approval
- Make architectural decisions autonomously
- Override security policies

AI SHOULD:
- Generate code for human review
- Suggest tests and validation strategies
- Diagnose failures and suggest fixes
- Automate repetitive analysis tasks
- Surface patterns and anomalies

## Usage Maturity Model (Summary)

| Level | Behavior | Repeatability | Impact |
|-------|----------|---------------|--------|
| **1. Chatting** | Ad-hoc questions | None | Limited |
| **2. Prompting** | Refined prompts | Low | Moderate |
| **3. Configuring** | Saved configurations | Medium | High |
| **4. Integrating** | Workflow automation | High | Very High |
| **5. Operating** | Continuous optimization | Continuous | Maximum |

See [maturity-model.md](maturity-model.md) for detailed progression.

## Contributing

Prompts and agent skills are first-class code artifacts:

1. Create a feature branch
2. Add or modify prompts/skills
3. Submit PR with clear justification
4. Require review from senior engineer or tech lead
5. Merge after approval

## Ownership

- **Engineering Leadership**: Overall playbook governance
- **Staff/Principal Engineers**: Agent skill definitions
- **Tech Leads**: Team-specific prompt libraries
- **Senior Engineers**: Prompt review and validation
- **All Engineers**: Usage and feedback

## Support

- **Questions**: #ai-engineering Slack channel
- **Issues**: File an issue in this repository
- **Improvements**: Submit a PR with proposed changes

## License

Internal use only. Not for redistribution.
