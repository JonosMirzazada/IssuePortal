# IssuePortal - System Design

# 1. System Architecture

IssuePortal follows a three-layer architecture:

1. Frontend: planned, not started yet
2. Backend: implemented
3. Database: implemented

```
React frontend  --HTTP/JSON + JWT-->  ASP.NET Core Web API  --EF Core-->  PostgreSQL
```

---

# 2. Frontend

## Technology

React + TypeScript + Tailwind CSS. The plan is to use Vite as the dev server, on `http://localhost:5173`.

## Responsibilities

The frontend is responsible for:

- Providing the user interface
- Handling user interactions
- Sending requests to the backend API
- Displaying information received from the backend
- Storing the JWT after login and sending it with every request

The backend already allows requests from `http://localhost:5173` through CORS.

---

# 3. Backend

## Technology

C# + ASP.NET Core Web API (.NET 10), Entity Framework Core (Npgsql), and JWT bearer authentication. Swagger is available in development.

The code is in `Backend/IssuePortal.Api/`.

## Responsibilities

The backend is responsible for:

- Handling HTTP requests
- Implementing business logic
- Authenticating users
- Validating data
- Communicating with the database

## Structure

The backend is split into three layers:

```
Controllers  →  Services  →  IssuePortalDbContext (EF Core)
```

| Folder | Contents |
|---|---|
| `Controllers/` | Thin controllers. They read the request, call a service, and map the result to an HTTP status code. |
| `Services/` | Business logic, validation, and database queries. |
| `Data/` | `IssuePortalDbContext`, which configures the relationships and delete rules. |
| `Models/` | Entities, request DTOs (`Create*Dto`/`Update*Dto`), response DTOs, and constants (`Roles`, `IssueStatuses`, `IssuePriorities`). |
| `Extensions/` | `User.ToCurrentUser()`, which reads the logged-in user from the JWT. |
| `Migrations/` | EF Core database migrations. |

## Security

- **Authentication:**
  - Users log in with email and password and receive a JWT, signed with HMAC-SHA256 and valid for 2 hours.
  - Passwords are hashed with ASP.NET Core's `PasswordHasher`.
- **Roles:**
  - There are three roles: `User`, `Developer`, and `Admin`.
  - Endpoints are restricted with `[Authorize(Roles = ...)]`.
- **Project access:**
  - `ProjectAccessService` checks that the user is a member of the project before they can read or write its data.
  - Admins can access all projects.
  - Data the user cannot access returns `404`, so its existence is not revealed.
- **Input:** write endpoints only accept request DTOs with validation attributes, never the database entities directly. This prevents over-posting, where a client sets fields like `Id`, `Role`, or `PasswordHash`.

See `ApiDesign.md` for all endpoints.

---

# 4. Database

## Technology

PostgreSQL

## Responsibilities

The database is responsible for:

- Storing application data
- Managing relationships between data
- Ensuring data consistency

See `DatabaseDesign.md` for the tables, relationships, and delete rules.

---

# 5. Main Entities

## User

Represents a person using the system.

Properties:

- Id
- Name
- Email
- PasswordHash
- Role (`User`, `Developer`, or `Admin`)
- CreatedAt


## Project

Represents a software project.

Properties:

- Id
- Name
- Description
- CreatedAt


## ProjectMember

Connects a user to a project.

Properties:

- Id
- ProjectId
- UserId
- JoinedAt


## Issue

Represents a task or bug.

Properties:

- Id
- Title
- Description
- Status (`Open`, `InProgress`, or `Closed`)
- Priority (`Low`, `Medium`, or `High`)
- CreatedAt
- UpdatedAt
- ProjectId
- AssignedUserId (optional)


## Comment

Represents a discussion message on an issue.

Properties:

- Id
- Content
- CreatedAt
- IssueId
- UserId (the author)


---

# 6. Relationships

## User - Project

- One user can participate in many projects, and one project can have many users.
- `ProjectMembers` connects them.


## Project - Issue

One project can contain many issues.


## Issue - Comment

One issue can contain many comments.


## User - Issue

- One user can be assigned many issues.
- An issue can be assigned to at most one user, who must be a member of the project.


## User - Comment

One user can write many comments.
