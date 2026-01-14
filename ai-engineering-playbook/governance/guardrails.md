# AI Guardrails

**Version:** 1.0  
**Last Updated:** January 2026  
**Owner:** CTO + Engineering Leadership

These guardrails are **non-negotiable**. Violations are escalated to Engineering Leadership immediately.

---

## Hard Constraints

AI must NEVER autonomously:

### 1. Auto-Merge Pull Requests

**Prohibited:** Any system that automatically merges PRs without human approval.

**Why:** Code quality, security, and accountability require human review. Even if all automated checks pass, a human must make the final decision.

**Enforcement:**
- GitHub branch protection: Require 1+ human approvals
- CI/CD checks can block merge, but not approve
- Violations flagged in security audits

**Allowed:**
- AI suggests PR is ready for review
- AI posts quality analysis as a comment
- AI sets labels (e.g., "ai-reviewed", "needs-work")

---

### 2. Auto-Deploy to Production

**Prohibited:** Automated deployments to production triggered by AI without human approval.

**Why:** Production deployments carry business risk. Deployment timing, customer communication, and rollback plans require human judgment.

**Enforcement:**
- Production deployment pipeline requires manual approval step
- Approval limited to Tech Leads and above
- Deployment audit log reviewed monthly

**Allowed:**
- AI deploys to dev/test environments automatically
- AI recommends deployment windows based on traffic patterns
- AI pre-validates deployment artifacts

---

### 3. Mutate Production Systems

**Prohibited:** Any change to production infrastructure, databases, or configuration without human approval.

**Why:** Unplanned changes can cause outages, data loss, or security breaches.

**Examples of prohibited actions:**
- Scaling Kubernetes pods
- Modifying database schemas
- Changing firewall rules
- Rotating credentials
- Deleting resources

**Enforcement:**
- Production infrastructure protected by RBAC
- AI service accounts have read-only access
- Change management process enforced via ticketing

**Allowed:**
- AI monitors production and alerts on anomalies
- AI suggests infrastructure optimizations
- AI generates runbooks for human execution

---

### 4. Override Security Policies

**Prohibited:** AI bypassing security controls, access policies, or compliance requirements.

**Examples:**
- Granting elevated permissions
- Disabling security scanning
- Whitelisting vulnerabilities without review
- Skipping audit logging

**Enforcement:**
- Security policies enforced at infrastructure layer
- AI tools cannot modify IAM or access control
- Security team reviews AI-generated code for policy violations

**Allowed:**
- AI flags security issues in code
- AI suggests secure coding patterns
- AI assists with vulnerability remediation

---

### 5. Make Breaking API Changes

**Prohibited:** Publishing API changes that break backward compatibility without review.

**Why:** Breaking changes impact customers and partners. Coordination and communication are required.

**Enforcement:**
- API versioning strategy enforced
- Breaking changes require architect approval
- API contract tests run in CI/CD

**Allowed:**
- AI detects breaking changes and flags them
- AI suggests backward-compatible alternatives
- AI generates migration guides for reviewers

---

### 6. Delete Data or Resources

**Prohibited:** Deleting production data, backups, or critical resources.

**Why:** Data loss is irreversible and can have legal/compliance consequences.

**Examples:**
- Dropping database tables
- Deleting S3 buckets
- Removing DNS records
- Purging backups

**Enforcement:**
- Production delete operations require MFA + secondary approval
- AI service accounts lack delete permissions
- Soft deletes preferred over hard deletes

**Allowed:**
- AI identifies orphaned resources for cleanup
- AI suggests retention policy optimizations
- AI generates cleanup scripts for human execution

---

### 7. Escalate Incidents

**Prohibited:** AI autonomously paging on-call engineers or declaring incidents.

**Why:** Incident severity and escalation require business judgment. False alarms erode trust.

**Enforcement:**
- Paging systems require human initiation
- AI alerts routed to monitoring channels, not pager
- Incident Commander role always held by human

**Allowed:**
- AI detects anomalies and posts to Slack
- AI correlates metrics and logs to surface likely root causes
- AI drafts incident timelines for human review

---

## Approval Requirements

Certain AI-assisted actions require explicit human approval:

| Action | Approval Required | Rationale |
|--------|-------------------|-----------|
| **Merge PR with AI-generated code** | Code reviewer | Accountability |
| **Deploy to staging** | Engineer | Testing validation |
| **Deploy to production** | Tech Lead | Business risk |
| **Infrastructure changes** | Platform engineer | Stability |
| **Security-sensitive code** | Security engineer | Compliance |
| **Database migrations** | DBA | Data integrity |
| **API contract changes** | Architect | Customer impact |

---

## Compliance and Audit

### Audit Logging

All AI tool usage must be logged:
- Who invoked the AI tool
- What inputs were provided
- What outputs were generated
- Whether output was accepted or rejected

**Retention:** 1 year minimum, 3 years for regulated workloads.

### Quarterly Review

Engineering Leadership reviews:
- AI usage trends (adoption, acceptance rates)
- Guardrail violations (if any)
- Security incidents involving AI tools
- Effectiveness of controls

### Violation Response

| Severity | Response | Example |
|----------|----------|---------|
| **Critical** | Immediate revocation of access, incident review | Auto-deploying to production |
| **High** | Mandatory retraining, manager notification | Pasting customer PII into ChatGPT |
| **Medium** | Warning, documentation review | Attempting to auto-merge PR |
| **Low** | Coaching, reminder of policies | Using unapproved AI tool |

---

## Exceptions

In rare cases, guardrails may be temporarily relaxed:

**Process:**
1. Engineer submits exception request with business justification
2. CTO and Security team review
3. If approved, exception is time-bound (e.g., 30 days)
4. Extra monitoring/logging enabled during exception period
5. Exception reviewed and either revoked or made permanent

**Example:** AI-assisted deployment to staging for a high-priority customer demo, with extra validation steps.

---

## Enforcement Mechanisms

### Technical Controls

- **RBAC:** AI service accounts have limited permissions
- **Branch Protection:** PRs require human approval
- **Deployment Gates:** Manual approval required for production
- **Audit Logs:** All AI actions logged and retained

### Process Controls

- **Code Review:** AI-generated code reviewed like human code
- **Security Review:** Security team spot-checks AI usage
- **Quarterly Audits:** Compliance team reviews audit logs
- **Training:** All engineers trained on guardrails

### Cultural Controls

- **Accountability:** Engineers own their code, AI-generated or not
- **Transparency:** AI usage tagged in commits and PRs
- **Feedback:** Violations discussed in retrospectives, not punished punitively

---

## Summary

**AI is a tool, not a decision-maker.**

These guardrails ensure AI accelerates engineering without introducing unacceptable risk.

**If in doubt, ask yourself:**
- Would I do this without review if I wrote the code manually?
- What's the worst case if this goes wrong?
- Can I explain this decision to the CTO?

**If the answer is "no" to any of these, stop and get approval.**

---

*For related policies, see [security.md](security.md) and [risk-management.md](risk-management.md).*
