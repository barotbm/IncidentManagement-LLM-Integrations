# Incident Summary Template

**Category:** Incident Response  
**Use Case:** Summarize production incidents from logs and alerts  
**Maturity Level:** 4  
**Last Updated:** January 2026

---

## Purpose

Generate structured incident summaries for postmortems and stakeholder communication.

---

## Prompt Template

```markdown
Analyze this production incident and generate a structured summary for postmortem documentation.

## Incident Context

**Incident ID:** {INCIDENT_ID}  
**Severity:** {SEV1 | SEV2 | SEV3}  
**Start Time:** {TIMESTAMP}  
**End Time:** {TIMESTAMP or "Ongoing"}  
**Detected By:** {Monitoring Alert | Customer Report | Engineer}  
**Services Affected:** {SERVICE_NAMES}

## Incident Data

### Alerts Triggered

```
{PASTE_ALERT_MESSAGES}
```

### Application Logs

```
{PASTE_RELEVANT_LOG_LINES}
```

### Metrics/Monitoring

- **Error Rate:** {BEFORE} → {DURING_INCIDENT}
- **Response Time:** {BEFORE} → {DURING_INCIDENT}
- **Traffic:** {BEFORE} → {DURING_INCIDENT}

### Actions Taken

```
{TIMELINE_OF_ACTIONS}
Example:
14:23 - Incident detected via PagerDuty
14:25 - Incident Commander assigned
14:30 - Identified high error rate in Orders API
14:35 - Rolled back deployment v2.5.1 → v2.5.0
14:40 - Error rate returned to normal
14:45 - Monitoring confirmed resolution
```

### Related Changes

```
{RECENT_DEPLOYMENTS, CONFIG_CHANGES, INFRASTRUCTURE_CHANGES}
```

## Analysis Required

### 1. Incident Timeline

Create a chronological timeline:
- When incident started (first anomaly)
- When incident was detected
- When response began
- Key investigation steps
- When mitigation was applied
- When incident was resolved

### 2. Root Cause Analysis

Identify:
- **Immediate cause:** What directly caused the failure?
- **Contributing factors:** What made it possible/worse?
- **Root cause:** Why did it happen? (5 whys analysis)

### 3. Impact Assessment

Quantify:
- **Duration:** Total time from start to resolution
- **Customer impact:** Number of affected users/requests
- **Business impact:** Revenue loss, SLA breach, reputation
- **Data impact:** Any data loss or corruption

### 4. What Went Well

Identify positive aspects:
- Fast detection
- Effective communication
- Quick mitigation
- Good collaboration

### 5. What Went Wrong

Identify gaps:
- Delayed detection
- Unclear runbooks
- Communication breakdowns
- Insufficient monitoring

### 6. Action Items

Generate specific, actionable follow-ups:
- Short-term fixes (stop the bleeding)
- Medium-term improvements (prevent recurrence)
- Long-term investments (systemic improvements)

Each action item should have:
- Clear description
- Owner assignment
- Target completion date
- Priority (P0 = critical, P1 = high, P2 = medium)

## Output Format

Provide analysis in this structure:

---

# Incident Postmortem: {INCIDENT_ID}

**Date:** {DATE}  
**Severity:** {SEV_LEVEL}  
**Duration:** {DURATION}  
**Status:** Resolved  

## Executive Summary

{2-3 sentence summary of what happened, impact, and resolution}

## Impact

- **Users Affected:** {NUMBER or PERCENTAGE}
- **Duration:** {TIME}
- **Services Down:** {SERVICE_LIST}
- **Customer-Facing Impact:** {DESCRIPTION}
- **SLA Breach:** {YES/NO, with details}

## Timeline

| Time (UTC) | Event |
|------------|-------|
| {TIME} | {EVENT_DESCRIPTION} |
| {TIME} | {EVENT_DESCRIPTION} |
| ... | ... |

## Root Cause

### Immediate Cause
{What directly caused the failure}

### Contributing Factors
1. {FACTOR_1}
2. {FACTOR_2}

### Root Cause (5 Whys)
1. Why did X happen? → {ANSWER}
2. Why did that happen? → {ANSWER}
3. Why did that happen? → {ANSWER}
4. Why did that happen? → {ANSWER}
5. Why did that happen? → {ANSWER} (ROOT CAUSE)

## What Went Well

- ✅ {POSITIVE_ASPECT_1}
- ✅ {POSITIVE_ASPECT_2}

## What Went Wrong

- ❌ {GAP_1}
- ❌ {GAP_2}

## Action Items

### Immediate (within 24 hours)

| Action | Owner | Status |
|--------|-------|--------|
| {ACTION} | @{OWNER} | {TODO/DONE} |

### Short-term (within 1 week)

| Action | Owner | Target Date |
|--------|-------|-------------|
| {ACTION} | @{OWNER} | {DATE} |

### Long-term (within 1 month+)

| Action | Owner | Target Date |
|--------|-------|-------------|
| {ACTION} | @{OWNER} | {DATE} |

## Lessons Learned

1. {LESSON_1}
2. {LESSON_2}

---

## Context

**Incident Response Framework:** {COMPANY_FRAMEWORK}  
**Postmortem Template:** Blameless, data-driven  
**Audience:** Engineering team, leadership, stakeholders  
**Tone:** Factual, objective, solution-oriented

## Constraints

- Use blameless language (no finger-pointing)
- Focus on systems and processes, not individuals
- Be specific with action items (no vague "improve monitoring")
- Include timestamps in UTC
- Quantify impact where possible
```

