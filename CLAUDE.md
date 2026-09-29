# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

IssuePortal is a work-in-progress issue tracker (projects, issues, comments, users). Only the backend exists so far: an ASP.NET Core Web API (.NET 10) on PostgreSQL via EF Core, in `Backend/IssuePortal.Api/`. A React + TypeScript + Tailwind frontend is planned (see `Documentation/SystemDesign.md`). The README and parts of `Documentation/` are written in Swedish.

`Documentation/` holds the intended design (requirements, API, DB, system). It describes more than has been built (for example file uploads and notifications).

## Commands

Run all commands from `Backend/IssuePortal.Api/`. There is no solution file and no test project yet.

```bash
dotnet build
dotnet run                          # http://localhost:5126; "/" redirects to /swagger (Development only)
dotnet run --launch-profile https   # https://localhost:7118

# EF Core migrations (requires the dotnet-ef tool)
dotnet ef migrations add <Name>
dotnet ef database update
```

`IssuePortal.Api.http` contains sample requests. Many of them predate authentication, so they now need a bearer token.

## Configuration

- The connection string `DefaultConnection` in `appsettings.Development.json` points to local Postgres (`localhost:5432`, DB `IssuePortalDb`, user `postgres`) with no password. Supply the password and `Jwt:Key` through user secrets (the project has a `UserSecretsId`), e.g. `dotnet user-secrets set "Jwt:Key" "<key>"`. The app throws on startup if `Jwt:Key` is missing.
- At startup, `Program.cs` only prints whether it could connect to the DB. It does not apply migrations.
- CORS uses the `Frontend` policy. Its allowed origins come from `Cors:AllowedOrigins`, which is set to the Vite dev server `http://localhost:5173` in `appsettings.Development.json`. Add the production frontend origin there, or in the environment, when you deploy.

## Architecture

The code is layered as Controller → Service → `IssuePortalDbContext`:

- **Controllers** (`Controllers/`) are thin. They map service results to HTTP status codes. Services are concrete classes (no interfaces), registered as scoped in `Program.cs`. Add new services there.
- **Service result conventions** vary by service:
  - `IssueService` returns `(Entity? Issue, string? Error)` tuples. `(null, null)` means not found (404), and `(null, error)` means a validation failure (400), such as a missing project or assigned user.
  - `AuthService` throws `InvalidOperationException`, which `AuthController` catches.
  - Delete methods return `bool`.
- **Models vs DTOs**: `Models/` contains both entities and DTOs. Read endpoints project entities into DTOs (`IssueDto`, `CommentDto`, `ProjectDto`, `UserDto`, …) to avoid navigation cycles. Write endpoints for issues, projects, and comments bind request DTOs (`Create*Dto`/`Update*Dto`) that carry validation attributes. Issue `Status`/`Priority` are strings restricted to the constants in `Models/IssueValues.cs`. A comment's author comes from the JWT `NameIdentifier` claim, not the request body. `RegisterDto`/`LoginDto` use namespace `IssuePortal.Api.DTOs`, while all other models use `IssuePortal.Api.Models`.
- **Relationships** are configured in `IssuePortalDbContext.OnModelCreating`:
  - Deleting an Issue cascades to its Comments.
  - A User with comments cannot be deleted (Restrict).
  - Deleting a user sets `Issue.AssignedUserId` to null.
  - `Issue → Project` uses the `ProjectId` convention.
  - `ProjectMember` links users and projects (many-to-many), with a unique index on (ProjectId, UserId). Deleting a project or a user cascades to its memberships. The user who creates a project becomes a member automatically. Members are managed through `/api/projects/{id}/members` (listing requires authentication; adding and removing require Admin).
- **Auth**: JWT bearer (HMAC-SHA256, 2h lifetime; issuer and audience are not validated). `TokenService` puts `NameIdentifier`, `Name`, `Email`, and `Role` claims in the token. Passwords are hashed with `PasswordHasher<User>`. Roles are `User` (default), `Developer`, and `Admin` (constants in `Models/Roles.cs`). Admins change roles through `PUT /api/users/{id}/role`, but not their own role. Because the role is stored in the token, a role change only takes effect after the user logs in again. The first admin still has to be set directly in the DB.
- **Authorization** is set with attributes on each controller:
  - `AuthController` (`/api/auth/register`, `/api/auth/login`) is anonymous.
  - Issues, Projects, and Comments require an authenticated user, and update/delete endpoints additionally require the `Admin` role. The exception is Issue update, which requires `Developer` or `Admin`. Plain Users can create issues and comment, but not update issues.
  - `UsersController` is Admin-only. Users are created only through `/api/auth/register`; there is no `POST /api/users`. `PUT /api/users/{id}` changes only name and email, and roles change through `PUT /api/users/{id}/role`.
- **Per-project access**: non-admins can only see and change projects, issues, and comments in projects they are a member of. Admins can access everything. `ProjectAccessService` holds the checks. Controllers pass `User.ToCurrentUser()` (see `Extensions/ClaimsPrincipalExtensions.cs`) into the services. Inaccessible resources look the same as missing ones: 404 on reads, and on create the same "does not exist" error as a missing ID, so non-members can't discover IDs. An issue's `AssignedUserId` must be a member of the issue's project.
- **Issue filtering**: `GET /api/issues` takes optional query parameters from `IssueQueryDto`: `projectId`, `status`, `priority`, `assignedUserId`, and `assignedToMe=true`. They combine with AND, are applied after the membership filter, and results are ordered by `UpdatedAt` descending.
- Swagger is configured with a Bearer security scheme, so you can authorize from the Swagger UI with a token from `/api/auth/login`.
