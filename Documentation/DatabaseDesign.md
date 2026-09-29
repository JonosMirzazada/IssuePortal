# IssuePortal - Database Design

## 1. Database Overview

IssuePortal uses PostgreSQL as its relational database, accessed through Entity Framework Core. The schema is managed with EF Core migrations in `Backend/IssuePortal.Api/Migrations/`.

The database stores users, projects, project memberships, issues, and comments.

Primary keys and foreign keys keep the relationships between tables consistent. Delete rules decide what happens to related rows when a row is deleted.

---

## 2. Users Table

| Column | Type | Description |
|---|---|---|
| Id | integer, PK | Unique identifier |
| Name | text, required | The user's name |
| Email | text, required | The user's email address, used to log in |
| PasswordHash | text, required | The hashed password (ASP.NET Core `PasswordHasher`). The plain password is never stored. |
| Role | text, required | `User` (default), `Developer`, or `Admin` |
| CreatedAt | timestamp | When the account was created |

The application checks that email addresses are unique, both on register and on user update.

---

## 3. Projects Table

| Column | Type | Description |
|---|---|---|
| Id | integer, PK | Unique identifier |
| Name | text, required | The project name. The API allows at most 200 characters. |
| Description | text, required | The project description. The API allows at most 2000 characters. |
| CreatedAt | timestamp | When the project was created |

---

## 4. ProjectMembers Table

`ProjectMembers` connects users and projects, which have a many-to-many relationship. Membership decides which projects, issues, and comments a non-admin user can access.

| Column | Type | Description |
|---|---|---|
| Id | integer, PK | Unique identifier |
| ProjectId | integer, FK → Projects.Id | The project |
| UserId | integer, FK → Users.Id | The user |
| JoinedAt | timestamp | When the user joined the project |

A **unique index on (ProjectId, UserId)** makes sure a user can only be a member of a project once.

---

## 5. Issues Table

| Column | Type | Description |
|---|---|---|
| Id | integer, PK | Unique identifier |
| Title | varchar(200), required | The issue title |
| Description | varchar(2000), required | The issue description |
| Status | text, required | `Open` (default), `InProgress`, or `Closed` |
| Priority | text, required | `Low`, `Medium` (default), or `High` |
| CreatedAt | timestamp | When the issue was created |
| UpdatedAt | timestamp | When the issue was last updated |
| ProjectId | integer, FK → Projects.Id | The project the issue belongs to |
| AssignedUserId | integer, FK → Users.Id, nullable | The user the issue is assigned to, or `NULL` if it is unassigned |

The API enforces the allowed values for `Status` and `Priority`. In the database, they are stored as plain text.

---

## 6. Comments Table

| Column | Type | Description |
|---|---|---|
| Id | integer, PK | Unique identifier |
| Content | varchar(1000), required | The comment text |
| CreatedAt | timestamp | When the comment was written |
| IssueId | integer, FK → Issues.Id | The issue the comment belongs to |
| UserId | integer, FK → Users.Id | The comment's author |

---

## 7. Relationships

### User - Project (many-to-many)

- A user can be a member of many projects, and a project can have many members.
- The `ProjectMembers` table connects them: `User 1 -> Many ProjectMembers` and `Project 1 -> Many ProjectMembers`.
- The user who creates a project becomes a member automatically.

### Project - Issues

- One project contains many issues, and each issue belongs to one project: `Project 1 -> Many Issues`.
- Foreign key: `Issues.ProjectId -> Projects.Id`

### Issue - Comments

- One issue can have many comments, and each comment belongs to one issue: `Issue 1 -> Many Comments`.
- Foreign key: `Comments.IssueId -> Issues.Id`

### User - Comments

- One user can write many comments, and each comment has one author: `User 1 -> Many Comments`.
- Foreign key: `Comments.UserId -> Users.Id`

### User - Issues (assignment)

- One user can be assigned many issues, and an issue is assigned to one user or none: `User 1 -> Many Issues`.
- Foreign key: `Issues.AssignedUserId -> Users.Id`, which is `NULL` when the issue is unassigned.
- The API only allows assigning an issue to a user who is a member of the issue's project.

---

## 8. Delete Rules

| When this is deleted | Related rows | Rule |
|---|---|---|
| Project | Its issues | **Cascade**: the issues are deleted, and so are their comments. |
| Project | Its memberships | **Cascade** |
| Issue | Its comments | **Cascade** |
| User | Their memberships | **Cascade** |
| User | Issues assigned to them | **Set null**: the issues stay, but become unassigned. |
| User | Comments they wrote | **Restrict**: the user cannot be deleted while they have comments, which preserves the discussion history. |

---

## 9. Primary Keys, Foreign Keys, and Indexes

**Primary keys:** `Users.Id`, `Projects.Id`, `ProjectMembers.Id`, `Issues.Id`, `Comments.Id`. They are all auto-incrementing integers (identity columns).

**Foreign keys:**

- `ProjectMembers.ProjectId -> Projects.Id`
- `ProjectMembers.UserId -> Users.Id`
- `Issues.ProjectId -> Projects.Id`
- `Issues.AssignedUserId -> Users.Id`
- `Comments.IssueId -> Issues.Id`
- `Comments.UserId -> Users.Id`

**Indexes:**

- Every foreign key column has an index.
- `ProjectMembers` also has a unique index on `(ProjectId, UserId)`.

---

## 10. Possible Future Changes

- `UpdatedAt` columns on Users, Projects, and Comments. Only Issues has one today.
- A unique index on `Users.Email`, so the database also enforces unique emails.
- A `CreatedByUserId` column on Issues to record who reported each issue.
- A table for file attachments on issues. File attachments are listed as a future feature in `Requirements.md`.
