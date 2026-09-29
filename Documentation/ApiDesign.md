# IssuePortal - API Design

## 1. API Overview

The IssuePortal backend is a REST API built with ASP.NET Core Web API (.NET 10). It uses PostgreSQL through Entity Framework Core.

- Base URL (development): `http://localhost:5126`
- All endpoints are under `/api`.
- Interactive documentation: Swagger UI at `http://localhost:5126/swagger`. It is only available in the Development environment.
- The API sends and receives JSON, and property names are camelCase.

---

## 2. Authentication

The API uses **JWT bearer tokens**.

1. Register with `POST /api/auth/register`.
2. Log in with `POST /api/auth/login` to receive a token.
3. Send the token in the `Authorization` header of every other request:

```
Authorization: Bearer <token>
```

- A token is valid for **2 hours**.
- Logout is handled by the client, which discards the token. There is no logout endpoint.
- The token contains the user's id, name, email, and role. **After a user's role changes, they must log in again** to get a token with the new role.

In Swagger UI, click **Authorize** and paste the token without the word `Bearer`.

---

## 3. Roles and Access Rules

| Role | Description |
|---|---|
| `User` | The default role for new accounts. Can create projects and issues, add comments, and view their projects. |
| `Developer` | Everything a User can do, plus updating issues (status, assignee, and other fields). |
| `Admin` | Full access to all projects, plus managing users, roles, and project members, and updating or deleting projects, issues, and comments. |

### Project membership

Non-admins can only see and work with projects **they are a member of**, together with the issues and comments in those projects. Admins can access everything.

- The user who creates a project becomes a member automatically.
- Admins add and remove members.
- A project, issue, or comment the user cannot access is treated **as if it does not exist**. Reads return `404`, and creates return `400` with a "does not exist" message. This way, non-members cannot find out which IDs exist.
- An issue can only be assigned to a user who is a member of the issue's project.

---

## 4. Responses and Errors

| Status | Meaning |
|---|---|
| `200 OK` | The request succeeded, and the response body contains the result. |
| `201 Created` | The resource was created. The `Location` header points to it. |
| `204 No Content` | The delete succeeded. |
| `400 Bad Request` | Validation failed. See the error formats below. |
| `401 Unauthorized` | The token is missing, invalid, or expired, or the login failed. |
| `403 Forbidden` | The user is logged in, but their role is not allowed to do this. |
| `404 Not Found` | The resource does not exist, or the user cannot access it. |
| `405 Method Not Allowed` | The route exists, but not for this HTTP method. |

There are three error formats:

- **Model validation errors**, such as a missing field, a field that is too long, or an invalid `status`, use the standard ASP.NET problem format:
  ```json
  {
    "title": "One or more validation errors occurred.",
    "status": 400,
    "errors": { "Status": ["The Status field does not equal any of the values specified in AllowedValuesAttribute."] }
  }
  ```
- **Business rule errors**, such as "Project with ID 5 does not exist.", are a plain text string.
- **Auth errors** from register and login are an object: `{ "message": "Email already exists." }`.

---

## 5. Endpoints

### 5.1 Auth

Anyone can call these endpoints; no token is needed.

| Method | Route | Description |
|---|---|---|
| POST | `/api/auth/register` | Create an account with the role `User`. |
| POST | `/api/auth/login` | Log in and receive a JWT. |

**Register**

Request:
```json
{ "name": "Jonos", "email": "jonos@example.com", "password": "Test1234!" }
```

Response `200`:
```json
{ "id": 1, "name": "Jonos", "email": "jonos@example.com", "role": "User", "createdAt": "2026-09-29T16:00:00Z", "assignedIssues": [] }
```

If the email is already registered, the response is `400` with `{ "message": "Email already exists." }`.

**Login**

Request:
```json
{ "email": "jonos@example.com", "password": "Test1234!" }
```

Response `200`:
```json
{ "token": "eyJhbGciOi..." }
```