---

## Usage Example

**Input:**

```markdown
## Incident Context

**Incident ID:** INC-2026-001  
**Severity:** SEV1  
**Start Time:** 2026-01-13 14:23 UTC  
**End Time:** 2026-01-13 14:45 UTC  
**Detected By:** Monitoring Alert  
**Services Affected:** Orders API

## Incident Data

### Alerts Triggered
```
[CRITICAL] Orders API - Error Rate Exceeded 10%
[CRITICAL] Orders API - Response Time P99 > 5s
```

### Application Logs
```
2026-01-13T14:23:15Z [ERROR] OrdersController: NullReferenceException at OrdersController.GetById
2026-01-13T14:23:16Z [ERROR] OrdersService: Object reference not set to an instance of an object
(repeated 1000+ times)
```

### Actions Taken
```
14:23 - PagerDuty alert triggered
14:25 - Engineer on-call acknowledged
14:28 - Identified recent deployment v2.5.1
14:30 - Incident Commander assigned
14:35 - Rolled back to v2.5.0
14:40 - Error rate returned to 0.1%
14:45 - Declared resolved
```

### Related Changes
- Deployment v2.5.1 at 14:20 UTC (3 min before incident)
```

**AI Output:**

```markdown
# Incident Postmortem: INC-2026-001

**Date:** January 13, 2026  
**Severity:** SEV1  
**Duration:** 22 minutes  
**Status:** Resolved

## Executive Summary

The Orders API experienced a complete outage for 22 minutes due to a NullReferenceException introduced in deployment v2.5.1. Approximately 15,000 customer requests failed during this window. The incident was detected within 3 minutes via monitoring and resolved by rolling back to the previous stable version.

## Impact

- **Users Affected:** ~5,000 unique users (~10% of active users during window)
- **Duration:** 22 minutes (14:23 - 14:45 UTC)
- **Services Down:** Orders API (100% error rate)
- **Customer-Facing Impact:** Unable to place or view orders; "Service Unavailable" errors
- **SLA Breach:** Yes (99.9% SLA allows 43 min/month; consumed 22 min in single incident)

## Timeline

| Time (UTC) | Event |
|------------|-------|
| 14:20 | Deployment v2.5.1 started |
| 14:23 | First errors detected; PagerDuty alert triggered |
| 14:25 | On-call engineer acknowledged alert |
| 14:28 | Identified correlation with recent deployment |
| 14:30 | Incident Commander assigned; rollback initiated |
| 14:35 | Rollback to v2.5.0 completed |
| 14:40 | Error rate normalized to <0.1% |
| 14:45 | Incident declared resolved; monitoring continues |

## Root Cause

### Immediate Cause
NullReferenceException in OrdersController.GetById() method when accessing a null _cacheService instance.

### Contributing Factors
1. Dependency injection registration for ICacheService was removed in v2.5.1 refactoring
2. No integration tests covering the GetById endpoint
3. Deployment occurred during peak traffic hours
4. No canary deployment strategy (100% traffic immediately)

### Root Cause (5 Whys)
1. Why did Orders API fail? → NullReferenceException when accessing _cacheService
2. Why was _cacheService null? → Not injected via DI
3. Why was it not injected? → DI registration accidentally removed during refactoring
4. Why didn't tests catch this? → No integration test covered this code path
5. Why not? → Integration tests exist only for "happy path" scenarios, not DI failures

**Root Cause:** Insufficient test coverage for dependency injection configuration combined with lack of pre-production validation.

## What Went Well

- ✅ Fast detection (3 minutes from deployment to alert)
- ✅ Clear runbook for rollback procedure
- ✅ Effective communication in #incidents channel
- ✅ Quick rollback execution (5 minutes)

## What Went Wrong

- ❌ No integration tests for critical endpoints
- ❌ Deployment during peak hours without traffic analysis
- ❌ No canary deployment to catch issues before full rollout
- ❌ Code review missed removed DI registration

## Action Items

### Immediate (within 24 hours)

| Action | Owner | Status |
|--------|-------|--------|
| Re-add ICacheService DI registration and deploy fix | @backend-team | DONE |
| Add integration test for GetById endpoint | @backend-team | TODO |

### Short-term (within 1 week)

| Action | Owner | Target Date |
|--------|-------|-------------|
| Audit all controllers for DI dependencies and add tests | @backend-team | Jan 20 |
| Implement canary deployment strategy (10% → 50% → 100%) | @devops-team | Jan 18 |
| Add DI validation to startup (fail-fast if services missing) | @backend-team | Jan 17 |

### Long-term (within 1 month+)

| Action | Owner | Target Date |
|--------|-------|-------------|
| Establish deployment windows (avoid peak hours) | @devops-team | Feb 1 |
| Implement automated rollback on error rate spike | @devops-team | Feb 15 |
| Increase integration test coverage to 80% | @qa-team | Feb 28 |

## Lessons Learned

1. **Canary deployments are critical:** 100% immediate rollout amplified impact. Progressive rollout would have limited blast radius to 10% of traffic.
2. **Integration tests must cover DI:** Unit tests passed but didn't catch runtime DI failure. Integration tests should validate full request lifecycle.
3. **Code review checklists:** Add "verify DI registrations match constructor dependencies" to PR checklist.
4. **Fail-fast validation:** Application should validate DI container at startup and refuse to start if misconfigured.

---
```

