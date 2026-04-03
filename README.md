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

- View **all findings** for a **sample report** (report/finding read model).
- **Create a follow-up task** from a finding (task linked to that finding).
- **Update task status**: Not Started, In Progress, Completed.
- **Search and filter** tasks by status and/or priority.
- **Lightweight activity history** (e.g. task created, status changes) for auditability.

## 3. Product focus

- **Security and compliance (HIPAA, SOC 2)** — Demonstrated through design choices and documentation; full compliance is out of scope for a local demo (see [docs/tradeoffs.md](docs/tradeoffs.md)).
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
dotnet run
```

API (default): `http://localhost:5000`

### Frontend

```bash
cd frontend
npm install
npm run dev
```

App (typical Vite dev server): `http://localhost:5173`

## 6. Architecture summary

Layered backend and a thin API layer on the frontend:

| Backend | Responsibility |
|--------|----------------|
| **API** | HTTP, validation, DTO mapping |
| **Application** | Use cases (create task from finding, update status, list with filters) |
| **Domain** | Entities, enums (task status, priority), rules |
| **Infrastructure** | EF Core, SQLite, persistence |

| Frontend | Responsibility |
|----------|------------------|
| **Pages** | Report findings, task list, task detail / activity |
| **Components** | Finding rows, task rows, filters, activity feed |
| **API modules** | Typed calls to the backend |

Design choices: explicit request/response DTOs, no EF entities at the API boundary, scope kept small but production-minded.

## 7. Assumptions

- Single-user or low-concurrency **demo** environment.
- **No authentication** in the first iteration (documented tradeoff).
- Sample report and findings are **seeded or static** — not a live imaging or report-ingestion pipeline.
- Consistency handled in the application layer (single database, no distributed transactions).

## 8. Security and compliance notes

This repo is a **demonstration**, not a certified HIPAA or SOC 2 deployment.

**In scope for the demo**

- Avoid real **PII/PHI**; use synthetic sample content.
- Validate inputs at API boundaries; use DTOs to limit over-posting and exposure.
- **Activity history** supports an audit narrative (who did what is limited without auth; events still show *what* changed and *when*).
- Document what full **HIPAA** / **SOC 2** would require (encryption, identity, access control, monitoring) — see [docs/tradeoffs.md](docs/tradeoffs.md).

## 9. Future improvements

**Product**

- Assign tasks to users; notifications and due dates.
- Multiple reports per person; richer finding taxonomy.

**Technical**

- Authentication and authorization (e.g. JWT / OIDC) and audit entries tied to user identity.
- **Pagination** for large task lists (MVP may use simple lists).
- Background jobs (reminders, outbound integrations).

**Scale and operations**

- Move from SQLite to PostgreSQL (or managed SQL) for concurrency and ops.
- Caching, queues, structured logging, metrics/tracing (e.g. OpenTelemetry).

**Quality**

- Unit tests for application services; API integration tests for main workflows.

## 10. Notes

The goal is a **small, reviewable** codebase that shows **clear boundaries** (findings vs tasks vs activity), **calm UX**, and **honest** tradeoffs — not a generic CRUD demo without domain context.
