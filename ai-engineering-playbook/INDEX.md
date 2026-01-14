# AI Engineering Playbook - Complete Index

**Quick navigation to all playbook resources**

---

## Core Documents

| Document | Purpose | Audience | Read Time |
|----------|---------|----------|-----------|
| [README.md](README.md) | Main entry point, overview, quick start | Everyone | 5 min |
| [PLAYBOOK.md](PLAYBOOK.md) | Complete playbook (15 sections) | All engineers, leadership | 45 min |
| [principles.md](principles.md) | 10 core principles | Everyone | 15 min |
| [maturity-model.md](maturity-model.md) | 5-level progression model | Everyone | 20 min |
| [IMPLEMENTATION-SUMMARY.md](IMPLEMENTATION-SUMMARY.md) | What was created, how to use it | New users | 10 min |

---

## Governance & Policy

| Document | Purpose | Owner | Enforcement |
|----------|---------|-------|-------------|
| [governance/guardrails.md](governance/guardrails.md) | 7 prohibited AI actions | CTO + Eng Leadership | Automated + Manual |
| [governance/security.md](governance/security.md) | Data handling, approved tools | CISO | Automated scanning |
| [governance/risk-management.md](governance/risk-management.md) | Risk assessment framework | Eng Leadership + Risk | Quarterly review |
| [governance/metrics.md](governance/metrics.md) | Productivity measurement | Eng Leadership | Continuous tracking |

---

## Prompt Templates

### Development

| Prompt | Use Case | Maturity Level | Output |
|--------|----------|----------------|--------|
| [prompts/dev/api-controller-template.md](prompts/dev/api-controller-template.md) | Generate REST API controllers | 2-3 | Controller + DTOs |

### Testing

| Prompt | Use Case | Maturity Level | Output |
|--------|----------|----------------|--------|
| [prompts/testing/unit-test-template.md](prompts/testing/unit-test-template.md) | Generate xUnit tests | 2-3 | Test class with 10+ tests |

### CI/CD

| Prompt | Use Case | Maturity Level | Output |
|--------|----------|----------------|--------|
| [prompts/cicd/diagnose-build-failure.md](prompts/cicd/diagnose-build-failure.md) | Diagnose CI/CD failures | 3-4 | Root cause + fix suggestions |

### Containers

| Prompt | Use Case | Maturity Level | Output |
|--------|----------|----------------|--------|
| [prompts/containers/dockerfile-template.md](prompts/containers/dockerfile-template.md) | Generate production Dockerfiles | 3 | Dockerfile + .dockerignore |

### Incidents

| Prompt | Use Case | Maturity Level | Output |
|--------|----------|----------------|--------|
| [prompts/incidents/incident-summary.md](prompts/incidents/incident-summary.md) | Summarize production incidents | 4 | Structured postmortem |

---

## Agent Skills

| Agent Skill | Purpose | Maturity Level | Integration |
|-------------|---------|----------------|-------------|
| [agent-skills/pr-readiness/](agent-skills/pr-readiness/README.md) | Validate PR quality before review | 4 | GitHub Actions |
| [agent-skills/test-coverage/](agent-skills/test-coverage/README.md) | Analyze coverage gaps, suggest tests | 4 | CI/CD pipeline |

---

## Examples

| Example | Scenario | Maturity Level | Time Saved |
|---------|----------|----------------|------------|
| [examples/api-controller/](examples/api-controller/README.md) | Create new REST API | 3 | 30 min (67%) |
| [examples/ci-failure-diagnosis/](examples/ci-failure-diagnosis/README.md) | Diagnose CI failure | 4 | 23-38 min (84%) |

---

## By Role

### Individual Contributor

**Start here:**
1. [maturity-model.md](maturity-model.md) - Assess your level
2. [examples/api-controller/](examples/api-controller/README.md) - See AI in action
3. [prompts/dev/api-controller-template.md](prompts/dev/api-controller-template.md) - Try a prompt
4. [prompts/testing/unit-test-template.md](prompts/testing/unit-test-template.md) - Generate tests

**Goal:** Level 2-3 within 90 days

### Senior Engineer

**Start here:**
1. [PLAYBOOK.md](PLAYBOOK.md) - Full context
2. [principles.md](principles.md) - Core constraints
3. [agent-skills/pr-readiness/](agent-skills/pr-readiness/README.md) - Define team skills
4. Create team-specific prompts

**Goal:** Level 3-4 within 6 months

### Tech Lead

**Start here:**
1. [PLAYBOOK.md](PLAYBOOK.md) - Strategy overview
2. [governance/guardrails.md](governance/guardrails.md) - Know boundaries
3. [governance/metrics.md](governance/metrics.md) - Track team progress
4. Deploy agent skills to CI/CD

**Goal:** Lead team to Level 4 within 12 months

### Engineering Leadership

**Start here:**
1. [maturity-model.md](maturity-model.md) - Organizational assessment
2. [governance/](governance/) - Risk, security, compliance
3. [governance/metrics.md](governance/metrics.md) - ROI and impact
4. [PLAYBOOK.md](PLAYBOOK.md) sections 13-14 - Metrics and rollout

**Goal:** Organization-wide Level 4-5 transformation

---

## By Use Case

### "I need to create a new API"

1. [prompts/dev/api-controller-template.md](prompts/dev/api-controller-template.md)
2. [examples/api-controller/](examples/api-controller/README.md)

### "I need to write tests"

