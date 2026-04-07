# Ezra Follow-Up Tracker

## 1. Project overview

**Ezra Follow-Up Tracker** is a small full-stack app for managing **post-scan follow-up actions** after an MRI-based screening report is available. Ezra delivers a digital report with actionable findings; this project focuses on the **workflow after the report exists**: helping a patient or care team **organize and complete** recommended follow-up steps.

The stack:

- RESTful API on **.NET** (Web API)
- **Vue 3** frontend
- **SQLite** via Entity Framework Core
- **Clean separation** between frontend and backend (explicit API contracts, no leaking persistence models into the UI)

More detail: [docs/architecture.md](docs/architecture.md) (domain and boundaries), [docs/tradeoffs.md](docs/tradeoffs.md) (assumptions, compliance posture, scalability).

## 2. MVP features

- View **all findings** (with severity) for a **sample report** (report/finding read model).
- **Create a follow-up task** from a finding with an optional due date.
- **Update task status** via a one-click toggle button group: Not Started, In Progress, Completed.
- **Automatic priority**: each task is scored by finding severity, due-date urgency, and status — overdue tasks are always Critical. Hover the priority badge to see the full score breakdown.
- **Search and filter** tasks by status, priority level, and text search.
- **Lightweight activity history** (e.g. task created, status changes) for auditability.
- **Dashboard** showing task counts by status and a critical-priority count.

## 3. Product focus

- **Security and compliance (HIPAA, SOC 2)** — Demonstrated through design choices and documentation; full compliance is out of scope for a local demo (see [docs/tradeoffs.md](docs/tradeoffs.md)).
- **Authentication** — JWT-based auth protecting all API endpoints. Login and registration pages, route guards, and automatic token handling on the frontend. Passwords hashed with BCrypt. Swagger UI includes a Bearer token input for manual testing.
- **Domain modeling** — Report/finding read models vs follow-up task lifecycle vs status updates vs room for future integrations.
- **Calm UX** — Clear, minimal, healthcare-appropriate UI (readable states, no visual noise).
- **Auditability** — Activity history makes workflow changes tangible for reviewers.

## 4. Tech stack

### Backend

- .NET 8 (Web API)
- Entity Framework Core
- SQLite

### Frontend

- Vue 3 (Composition API, TypeScript)
- HTTP client for API calls (e.g. Axios), centralized API modules

### Tooling

- Cursor (AI-assisted development)
- Git with structured commits (`feat:`, `chore:`, `docs:`)

## 5. Setup

### Prerequisites

- .NET 8 SDK
- Node.js (v18+ recommended)
- npm or yarn

### Backend

```bash
cd backend
dotnet restore
dotnet run --project src/Ezra.Api
```

API (default): `http://localhost:5124` — Swagger UI at `/swagger`

The database is created and seeded automatically on first run (SQLite via `EnsureCreated`).

**Demo credentials** (seeded on first run):

| Email | Password |
|-------|----------|
| `demo@ezra.com` | `Password123!` |
| `admin@ezra.com` | `Password123!` |

> If you already have an `ezra.db` from before auth was added, delete it and restart the backend so the Users table is created and seeded.

### Tests

```bash
cd backend
dotnet test
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

App (Vite dev server): `http://localhost:5173` — proxies `/api` to the backend automatically.

## 6. Architecture summary

Layered backend and a thin API layer on the frontend:

| Backend | Responsibility |
|--------|----------------|
| **API** | HTTP, validation, DTO mapping |
| **Application** | Use cases (create task, update status, list with filters, compute priority) |
| **Domain** | Entities, enums (task status, finding severity, priority level), rules |
| **Infrastructure** | EF Core, SQLite, persistence |

| Frontend | Responsibility |
|----------|------------------|
| **Pages** | Report findings, task list, task detail / activity |
| **Components** | Finding rows, task rows, filters, activity feed |
| **API modules** | Typed calls to the backend |

Design choices: explicit request/response DTOs, no EF entities at the API boundary, scope kept small but production-minded.

## 7. Assumptions

