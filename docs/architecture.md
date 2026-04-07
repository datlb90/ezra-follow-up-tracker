# Architecture — Ezra Follow-Up Tracker

## Overview

This project is a **small, production-minded** full-stack app: after a **sample MRI report** is available, users view **findings** (with severity), create **follow-up tasks** from those findings, update **status**, see **automatically computed priority**, filter/search tasks, and see a **lightweight activity history**.

The backend is a **modular monolith**; the frontend is a **Vue SPA**. That keeps setup and review simple while still showing **clear module boundaries** aligned with the product:

- **Report / finding read models** — What the report says (demo data).
- **Follow-up task management** — Work the user or care team tracks.
- **Status updates** — Controlled transitions (Not Started → In Progress → Completed).
- **Activity / audit trail** — Append-only style events for important changes.
- **Future integrations** — Imaging, EHR, or notification systems are **out of scope** for MVP; boundaries below leave room to add adapters without collapsing the domain.

---

## Bounded contexts (conceptual)

| Boundary | Role in MVP | Notes |
|----------|-------------|--------|
| **Reporting (read)** | Sample `Report` + `Finding` data exposed read-only to the UI | Could be seeded tables, JSON seed, or a thin query service — not a real PACS/RIS integration. |
| **Follow-up tasks** | Create/list/update tasks; each task references a **finding** | Task commands live here; finding IDs are foreign keys, not duplicated clinical narrative as source of truth. |
| **Status and workflow** | Enforce allowed status values and transitions | Application layer validates; activity records significant changes. |
| **Activity** | Record events (task created, status changed, etc.) | Lightweight history, not full event sourcing (see [tradeoffs.md](tradeoffs.md)). |
| **Integrations (future)** | N/A in MVP | Prefer ports/adapters or separate API modules later so clinical ingestion does not entangle with task CRUD. |

---

## Domain model

### Report

Anchors a **sample digital report** (one report for the demo is enough).

Example fields:

- `Id`
- `Title` or external reference (synthetic)
- `ReceivedAt` (or `CreatedAt`)
- Optional: short summary for UI

### Finding

A **single actionable row** from the report (read model shaped for the UI).

Example fields:

- `Id`
- `ReportId`
- `Title` / headline
- `Description` (plain text for demo)
- `Severity` — `Low`, `Medium`, `High` (used as an input to automatic task priority scoring)

A report has many findings.

### FollowUpTask

Work tracked **because of** a finding.

Example fields:

- `Id`
- `FindingId` (required — task is created **from** a finding)
- `Title` (may default from finding; editable if product allows)
- `Description` (optional)
- `Status` — **NotStarted**, **InProgress**, **Completed**
- `DueAt` (optional) — used for due-date urgency scoring
- `CreatedAt`, `UpdatedAt`

Priority is **not stored** on the task. It is computed at read time by the `TaskPriorityService` using finding severity, due-date urgency, and task status. The result is a score, a level (`Low`, `Medium`, `High`, `Critical`), and a human-readable reason string. Overdue tasks are always `Critical`.

### TaskActivity (or Activity)

**Lightweight audit** entries.

Example fields:

- `Id`
- `FollowUpTaskId`
- `OccurredAt`
- `Type` (e.g. `TaskCreated`, `StatusChanged`, `TaskUpdated`)
- `Summary` or structured payload (e.g. previous/new status) — keep DTOs explicit at the API
- `ActorId` (nullable) — the authenticated user who performed the action
- `ActorName` (nullable) — denormalized snapshot of the actor's display name at the time of the action

---

## Domain relationships

- One **Report** has many **Findings**.
- One **Finding** may have many **FollowUpTasks** (if the product allows multiple tasks per finding; otherwise enforce one-to-one in the application layer).
- One **FollowUpTask** has many **TaskActivity** rows (ordered by time).

---

## Layer boundaries (backend)

### API layer

- HTTP, model validation, call application services, return **response DTOs**.
- No business rules beyond input shape; no direct EF usage in controllers if the project uses an application layer.

### Application layer

- Use cases such as: `GetReportFindings`, `CreateFollowUpTaskFromFinding`, `UpdateTaskStatus`, `SearchFollowUpTasks`, `GetTaskActivity`.
- `TaskPriorityService` — rule-based scoring engine that computes priority at read time from finding severity, due-date urgency, and task status.
- Orchestrates domain + persistence; records **activity** on task creation and status changes. Status transitions are currently unconstrained; enforcing a transition matrix is a future improvement.