If the email or password is wrong, the response is `401` with `{ "message": "Invalid email or password." }`.

---

### 5.2 Projects

All project endpoints require a token.

| Method | Route | Who | Description |
|---|---|---|---|
| GET | `/api/projects` | Any user | List the projects you can access. |
| GET | `/api/projects/{id}` | Member or Admin | Get one project, including a short summary of its issues. |
| POST | `/api/projects` | Any user | Create a project. You become a member of it. |
| PUT | `/api/projects/{id}` | Admin | Update a project's name and description. |
| DELETE | `/api/projects/{id}` | Admin | Delete a project. Its issues, comments, and memberships are deleted too. |
| GET | `/api/projects/{id}/members` | Member or Admin | List the project's members. |
| POST | `/api/projects/{id}/members` | Admin | Add a member. |
| DELETE | `/api/projects/{id}/members/{userId}` | Admin | Remove a member. |

**Create or update a project**

Request:
```json
{ "name": "Webshop", "description": "Customer-facing webshop" }
```

- `name` is required and can be at most 200 characters.
- `description` can be at most 2000 characters.

Response:
```json
{ "id": 3, "name": "Webshop", "description": "Customer-facing webshop", "createdAt": "2026-09-29T16:00:00Z", "issues": [] }
```

`GET /api/projects/{id}` returns the same shape, and `issues` contains `{ id, title, status, priority }` for each issue in the project.

In the list returned by `GET /api/projects`, the `issues` and `members` arrays are always empty. To get a project's issues, use `GET /api/projects/{id}` or `GET /api/issues?projectId={id}`.

**Add a member**

Request:
```json
{ "userId": 5 }
```

Response `201`:
```json
{ "userId": 5, "userName": "Anna", "email": "anna@example.com", "joinedAt": "2026-09-29T16:00:00Z" }
```

If the user does not exist or is already a member, the response is `400`. If the project does not exist, it is `404`.

`GET /api/projects/{id}/members` returns a list in the same shape, ordered by `joinedAt`.

---

### 5.3 Issues

All issue endpoints require a token.

| Method | Route | Who | Description |
|---|---|---|---|
| GET | `/api/issues` | Any user | List issues in your projects. Supports filters, see below. |
| GET | `/api/issues/{id}` | Member or Admin | Get one issue with its comments. |
| POST | `/api/issues` | Member or Admin | Create an issue in a project you are a member of. |
| PUT | `/api/issues/{id}` | Developer or Admin | Update an issue. You must be a member of both its current project and the new `projectId`. |
| DELETE | `/api/issues/{id}` | Admin | Delete an issue. Its comments are deleted too. |

**Filters for `GET /api/issues`**

All filters are optional, and when you combine them an issue has to match all of them.

| Query parameter | Example | Description |
|---|---|---|
| `projectId` | `?projectId=3` | Only issues in this project. |
| `status` | `?status=Open` | Only issues with this status. |
| `priority` | `?priority=High` | Only issues with this priority. |
| `assignedUserId` | `?assignedUserId=5` | Only issues assigned to this user. |
| `assignedToMe` | `?assignedToMe=true` | Only issues assigned to you. This overrides `assignedUserId`. |

Results are ordered by `updatedAt`, newest first. In the list, `comments` is always an empty array; use `GET /api/issues/{id}` to get the comments.

**Create or update an issue**

Request:
```json
{
  "title": "Login does not work",
  "description": "Users get a 500 error when logging in.",
  "status": "Open",
  "priority": "High",
  "projectId": 3,
  "assignedUserId": 5
}
```

| Field | Rules |
|---|---|
| `title` | Required, at most 200 characters. |
| `description` | Required, at most 2000 characters. |
| `status` | `Open`, `InProgress`, or `Closed`. Defaults to `Open`. |
| `priority` | `Low`, `Medium`, or `High`. Defaults to `Medium`. |
| `projectId` | A project you are a member of. |
| `assignedUserId` | Optional. Must be a member of the project. |