- Single-user or low-concurrency **demo** environment.
- **Demo authentication** — JWT-based auth with seeded demo accounts. All API endpoints except login and registration require a valid Bearer token. Passwords are hashed with BCrypt. The JWT secret key in `appsettings.json` is for local demo only; production would use a secrets vault. Token is stored in `localStorage` (production would use httpOnly cookies or a more secure strategy).
- Sample report and findings are **seeded or static** — not a live imaging or report-ingestion pipeline.
- Consistency handled in the application layer (single database, no distributed transactions).

## 8. Security and compliance notes

### Security considerations

The following measures are present in this demo:

- **No real PHI** — all report, finding, and task data is synthetic seed content. No patient-identifiable information is stored or transmitted.
- **Authentication** — JWT-based auth with BCrypt password hashing. Registration and login endpoints issue signed tokens; all data endpoints (reports, tasks, activities) and `/api/auth/me` are protected with `[Authorize]`. The frontend enforces route guards and handles 401 responses with automatic logout. The JWT secret key is a demo-only value stored in `appsettings.json`; production deployments would use a secrets vault.
- **Input validation** — all API endpoints enforce validation via DTOs with data annotations (`[Required]`, `[StringLength]`). Invalid requests are rejected with structured error responses before reaching business logic.
- **Layered architecture** — the API surface is separated from domain logic and persistence. EF entities never leak to the client; only explicit DTOs cross the API boundary.
- **Audit trail** — the activity history records what changed and when (task created, status transitions). This is a stepping stone toward full audit logging.
- **Protected API boundary** — all data endpoints require a valid JWT Bearer token via `[Authorize]` at the controller level. Only login and registration are anonymous.
- **Global exception handler** — unhandled exceptions return a generic error message. Internal details and stack traces are not exposed to callers.
- **Configuration-based secrets** — the connection string and other settings are loaded from `appsettings.json` / environment variables, not hardcoded. In production, these would be managed by a secrets vault.
- **Logging avoids sensitive content** — no PHI exists in the seed data, and the application does not log request bodies.

### HIPAA / SOC 2 posture

This demo is **not** HIPAA compliant or SOC 2 certified. Full compliance requires infrastructure, organizational policies, and third-party audits that are beyond the scope of a take-home project.

In a production healthcare deployment, the next steps would include:

- **RBAC** — role-based access control tied to user identity
- **SSO / MFA** — single sign-on with multi-factor authentication (e.g. OAuth2 / OIDC)
- **Audit logging** — immutable, timestamped logs tied to authenticated user identity
- **Data retention policy** — defined lifecycle for PHI storage, archival, and deletion
- **Encryption** — data encrypted at rest and in transit (TLS, disk-level or column-level encryption)
- **Secure backups** — encrypted, access-controlled, regularly tested
- **Vulnerability management** — dependency scanning, penetration testing, patch cadence
- **Incident response** — documented runbooks for breach detection, containment, and notification
- **Infrastructure controls** — network segmentation, least-privilege IAM, secure CI/CD
- **Monitoring and access reviews** — anomaly detection, periodic access audits, uptime/availability tracking

For a detailed discussion, see [docs/tradeoffs.md](docs/tradeoffs.md).

## 9. Future improvements

**Product**

- Assign tasks to users; notifications.
- Multiple reports per person; richer finding taxonomy.
- Configurable priority weights or ML-based scoring.

**Technical**

- Refresh tokens and token expiry handling.
- Tie audit trail entries to authenticated user identity.
- **Pagination** for large task lists (MVP may use simple lists).
- Background jobs (reminders, outbound integrations).
- Server-side priority-level filtering (currently client-side since priority is computed, not stored).

**Scale and operations**

- Move from SQLite to PostgreSQL (or managed SQL) for concurrency and ops.
- Caching, queues, structured logging, metrics/tracing (e.g. OpenTelemetry).

**Quality**

- API integration tests for main workflows.

## 10. Notes

The goal is a **small, reviewable** codebase that shows **clear boundaries** (findings vs tasks vs activity), **calm UX**, and **honest** tradeoffs — not a generic CRUD demo without domain context.
