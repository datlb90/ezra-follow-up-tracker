# Tradeoffs & Assumptions

This project is **Ezra Follow-Up Tracker**: a demo focused on **post-report** workflows (sample **findings** → **follow-up tasks** → **status** → **automatic priority** → **activity history**). It is intentionally scoped as a small full-stack exercise. The goal is to demonstrate sound engineering judgment and domain-aligned boundaries, not to fully replicate a production-grade healthcare system. Below are the key tradeoffs and decisions made.

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

**Decision:** JWT-based authentication with demo accounts

**What is implemented:**

- User registration and login endpoints with BCrypt password hashing
- JWT token issuance with HMAC-SHA256 signing
- All data endpoints (reports, tasks, activities) protected with `[Authorize]`; only login and registration are anonymous
- Frontend login/register pages, Pinia auth store with `localStorage` token persistence
- Vue Router navigation guard redirecting unauthenticated users to `/login`
- Axios 401 response interceptor for automatic logout on expired/invalid tokens
- Two seeded demo accounts for reviewer convenience
- Swagger UI Bearer token input for manual API testing
- Demo-only secret key in `appsettings.json` (clearly named, documented as not production-safe)

**Tradeoffs:**

- Token stored in `localStorage` — vulnerable to XSS in a real app; production would use httpOnly cookies or a BFF pattern
- No refresh tokens — token expires after 60 minutes with no silent renewal
- No RBAC — all authenticated users see the same data; no per-user scoping
- Audit trail entries are not yet tied to the authenticated user identity

**Production Alternative:**

- httpOnly cookie token storage or Backend-for-Frontend (BFF) pattern
- Refresh token rotation with short-lived access tokens
- OAuth2 / OIDC (e.g. Auth0, Azure AD B2C) for SSO and MFA
- Role-based access control (RBAC) with per-user or per-organization data scoping
- Audit logging tied to authenticated user identity

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
- Audit entries now capture **who** via denormalized `ActorId` and `ActorName` fields on `TaskActivity`, preserving the actor's name at the time of the action even if the user is later renamed or deleted. Both fields are nullable for backward compatibility with entries created before authentication was introduced

**Production Alternative:**

- Event sourcing with append-only logs
- CQRS for read/write separation
- Replayable events for debugging and analytics

---

## 6. Prioritization: Computed vs Stored

**Decision:** Priority is computed at read time by a rule-based `TaskPriorityService`, not stored on the task entity

**Why:**

- Priority depends on factors that change over time (due-date urgency shifts daily, status changes via user action)
- Avoids stale priority values that drift from reality
- Keeps the domain model simpler — no manual priority field to maintain or reconcile
- Scoring rules (severity weight, due-date brackets, status bonus) are transparent and easy to explain

**Scoring rules:**

| Factor | Value | Points |
|--------|-------|--------|
| Finding severity | High | +50 |
| | Medium | +30 |
| | Low / Unknown | +10 |
| Due date | Overdue | Critical override |
| | 0–3 days | +40 |
| | 4–7 days | +20 |
| | Later / none | +0 |
| Task status | Not Started | +10 |
| | In Progress | +5 |
| | Completed | +0 |

Score mapping: ≥80 → High, ≥50 → Medium, <50 → Low. Overdue always → Critical.

**Tradeoffs:**

- Priority cannot be queried or sorted at the database level (filtering is client-side)
- Computation runs on every read — acceptable for demo scale, not ideal for large datasets
- No manual override — users cannot pin a task to a specific priority level

**Production Alternative:**

- Materialized priority column updated via background job or domain event
- Configurable weight profiles per organization
- Manual override with an audit trail entry

---

## 7. Architecture Scope

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

## 8. Security and Compliance

### 8a. Security considerations (what the demo demonstrates)

This demo is not a production deployment, but it does incorporate practices that reflect security awareness:

- **No real PHI** — all data is synthetic. Report titles, finding descriptions, and task content are fabricated seed data with no patient-identifiable information.
- **Input validation at API boundaries** — every endpoint uses explicit request DTOs with data annotations (`[Required]`, `[StringLength(200)]`, `[StringLength(2000)]`). Invalid payloads are rejected before reaching business logic. Foreign key references (e.g. `FindingId`) are checked for existence before writes.
- **Layered architecture** — the API layer is separated from application services, domain entities, and infrastructure. EF entities never cross the API boundary; only typed DTOs are serialized to clients. This limits over-posting and accidental data exposure.
- **Activity history as proto-audit-log** — task creation, status transitions, and other significant events are recorded with timestamps in an append-style `TaskActivity` table. Authentication is present; tying entries to the authenticated user identity is a planned next step.
- **All data endpoints protected** — every controller serving reports, tasks, and activities requires a valid JWT Bearer token via `[Authorize]`. Only login and registration are anonymous. The frontend enforces route guards and handles 401 responses with automatic logout.
- **Global exception handler** — `UseExceptionHandler` middleware catches unhandled exceptions and returns a generic `{ "message": "An unexpected error occurred." }` response. Internal details, stack traces, and EF exception messages are never exposed to callers.
- **Configuration-based secrets** — connection strings and settings are loaded from `appsettings.json` and can be overridden by environment variables. No credentials are hardcoded. In production, these would be managed by a secrets vault (e.g. Azure Key Vault, AWS Secrets Manager).
- **Logging avoids sensitive content** — no PHI exists in the seed data. The application does not log request or response bodies. Production logging would add structured logging with explicit PHI exclusion rules.

### 8b. HIPAA / SOC 2 discussion (what production would require)

**This demo is not HIPAA compliant or SOC 2 certified.** Full compliance requires infrastructure, organizational policies, training, and third-party audits that are beyond the scope of a take-home project. The goal here is to show awareness of what a production healthcare system would need.

**Identity and access**

- Role-based access control (RBAC) with least-privilege principles
- SSO with multi-factor authentication (OAuth2 / OIDC, e.g. Auth0 or Azure AD B2C)
- Service accounts with scoped permissions for background jobs and integrations

**Audit**

- Immutable, timestamped audit logs tied to authenticated user identity
- Log retention policy aligned with regulatory requirements
- Separation of audit storage from application data to prevent tampering

**Data protection**

- Encryption at rest (disk-level or column-level for PHI fields) and in transit (TLS everywhere)
- PHI masking in non-production environments
- Encrypted, access-controlled backups with regular restore testing
- Data retention and deletion policies (right to erasure where applicable)
- Business Associate Agreements (BAAs) with cloud providers and subprocessors

**Operations**

- Vulnerability management — dependency scanning, container scanning, penetration testing, patch cadence
- Incident response — documented runbooks for breach detection, containment, notification (HIPAA Breach Notification Rule), and post-incident review
- Change management — code review, CI/CD with automated testing, staged rollouts

**Monitoring**

- Access reviews — periodic audits of who has access to what
- Anomaly detection — alerting on unusual access patterns or data exports
- Uptime and availability tracking (SOC 2 Availability criteria)
- Centralized logging and tracing (e.g. OpenTelemetry, ELK, Datadog)

**Standards mapping:**

| Standard | Key Requirements |
|---|---|
| **HIPAA** | PHI protection, audit controls, access tracking, encryption, secure transmission, breach notification |
| **SOC 2** | Security, availability, processing integrity, monitoring, incident response, change management |

---

## 9. General Assumptions

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
