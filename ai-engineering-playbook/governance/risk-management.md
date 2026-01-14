# AI Risk Management

**Version:** 1.0  
**Last Updated:** January 2026  
**Owner:** Engineering Leadership + Risk Management

This document outlines risk assessment and mitigation strategies for AI usage in engineering.

---

## Risk Framework

### Risk Categories

| Category | Description | Impact if Unmanaged |
|----------|-------------|---------------------|
| **Technical Risk** | AI generates incorrect or harmful code | Production outages, data corruption |
| **Security Risk** | Data leakage, unauthorized access | Data breach, compliance violations |
| **Operational Risk** | Over-reliance on AI, skill atrophy | Team unable to function without AI |
| **Compliance Risk** | Regulatory violations | Fines, legal liability |
| **Reputational Risk** | Customer trust erosion | Customer churn, brand damage |
| **Financial Risk** | Cost overruns, wasted investment | Budget bloat, ROI failure |

---

## Risk Assessment Matrix

| Risk | Likelihood | Impact | Severity | Mitigation Priority |
|------|-----------|--------|----------|---------------------|
| **AI generates code with security vulnerability** | High | High | 🔴 Critical | Immediate |
| **Customer PII leaked to AI tool** | Medium | Critical | 🔴 Critical | Immediate |
| **Over-reliance reduces engineering capability** | Medium | High | 🟡 High | Short-term |
| **AI tool vendor data breach** | Low | Critical | 🟡 High | Short-term |
| **False positive from agent skill blocks workflow** | High | Low | 🟢 Medium | Medium-term |
| **Cost exceeds budget** | Low | Medium | 🟢 Medium | Medium-term |
| **AI-generated code is biased or unfair** | Low | Medium | 🟢 Medium | Medium-term |

---

## Detailed Risk Analysis

### 1. Technical Risks

#### 1.1 Incorrect Code Generation

**Description:** AI generates syntactically valid but logically incorrect code.

**Examples:**
- Off-by-one errors
- Incorrect algorithm implementation
- Race conditions in concurrent code
- Memory leaks

**Likelihood:** High (AI is not perfect)

**Impact:** High (production bugs, customer impact)

**Mitigation:**
- ✅ Mandatory code review for all AI-generated code
- ✅ Comprehensive test suite (unit + integration)
- ✅ Static analysis and linting
- ✅ Gradual rollout (canary deployments)

**Residual Risk:** Medium (tests and reviews catch most issues, but not all)

#### 1.2 Performance Issues

**Description:** AI-generated code is inefficient (slow, memory-intensive).

**Examples:**
- N+1 database queries
- Blocking I/O in async contexts
- Excessive memory allocations

**Likelihood:** Medium

**Impact:** High (poor user experience, infrastructure costs)

**Mitigation:**
- ✅ Performance testing in CI/CD
- ✅ Code review for performance anti-patterns
- ✅ Profiling in staging before prod deployment

**Residual Risk:** Low

#### 1.3 Dependency Vulnerabilities

**Description:** AI suggests importing vulnerable packages.

**Examples:**
- Outdated libraries with known CVEs
- Malicious packages (typosquatting)

**Likelihood:** Medium

**Impact:** Critical (security breach)

**Mitigation:**
- ✅ Dependency scanning (Snyk, Dependabot)
- ✅ Approved package whitelist
- ✅ Automated vulnerability alerts

**Residual Risk:** Low

---

### 2. Security Risks

#### 2.1 Data Exfiltration

**Description:** Sensitive data sent to AI tools and retained by vendor.

**Likelihood:** Medium (human error)

**Impact:** Critical (GDPR violations, customer trust loss)

**Mitigation:**
- ✅ Approved AI tools only (zero data retention)
- ✅ Automated PII scanning in prompts
- ✅ Training on data handling policies
- ✅ Audit logs reviewed quarterly

**Residual Risk:** Low (with technical controls)

#### 2.2 Credential Leakage