1. [prompts/testing/unit-test-template.md](prompts/testing/unit-test-template.md)
2. [agent-skills/test-coverage/](agent-skills/test-coverage/README.md)

### "My CI build is failing"

1. [prompts/cicd/diagnose-build-failure.md](prompts/cicd/diagnose-build-failure.md)
2. [examples/ci-failure-diagnosis/](examples/ci-failure-diagnosis/README.md)

### "I need to containerize my app"

1. [prompts/containers/dockerfile-template.md](prompts/containers/dockerfile-template.md)

### "I need to document an incident"

1. [prompts/incidents/incident-summary.md](prompts/incidents/incident-summary.md)

### "I want to improve PR quality"

1. [agent-skills/pr-readiness/](agent-skills/pr-readiness/README.md)

### "I want to understand security policies"

1. [governance/security.md](governance/security.md)
2. [governance/guardrails.md](governance/guardrails.md)

### "I want to measure AI impact"

1. [governance/metrics.md](governance/metrics.md)

---

## By Maturity Level

### Level 1 → Level 2 (Chatting → Prompting)

**Resources:**
- [PLAYBOOK.md](PLAYBOOK.md) section 5 (Prompting skill)
- [prompts/dev/api-controller-template.md](prompts/dev/api-controller-template.md)

**Time:** 1-2 weeks of practice

### Level 2 → Level 3 (Prompting → Configuring)

**Resources:**
- [principles.md](principles.md) (principle 4: Configuration beats conversation)
- Create team prompt library in your repo
- [examples/api-controller/](examples/api-controller/README.md)

**Time:** 1-2 months with team collaboration

### Level 3 → Level 4 (Configuring → Integrating)

**Resources:**
- [agent-skills/pr-readiness/](agent-skills/pr-readiness/README.md)
- [agent-skills/test-coverage/](agent-skills/test-coverage/README.md)
- [examples/ci-failure-diagnosis/](examples/ci-failure-diagnosis/README.md)

**Time:** 3-6 months with CI/CD integration

### Level 4 → Level 5 (Integrating → Operating)

**Resources:**
- [governance/metrics.md](governance/metrics.md)
- [PLAYBOOK.md](PLAYBOOK.md) section 13 (Measuring productivity)
- Implement feedback loops

**Time:** 6-12 months with dedicated platform investment

---

## Roadmap

### Month 1-3 (Foundation)

**Read:**
- [PLAYBOOK.md](PLAYBOOK.md)
- [principles.md](principles.md)
- [maturity-model.md](maturity-model.md)

**Do:**
- Try 2-3 prompt templates
- Assess current maturity level
- Create first team-specific prompt

### Month 4-6 (Integration)

**Read:**
- [governance/guardrails.md](governance/guardrails.md)
- [agent-skills/pr-readiness/](agent-skills/pr-readiness/README.md)

**Do:**
- Deploy PR readiness skill
- Create 10+ prompt templates
- Measure baseline metrics

### Month 7-12 (Optimization)

**Read:**
- [governance/metrics.md](governance/metrics.md)
- [governance/risk-management.md](governance/risk-management.md)

**Do:**
- Implement feedback loops
- Tune agent skills
- Achieve 80%+ adoption

### Month 13+ (Operating)

**Read:**
- Quarterly reviews of all governance docs

**Do:**
- Continuous improvement
- Share learnings across org
- Expand to new use cases

---

## Quick Reference

### Common Questions

| Question | Answer |
|----------|--------|
| Where do I start? | [README.md](README.md) → [examples/api-controller/](examples/api-controller/README.md) |
| What's my maturity level? | [maturity-model.md](maturity-model.md) section: Self-Assessment |
| What can AI NOT do? | [governance/guardrails.md](governance/guardrails.md) section: Hard Constraints |
| How do I measure impact? | [governance/metrics.md](governance/metrics.md) |
| What data can I send to AI? | [governance/security.md](governance/security.md) section: Data Handling |
| How do I create a prompt? | [PLAYBOOK.md](PLAYBOOK.md) section 5: Prompting Skill |
| How do I deploy an agent skill? | [agent-skills/pr-readiness/](agent-skills/pr-readiness/README.md) section: Integration |

### Key Takeaways

From [PLAYBOOK.md](PLAYBOOK.md) section 15:

1. AI is infrastructure, not a chat tool
2. Prompts are code (version, review, own)
3. Agent skills encode engineering intent
4. Humans retain full ownership
5. Guardrails are non-negotiable
6. Prompting is a first-class skill
7. Configuration beats conversation
8. Measure everything
9. The gap will widen (operators vs. casual users)
10. Start small, scale fast

---

## File Count & Size

**Statistics:**
- Total files: 17
- Total words: ~40,000
- Total size: ~350 KB (text)
- Code examples: 15+
- Prompt templates: 5
- Agent skills: 2
- Examples: 2

---

## Contributing

To add or improve content:

1. File an issue describing the need
2. Create a feature branch
3. Add/modify files following existing patterns
4. Submit PR with clear description
5. Require review from senior engineer
6. Merge after approval

See [README.md](README.md) section: Contributing

---

## Support

- **Questions:** #ai-engineering Slack channel
- **Issues:** File in this repository
- **Documentation:** This playbook
- **Owner:** Engineering Leadership

---

**Last Updated:** January 13, 2026  
**Version:** 1.0  
**Status:** Production-Ready

---

**Ready to start?** Pick your role above and follow the recommended path.