---

## Expected Output Quality

AI should generate:
- ✅ Clear, blameless narrative
- ✅ Specific, quantified impact
- ✅ Accurate timeline from logs
- ✅ Actionable (not vague) follow-ups
- ✅ Root cause via systematic analysis (5 whys)
- ✅ Lessons applicable to future incidents

Common issues to watch for:
- ❌ Vague action items ("improve testing")
- ❌ Blaming individuals instead of systems
- ❌ Missing impact quantification
- ❌ No preventive measures identified

---

## After Generation

1. **Review** for accuracy (did AI misinterpret logs?)
2. **Validate** impact numbers with actual data
3. **Assign owners** to action items
4. **Schedule** postmortem review meeting
5. **Publish** to incident documentation system
6. **Track** action items to completion

---

## Related Prompts

- [../observability/analyze-logs.md](../observability/analyze-logs.md) - Deep log analysis
- [root-cause-analysis.md](root-cause-analysis.md) - Detailed RCA
- [../cicd/diagnose-build-failure.md](../cicd/diagnose-build-failure.md) - Non-production failures

---

## Ownership

- **Created by:** SRE Team
- **Maintained by:** Incident Commanders
- **Last reviewed:** January 2026
- **Usage count:** TBD

---

## Feedback

If AI misses critical details or generates poor action items, refine with more specific context.