**Description:** API keys or passwords embedded in AI-generated code.

**Likelihood:** Low (AI tools trained to avoid this)

**Impact:** Critical (unauthorized access)

**Mitigation:**
- ✅ Secret scanning in CI/CD
- ✅ Vault for credential management
- ✅ Regular credential rotation

**Residual Risk:** Very Low

#### 2.3 Malicious Code Injection

**Description:** AI tricked into generating harmful code via prompt injection.

**Likelihood:** Low (requires sophisticated attack)

**Impact:** High (production compromise)

**Mitigation:**
- ✅ Input sanitization
- ✅ Code review catches anomalies
- ✅ Static analysis detects patterns

**Residual Risk:** Very Low

---

### 3. Operational Risks

#### 3.1 Over-Reliance on AI

**Description:** Engineers lose ability to code without AI assistance.

**Likelihood:** Medium (especially juniors)

**Impact:** High (team cannot function if AI unavailable)

**Mitigation:**
- ✅ "AI-free Fridays" (optional, for skill maintenance)
- ✅ Training on fundamentals, not just AI usage
- ✅ Code review requires understanding, not just accepting AI output

**Residual Risk:** Medium (cultural shift needed)

#### 3.2 AI Tool Outage

**Description:** Primary AI tool (e.g., Copilot) becomes unavailable.

**Likelihood:** Low (high SLA from vendors)

**Impact:** Medium (productivity drop)

**Mitigation:**
- ✅ Multiple approved AI tools (fallback options)
- ✅ Critical workflows can proceed manually
- ✅ Incident response plan for AI outages

**Residual Risk:** Low

#### 3.3 Skill Divergence

**Description:** Wide gap emerges between AI power users and non-users.

**Likelihood:** High (natural adoption curve)

**Impact:** Medium (team cohesion, hiring challenges)

**Mitigation:**
- ✅ Mandatory AI training for all engineers
- ✅ Peer mentoring programs
- ✅ Standardized prompts reduce skill gap

**Residual Risk:** Medium

---

### 4. Compliance Risks

#### 4.1 Regulatory Violations

**Description:** AI usage violates GDPR, CCPA, SOC 2, etc.

**Likelihood:** Low (with proper controls)

**Impact:** Critical (fines, legal action)

**Mitigation:**
- ✅ Legal review of AI tool contracts
- ✅ Data handling policies enforced
- ✅ Regular compliance audits

**Residual Risk:** Very Low

#### 4.2 Audit Failures

**Description:** Insufficient audit trail for AI-generated code.

**Likelihood:** Low

**Impact:** High (SOC 2 non-compliance)

**Mitigation:**
- ✅ All AI usage logged (who, what, when)
- ✅ Git commit messages tag AI-assisted code
- ✅ Retention policies meet regulatory requirements

**Residual Risk:** Very Low

---

### 5. Reputational Risks

#### 5.1 Customer Trust Erosion

**Description:** Customers lose trust if they learn code is AI-generated.

**Likelihood:** Low (transparency mitigates)

**Impact:** High (customer churn)

**Mitigation:**
- ✅ Emphasize that AI is tool, not replacement for engineers
- ✅ Communicate quality standards remain unchanged
- ✅ Showcase productivity gains benefiting customers

**Residual Risk:** Low

#### 5.2 Public Relations Issues

**Description:** Negative media coverage of AI usage (e.g., "AI writes your code").

**Likelihood:** Low

**Impact:** Medium (brand perception)

**Mitigation:**
- ✅ Proactive communication strategy
- ✅ Highlight human oversight and review
- ✅ Position as engineering excellence, not cost-cutting

**Residual Risk:** Low

---

### 6. Financial Risks

#### 6.1 Cost Overruns

**Description:** AI tool licensing costs exceed budget.

**Likelihood:** Medium (adoption faster than expected)

**Impact:** Medium (budget reallocation needed)

