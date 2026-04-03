# Tradeoffs & Assumptions

This project is **Ezra Follow-Up Tracker**: a demo focused on **post-report** workflows (sample **findings** → **follow-up tasks** → **status** / **priority** → **activity history**). It is intentionally scoped as a small full-stack exercise. The goal is to demonstrate sound engineering judgment and domain-aligned boundaries, not to fully replicate a production-grade healthcare system. Below are the key tradeoffs and decisions made.

---

## 1. Database Choice: SQLite

**Decision:** Use SQLite via EF Core

**Why:**

- Zero setup, no external dependencies
- Easy for reviewers to run locally
- Keeps focus on API and domain design instead of infrastructure

**Tradeoffs:**

- Not suitable for concurrent, high-scale workloads
- Limited support for advanced indexing and performance tuning
- No built-in horizontal scaling

**Production Alternative:**

- PostgreSQL or SQL Server with proper indexing, migrations, and connection pooling

---

## 2. Authentication & Authorization

**Decision:** No real authentication implemented

**Why:**

- Out of scope for a take-home
- Avoid unnecessary complexity (JWT, identity providers, RBAC)

**Tradeoffs:**

- Endpoints are not protected
- No user identity or role-based access control
- Not secure for real-world usage

**Production Alternative:**

- OAuth2 / OIDC (e.g., Auth0, Azure AD B2C)
- Role-based access control (RBAC)
- Audit logging tied to user identity

---

## 3. Data Sensitivity (PHI)

**Decision:** No real PHI stored, only seeded demo data

**Why:**

- Avoid handling sensitive healthcare data
- Keep project safe and shareable

**Tradeoffs:**

- Does not demonstrate full data protection workflows
- No encryption, masking, or access auditing required

**Production Considerations:**

- Encrypt data at rest and in transit
- Strict access controls and audit logs
- Data retention and deletion policies
- Business Associate Agreements (BAA)

---

## 4. External Integrations

**Decision:** No third-party integrations; sample report/findings are **in-repo** (seed or static read model)

**Why:**

- Keeps the system self-contained and easy to run for reviewers
- Avoids API keys, rate limits, and external dependencies
- Focus remains on **clear boundaries**: report/finding **reads** vs **task** writes vs future **ingestion** adapters

**Tradeoffs:**

- Does not demonstrate live imaging, report delivery, or EHR connectivity
- No retries, circuit breakers, or failure handling for external calls

**Production Alternative:**

- Ingest reports from Ezra or clinical systems behind a dedicated integration layer
- EHR/EMR messaging, webhooks, or queues (Kafka, SQS) with resilience patterns (retry, timeout, circuit breaker)

---

## 5. Audit Trail vs Event Sourcing

**Decision:** Lightweight **activity history** (append-style events on tasks) instead of full event sourcing

**Why:**

- Meets MVP **auditability** without the complexity of event stores and projections
- Sufficient to show **task created**, **status changes**, and similar events for reviewers
- Keeps code simple and readable

**Tradeoffs:**

- No full history reconstruction of the entire system
- Limited traceability of complex workflows
- Without authentication, “who” is often omitted or synthetic — **when** and **what** still demonstrate the pattern

**Production Alternative:**

- Event sourcing with append-only logs
- CQRS for read/write separation
- Replayable events for debugging and analytics

---

## 6. Architecture Scope

**Decision:** Modular monolith + SPA frontend

**Why:**

- Faster to build and easier to review
- Avoids distributed system complexity
- Clear separation of concerns within a single deployable unit

**Tradeoffs:**

- Limited independent scaling of components
- Tighter coupling compared to microservices

**Production Alternative:**

- Extract services based on clear boundaries (e.g., AI processing, ingestion)
- Introduce service-to-service communication (gRPC/REST)

---

## 7. HIPAA & SOC 2 Considerations

**Decision:** Discussed, not fully implemented

**Why:**

- Full compliance requires infrastructure, policies, and audits beyond code
- Not realistic for a take-home project

**What is Demonstrated:**

- Awareness of sensitive data handling and avoidance of real **PII/PHI** in demo data
- Separation of **finding read models** vs **follow-up task** lifecycle vs **activity** recording
- **Activity history** as a stepping stone toward full audit controls (correlate with HIPAA audit controls and SOC 2 logging expectations in documentation)

**What is NOT Implemented:**

- Formal access controls and audit requirements
- Encryption key management
- Compliance logging and monitoring
- Security policies and procedures

**Production Requirements:**

| Standard | Key Requirements |
|---|---|
| **HIPAA** | PHI protection, audit controls, access tracking, encryption and secure transmission |
| **SOC 2** | Security, availability, processing integrity, monitoring, incident response, change management |

---

## 8. General Assumptions

- Single-tenant demo environment
- One **sample report** with multiple **findings** is enough for MVP review
- Low data volume; search/filter can start as simple query parameters
- No concurrent heavy workloads
- Developer-run locally (no CI/CD required)
- **Calm, minimal UI** — clarity over feature breadth; healthcare-appropriate tone

---

## Summary

This project prioritizes:

- **Clarity** over completeness
- **Simplicity** over scalability
- **Demonstration of judgment** over production readiness

The decisions above reflect a deliberate focus on building a clean, understandable system that can evolve into a production-ready architecture with the right extensions.
