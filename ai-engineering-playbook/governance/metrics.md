# AI Productivity Metrics

**Version:** 1.0  
**Last Updated:** January 2026  
**Owner:** Engineering Leadership

This framework defines how we measure AI's impact on productivity and quality.

---

## Measurement Philosophy

**Core Principle:** Measure outcomes, not activity.

- ❌ Bad metric: "Number of AI prompts used per day"
- ✅ Good metric: "Time from task start to PR submission"

**Why:** Activity metrics can be gamed. Outcome metrics reflect actual value.

---

## Metric Categories

### 1. Productivity Metrics

Measure **speed and efficiency**.

### 2. Quality Metrics

Measure **defects and rework**.

### 3. Adoption Metrics

Measure **usage and engagement**.

### 4. Value Metrics

Measure **ROI and impact**.

---

## 1. Productivity Metrics

### 1.1 Time to First Draft

**Definition:** Time from task assignment to first code commit.

**Measurement:**
- Track start time (task assigned in Jira/Azure DevOps)
- Track first commit time (Git timestamp)
- Calculate median across all tasks

**Target:**
- **Baseline (no AI):** 4 hours
- **With AI:** 3 hours (-25%)

**Segmentation:**
- By task type (new feature, bug fix, refactoring)
- By engineer level (junior, mid, senior)
- By AI maturity level (1-5)

### 1.2 PR Cycle Time

**Definition:** Time from PR open to PR merged.

**Measurement:**
- PR opened timestamp
- PR merged timestamp
- Median cycle time per team/engineer

**Target:**
- **Baseline:** 24 hours
- **With AI:** 18 hours (-25%)

**Why it matters:** Faster PR cycle = faster feature delivery.

### 1.3 Lines of Code per Engineer-Day

**Definition:** Code output per engineer per day (with caveats).

**Measurement:**
- Count net lines added/modified (exclude deletions)
- Track daily per engineer
- Calculate rolling 30-day average

**Caveats:**
- More code ≠ better (quality matters)
- Use as directional indicator only
- Segment by code type (boilerplate vs. complex logic)

**Target:**
- **Baseline:** 150 LOC/day
- **With AI:** 200 LOC/day (+33%)

### 1.4 Rework Rate

**Definition:** Percentage of PRs requiring major revision after initial review.

**Measurement:**
- "Major revision" = >50 lines changed after first review
- Track % of PRs meeting this threshold
- Compare AI-assisted vs. manual

**Target:**
- **AI-assisted:** ≤ Manual code rework rate

**Why it matters:** If AI code requires more rework, it's not actually faster.

### 1.5 Time Saved on Repetitive Tasks

**Definition:** Hours saved on boilerplate, tests, documentation.

**Measurement:**
- Engineer self-reports time saved (survey)
- Validate with task completion data
- Aggregate monthly

**Target:**
- **5 hours/engineer/week saved** within 6 months

---

## 2. Quality Metrics

### 2.1 Defect Rate

**Definition:** Bugs per 1000 lines of code.

**Measurement:**
- Track defects found in code review, QA, production
- Tag commits as "AI-assisted" or "manual"
- Calculate defect rate per category

**Target:**
- **AI-assisted code:** ≤ Manual code defect rate

**Failure condition:** If AI code has >1.5x defect rate, investigate and adjust.

### 2.2 Test Coverage

**Definition:** Percentage of code covered by automated tests.