### Domain layer

- Entities: `Report`, `Finding`, `FollowUpTask`, `TaskActivity`, `User`.
- Enums: `FollowUpTaskStatus`, `FindingSeverity`, `TaskPriorityLevel`, `ActivityType`.
- Invariants that belong in the domain (e.g. status transition rules could be enforced here in a future iteration).

### Infrastructure layer

- `DbContext`, EF mappings, `EnsureCreated()` for schema creation (no migrations), SQLite.
- Optional: seed data for one report and several findings.

**Dependency direction:** API → Application → Domain; Infrastructure implements persistence for Application.

---

## API overview (illustrative)

Routes are indicative; final paths should stay **resource-oriented** and **DTO-backed**.

**Authentication:** All data endpoints require a valid JWT Bearer token (`[Authorize]` at the controller level). Only `POST /api/auth/login` and `POST /api/auth/register` are anonymous. See the Setup section in [README.md](../README.md) for demo credentials.

### Authentication

#### `POST /api/auth/register`

Create a new account. Returns a JWT token on success.

#### `POST /api/auth/login`

Authenticate with email and password. Returns a JWT token on success.

#### `GET /api/auth/me`

Returns the authenticated user's profile. Requires Bearer token.

### Report and findings (read)

#### `GET /api/reports`

List reports available in the demo (often one).

#### `GET /api/reports/{reportId}/findings`

All findings for that report (MVP: “view all findings for a sample report”).

### Follow-up tasks

#### `GET /api/follow-up-tasks`

List tasks with optional query parameters:

- `status`
- `search` (title/description — MVP-level text search)

Each task response includes computed `priorityScore`, `priorityLevel`, and `priorityReason` fields. Priority-level filtering is done client-side.

#### `GET /api/follow-up-tasks/{id}`

Task detail. Activity history is fetched separately via the activities endpoint below.

#### `POST /api/follow-up-tasks`

Create from a finding; body includes `findingId`, `title`, optional `description`, and optional `dueAt`.

#### `PUT /api/follow-up-tasks/{id}`

Update task fields such as **title**, **description**, **dueAt**, or **status**. Priority is computed automatically and cannot be set manually.

### Activity

#### `GET /api/follow-up-tasks/{taskId}/activities`

List of activity events for that task, ordered by most recent first. Each entry includes `actorId` and `actorName` identifying the user who performed the action.

---

## API design notes

- Request and response **DTOs** for every endpoint; do not expose EF entities.
- **Finding** endpoints are **read-focused**; task creation is the main **write** from the finding context.
- Errors: consistent problem shape and HTTP status codes for validation vs not found.

---

## Frontend structure

Vue SPA: **calm**, readable, accessible forms and tables; explicit **loading / empty / error** states.

### Main responsibilities

- **Authentication** — login and registration pages; Pinia auth store with `localStorage` token persistence; Vue Router navigation guard redirecting unauthenticated users to `/login`; Axios interceptors for Bearer token attachment and automatic logout on 401.
- Show sample report and **findings list**.
- Create **follow-up task** from a finding (modal or side panel).
- **Task list** with **search** and **filters** (status, priority level).
- One-click **status toggle** button group on each task row.
- **Priority badge** with hover tooltip showing the score breakdown.
- **Task activity** panel with history timeline.

### Actual layout

```text
src/
  api/
    authApi.ts
    http.ts
    reportsApi.ts
    followUpTasksApi.ts

  components/
    layout/        (AppLayout, AppHeader, AppSidebar)
    findings/      (FindingCard, CreateTaskModal)
    tasks/         (TaskRow, TaskFilters, TaskActivityPanel)
    shared/        (LoadingSpinner, SeverityBadge, PriorityLevelBadge)

  pages/
    DashboardPage.vue
    FindingsPage.vue
    TasksPage.vue
    LoginPage.vue
    RegisterPage.vue

  stores/
    authStore.ts
    taskStore.ts

  router/
    index.ts

  types/
    api.ts
```

API calls live in `api/`; page components orchestrate loading, error, and empty states.

---

## Scalability and evolution

- **SQLite** is fine for local demo and single-user review; see [tradeoffs.md](tradeoffs.md) for production storage and scaling.
- If **integrations** arrive later, keep **ingestion** and **report versioning** behind a dedicated module or service contract so the task API remains stable.