Response (`201` for create, `200` for update):
```json
{
  "id": 7,
  "title": "Login does not work",
  "description": "Users get a 500 error when logging in.",
  "status": "Open",
  "priority": "High",
  "createdAt": "2026-09-29T16:00:00Z",
  "updatedAt": "2026-09-29T16:00:00Z",
  "projectId": 3,
  "assignedUserId": 5,
  "comments": []
}
```

`GET /api/issues/{id}` returns the same shape, and `comments` contains the issue's comments. See 5.4 for the comment format.

---

### 5.4 Comments

All comment endpoints require a token.

| Method | Route | Who | Description |
|---|---|---|---|
| GET | `/api/comments` | Any user | List comments on issues in your projects. |
| GET | `/api/comments/{id}` | Member or Admin | Get one comment. |
| POST | `/api/comments` | Member or Admin | Add a comment to an issue. |
| PUT | `/api/comments/{id}` | Admin | Edit a comment's text. |
| DELETE | `/api/comments/{id}` | Admin | Delete a comment. |

**Create a comment**

Request:
```json
{ "issueId": 7, "content": "I can reproduce this in Firefox." }
```

- `content` is required and can be at most 1000 characters.
- The author is always the logged-in user. It is not sent in the request.

For an update, the request contains only `{ "content": "..." }`.

Response:
```json
{
  "id": 12,
  "content": "I can reproduce this in Firefox.",
  "createdAt": "2026-09-29T16:00:00Z",
  "issueId": 7,
  "issueTitle": "Login does not work",
  "userId": 1,
  "userName": "Jonos"
}
```

---

### 5.5 Users

All user endpoints require a token with the **Admin** role.

| Method | Route | Description |
|---|---|---|
| GET | `/api/users` | List all users. |
| GET | `/api/users/{id}` | Get one user. |
| PUT | `/api/users/{id}` | Update a user's name and email. |
| PUT | `/api/users/{id}/role` | Change a user's role. |
| DELETE | `/api/users/{id}` | Delete a user. |

New users are created through `POST /api/auth/register`. There is no `POST /api/users`.

**User response**
```json
{
  "id": 5,
  "name": "Anna",
  "email": "anna@example.com",
  "role": "Developer",
  "createdAt": "2026-09-29T16:00:00Z",
  "assignedIssues": [ { "id": 7, "title": "Login does not work", "status": "Open", "priority": "High" } ]
}
```

**Update a user**

Request:
```json
{ "name": "Anna Svensson", "email": "anna@example.com" }
```

- Both fields are required, and `email` must be a valid email address.
- If the email is already in use, the response is `400`.
- A user's role and password cannot be changed through this endpoint.

**Change a role**

Request:
```json
{ "role": "Developer" }
```

- `role` must be `User`, `Developer`, or `Admin`.
- Admins cannot change their own role, so they cannot lock themselves out.
- The change takes effect the next time the user logs in.

---

## 6. CORS

Browsers only allow the frontend to call the API from the origins listed in `Cors:AllowedOrigins`. In development, this is set in `appsettings.Development.json`:

```json
"Cors": { "AllowedOrigins": [ "http://localhost:5173" ] }
```

`http://localhost:5173` is the Vite dev server. When the frontend is deployed, add its URL to this list.

---

## 7. Known Limitations

- **The first admin** has to be set directly in the database:
  ```sql
  UPDATE "Users" SET "Role" = 'Admin' WHERE "Email" = 'you@example.com';
  ```
- **Deleting a user who has written comments fails with a server error (`500`).** The database blocks it to preserve the comment history, and the API does not yet turn that into a clear `400`.
- **Registration input is not validated.** For example, there is no minimum password length and no email format check.
- **Pagination is not implemented.** List endpoints return all matching rows.