**Measurement:**
- Code coverage tool (e.g., Coverlet for C#)
- Track separately for AI-generated tests
- Compare to baseline

**Target:**
- **AI-generated code:** ≥80% coverage
- **AI-generated tests:** ≥60% acceptance rate by engineers

### 2.3 Security Vulnerabilities

**Definition:** Number of security issues in AI-generated code.

**Measurement:**
- SAST tool findings (SonarQube, CodeQL)
- Tag AI-generated code
- Count high/critical vulnerabilities

**Target:**
- **AI-generated code:** ≤ Manual code vulnerability rate

**Escalation:** Any critical vulnerability in AI code triggers security review.

### 2.4 Code Review Feedback Volume

**Definition:** Number of comments per PR.

**Measurement:**
- Count PR review comments
- Segment by AI-assisted vs. manual
- Track trend over time

**Target:**
- **AI-assisted PRs:** Similar or fewer comments than manual

**Why it matters:** If AI code generates more feedback, it's not saving reviewer time.

### 2.5 Production Incidents

**Definition:** Production outages or degradations attributed to code.

**Measurement:**
- Incident postmortems identify root cause
- Tag if root cause was AI-generated code
- Calculate incident rate per 1000 deployments

**Target:**
- **AI-assisted deployments:** ≤ Manual deployment incident rate

**Zero tolerance:** If AI code causes customer-impacting incident, mandatory review.

---

## 3. Adoption Metrics

### 3.1 Engineer Adoption Rate

**Definition:** Percentage of engineers actively using AI tools.

**Measurement:**
- "Active" = used AI tool in last 7 days
- Track via AI tool logs
- Segment by team, level, tenure

**Target:**
- **3 months:** 50% adoption
- **6 months:** 80% adoption
- **12 months:** 90% adoption

### 3.2 AI-Assisted Code Percentage

**Definition:** Percentage of commits tagged as AI-assisted.

**Measurement:**
- Parse commit messages for "AI-assisted" tag
- Calculate % of total commits
- Track monthly trend

**Target:**
- **6 months:** 30% of commits
- **12 months:** 50% of commits

### 3.3 Prompt Library Usage

**Definition:** Number of invocations of saved prompts.

**Measurement:**
- Track downloads/views of prompts in this repo
- Count references in commit messages
- Survey: "Which prompts do you use regularly?"

**Target:**
- **Each prompt used by ≥3 engineers within 90 days** of publication

**Action:** Retire unused prompts, promote popular ones.

### 3.4 Agent Skill Invocations

**Definition:** Number of times agent skills are triggered.

**Measurement:**
- Log each agent skill run (PR checks, CI analysis, etc.)
- Count daily/weekly invocations
- Track acceptance rate (how often output is acted upon)

**Target:**
- **PR Readiness Skill:** 90% of PRs
- **Test Coverage Skill:** 50% of PRs
- **Acceptance rate:** >60%

### 3.5 Maturity Level Distribution

**Definition:** How many engineers at each maturity level (1-5).

**Measurement:**
- Quarterly self-assessment survey
- Tech Lead validation of self-assessments
- Track movement between levels

**Target (12 months):**
- Level 1: 5%
- Level 2: 20%
- Level 3: 50%
- Level 4: 20%
- Level 5: 5%

---

## 4. Value Metrics

### 4.1 Return on Investment (ROI)

**Definition:** Value generated vs. cost of AI tools.

**Calculation:**
```
ROI = (Value Generated - Cost) / Cost × 100%

Value Generated = Engineer Hours Saved × Average Hourly Cost
Cost = AI Tool Licenses + Training + Maintenance
```

**Example:**
- 50 engineers save 5 hours/week each
- 50 × 5 × 52 = 13,000 hours/year
- @ $100/hour = $1.3M value
- AI cost = $200K (licenses + training)
- ROI = ($1.3M - $200K) / $200K = 550%

**Target:** ROI > 300% within 12 months

### 4.2 Feature Velocity

**Definition:** Story points delivered per sprint.

**Measurement:**
- Track sprint velocity in Jira/Azure DevOps
- Compare before/after AI adoption
- Control for team changes

**Target:** +20% velocity within 6 months

### 4.3 Customer-Facing Improvements

**Definition:** Features delivered to customers faster.

**Measurement:**
- Time from feature request to production
- Count of features shipped per quarter
- Customer satisfaction (NPS impact)

**Target:**
- 30% more features shipped per quarter
- Maintain or improve quality (NPS stable or up)

### 4.4 Engineering Satisfaction

**Definition:** How engineers feel about AI tools.

**Measurement:**
- Quarterly survey (1-5 scale):
  - "AI makes me more productive"
  - "I trust AI-generated code"
  - "AI improves my work quality"
  - "I would not want to work without AI"
  
**Target:**
- Average score ≥4.0 across all questions

**Leading indicator:** Low satisfaction predicts low adoption.

---

## Measurement Cadence

### Daily
- Agent skill invocations
- AI tool usage logs

### Weekly
- PR cycle time
- Time to first draft
- LOC per engineer-day

### Monthly
- Defect rates
- Test coverage
- Adoption rates
- Prompt usage

### Quarterly
- ROI analysis
- Maturity level assessment
- Engineer satisfaction survey
- Value metrics (velocity, features shipped)

### Annually
- Comprehensive impact report
- Industry benchmarking
- Strategic planning

---

## Dashboards

### Executive Dashboard (CTO)

**Metrics displayed:**
- ROI %
- Feature velocity trend
- Adoption rate
- Production incident rate (AI vs. manual)

**Update frequency:** Monthly

### Engineering Manager Dashboard

**Metrics displayed:**
- PR cycle time
- Rework rate
- Defect rate
- Team adoption rate
- Maturity level distribution

**Update frequency:** Weekly

### Individual Engineer Dashboard

**Metrics displayed:**
- Personal productivity (time saved)
- AI code acceptance rate
- Maturity level progress
- Recommended prompts

**Update frequency:** Daily

---

## Baseline Measurement

**Before AI rollout, establish baselines:**

- Time to first draft (median)
- PR cycle time (median)
- LOC per engineer-day
- Defect rate
- Test coverage
- Feature velocity

**Measure for 30 days minimum.**

**Then track same metrics post-AI adoption.**

---

## Data Collection Methods

### Automated

- Git commit timestamps and tags
- PR open/merge timestamps
- CI/CD build logs
- Code coverage reports
- SAST/DAST scan results
- AI tool usage logs

### Manual

- Engineer surveys (quarterly)
- Manager assessments
- Incident postmortems
- Customer feedback

### Hybrid

- Commit message analysis (AI-assisted tag)
- Code review comments (manual categorization)

---

## Reporting

### Monthly Report Template

**To:** Engineering Leadership  
**From:** AI Adoption Lead

**Summary:**
- Adoption: X% of engineers using AI weekly
- Productivity: Y hours saved per engineer
- Quality: Defect rate stable/improved
- ROI: Z% return on investment

**Highlights:**
- Top 3 wins (e.g., "PR cycle time down 30%")
- Top 3 challenges (e.g., "Test coverage not meeting target")

**Next Month:**
- Actions to address challenges
- New prompts/skills to deploy

### Quarterly Business Review

**Audience:** CTO, VPs, Senior Leadership

**Contents:**
- ROI analysis with financial impact
- Maturity model progress
- Case studies (before/after examples)
- Challenges and mitigation plans
- Roadmap for next quarter

---

## Key Performance Indicators (KPIs)

### Primary KPIs

| KPI | Target | Status |
|-----|--------|--------|
| **Adoption Rate** | 80% in 6 months | Track monthly |
| **ROI** | >300% in 12 months | Track quarterly |
| **Defect Rate** | ≤ manual code | Track monthly |
| **PR Cycle Time** | -25% vs. baseline | Track weekly |

**If any KPI is off-target, escalate to Engineering Leadership.**

---

## Continuous Improvement

### Feedback Loop

1. **Measure** → Collect data
2. **Analyze** → Identify patterns
3. **Adjust** → Refine prompts, skills, policies
4. **Re-measure** → Validate improvements
5. **Repeat** → Continuous cycle

### A/B Testing

Test prompt/skill variations:
- **Control group:** Existing prompt
- **Treatment group:** New prompt variant
- **Measure:** Acceptance rate, time saved
- **Decision:** Adopt better-performing variant

**Run A/B tests quarterly.**

---

## Red Flags

Escalate immediately if:

- ❌ Defect rate in AI code >1.5x manual code
- ❌ ROI < 100% after 6 months
- ❌ Engineer satisfaction < 3.0
- ❌ Adoption plateaus below 50%
- ❌ Production incidents attributed to AI code

**Action:** Pause rollout, investigate, adjust strategy.

---

## Summary

**Measure rigorously. Improve continuously.**

Key principles:
- Measure outcomes, not activity
- Compare AI-assisted vs. manual code
- Track productivity AND quality
- Report transparently
- Use data to drive decisions

**If you can't measure it, you can't manage it.**

---

*For related content, see [PLAYBOOK.md](../PLAYBOOK.md) and [maturity-model.md](../maturity-model.md).*