**Mitigation:**
- ✅ Usage monitoring and forecasting
- ✅ Tiered licensing (not all engineers need premium features)
- ✅ Quarterly cost reviews

**Residual Risk:** Low

#### 6.2 Poor ROI

**Description:** Productivity gains don't justify AI investment.

**Likelihood:** Low (early data shows strong ROI)

**Impact:** High (wasted budget, credibility loss)

**Mitigation:**
- ✅ Measure productivity metrics (see metrics.md)
- ✅ Quarterly ROI analysis
- ✅ Adjust strategy based on data

**Residual Risk:** Very Low

---

## Risk Mitigation Strategies

### Preventive Controls

**Before AI adoption:**
- Establish guardrails and policies
- Train engineers on secure AI usage
- Select approved tools with strong data protection

**During AI usage:**
- Code review for all AI-generated code
- Automated scanning (security, quality, compliance)
- Monitoring and alerting

**After deployment:**
- Incident response procedures
- Postmortem analysis
- Continuous improvement

### Detective Controls

- Audit logs of AI tool usage
- Metrics dashboards (acceptance rates, defect rates)
- Quarterly security reviews
- Anomaly detection (unusual prompts or outputs)

### Corrective Controls

- Incident response team
- Rollback procedures
- Revocation of access for policy violations
- Remediation plans for vulnerabilities

---

## Risk Ownership

| Risk Category | Owner | Review Frequency |
|---------------|-------|------------------|
| **Technical** | Tech Leads | Monthly |
| **Security** | CISO | Quarterly |
| **Operational** | Engineering Manager | Monthly |
| **Compliance** | Legal + Compliance | Quarterly |
| **Reputational** | CTO + Marketing | Annually |
| **Financial** | CFO + Engineering Leadership | Quarterly |

---

## Risk Appetite

### Acceptable Risk Levels

| Risk Type | Acceptable | Unacceptable |
|-----------|-----------|--------------|
| **Security** | Low | Medium or High |
| **Compliance** | Very Low | Low or above |
| **Operational** | Medium | High |
| **Financial** | Medium | High |
| **Reputational** | Low | Medium or High |

**If residual risk exceeds acceptable level, escalate to CTO.**

---

## Continuous Risk Assessment

### Monthly

- Review new risks from AI usage
- Update mitigation strategies
- Track metrics (incidents, near-misses)

### Quarterly

- Formal risk assessment with leadership
- Update risk register
- Adjust policies if needed

### Annually

- Comprehensive risk audit
- Third-party penetration testing
- Benchmark against industry standards

---

## Escalation Path

### Risk Identification

1. Engineer identifies potential risk → Report to Tech Lead
2. Tech Lead assesses severity → Escalate if Medium or above
3. Engineering Leadership reviews → Implement mitigation or accept risk

### Risk Realization (Incident)

1. Incident detected → Security/Ops team engaged
2. Immediate containment → Stop the bleeding
3. Root cause analysis → Understand what happened
4. Remediation → Fix the issue
5. Prevention → Update policies/controls
6. Communication → Notify stakeholders if required

---

## Key Metrics

Track these risk indicators:

| Metric | Target | Alert Threshold |
|--------|--------|-----------------|
| **AI-related incidents** | 0 per quarter | 1+ per quarter |
| **Defect rate (AI code)** | ≤ human code | >1.5x human code |
| **Security vulnerabilities (AI code)** | ≤ human code | >1.5x human code |
| **Policy violations** | 0 per quarter | 1+ per quarter |
| **Over-reliance score** (survey) | <30% | >50% |

See [metrics.md](metrics.md) for measurement details.

---

## Summary

**Risk management is ongoing, not one-time.**

- Assess risks before adoption
- Monitor risks during usage
- Adjust controls based on outcomes
- Escalate when thresholds exceeded

**AI is powerful, but not risk-free. Manage it accordingly.**

---

*For related policies, see [guardrails.md](guardrails.md) and [security.md](security.md).*
