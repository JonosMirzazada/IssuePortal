# IssuePortal - Database Design

## 1. Database Overview

IssuePortal will use PostgreSQL as its relational database.

The database will store and manage users, projects, project memberships, issues, and comments.

The database will use primary keys and foreign keys to maintain relationships between entities and ensure data consistency.

---

## 2. Users Table

| Column | Description |
|---|---|
| Id | Unique identifier |
| Name | User's name |
| Email | User's email address |
| PasswordHash | Hashed user password |
| Role | User role |
| CreatedAt | Account creation date |
| UpdatedAt | Date when the user information was last updated |

---

## 3. Projects Table

| Column | Description |
|---|---|
| Id | Unique identifier |
| Name | Project name |
| Description | Project description |
| CreatedAt | Project creation date |
| UpdatedAt | Date when the project information was last updated |

---

## 4. ProjectMembers Table

The `ProjectMembers` table represents the membership relationship between users and projects.

| Column | Description |
|---|---|
| Id | Unique identifier |
| ProjectId | Related project |
| UserId | Related user |
| JoinedAt | Date when the user joined the project |

---

## 5. Issues Table

| Column | Description |
|---|---|
| Id | Unique identifier |
| Title | Issue title |
| Description | Issue description |
| Status | Current issue status |
| Priority | Issue priority |
| CreatedAt | Issue creation date |
| UpdatedAt | Date when the issue was last updated |
| ProjectId | Related project |
| AssignedUserId | User assigned to the issue; can be null if no user has been assigned |

---

## 6. Comments Table

| Column | Description |
|---|---|
| Id | Unique identifier |
| Message | Comment text |
| CreatedAt | Comment creation date |
| UpdatedAt | Date when the comment was last updated |
| IssueId | Related issue |
| UserId | Comment author |

---

## 7. Relationships

### User - Project

Users and Projects have a many-to-many relationship.

A user can participate in many projects.

A project can have many users.

The `ProjectMembers` table is used to connect users and projects.

**Relationship:**

`User 1 -> Many ProjectMembers`

`Project 1 -> Many ProjectMembers`

---

### User - Comments

One user can create many comments.

Each comment belongs to one user.

**Relationship:**

`User 1 -> Many Comments`

**Foreign Key:**

`Comments.UserId -> Users.Id`

---

### Project - Issues

One project can contain many issues.

Each issue belongs to one project.

**Relationship:**

`Project 1 -> Many Issues`

**Foreign Key:**

`Issues.ProjectId -> Projects.Id`

---

### Issue - Comments

One issue can contain many comments.

Each comment belongs to one issue.

**Relationship:**

`Issue 1 -> Many Comments`

**Foreign Key:**

`Comments.IssueId -> Issues.Id`

---

### User - Issues

One user can be assigned many issues.

An issue can be assigned to one user or remain unassigned.

**Relationship:**

`User 1 -> Many Issues`

**Foreign Key:**

`Issues.AssignedUserId -> Users.Id`

`AssignedUserId` can be `NULL` when an issue has not yet been assigned.

---

## 8. Primary Keys and Foreign Keys

### Primary Keys

Each table has an `Id` column that uniquely identifies each record.

**Primary keys:**

- `Users.Id`
- `Projects.Id`
- `ProjectMembers.Id`
- `Issues.Id`
- `Comments.Id`

### Foreign Keys

Foreign keys are used to connect related records between tables.

- `ProjectMembers.ProjectId -> Projects.Id`
- `ProjectMembers.UserId -> Users.Id`
- `Issues.ProjectId -> Projects.Id`
- `Issues.AssignedUserId -> Users.Id`
- `Comments.IssueId -> Issues.Id`
- `Comments.UserId -> Users.Id`

---

## 9. Database Design Notes

- Each table uses a primary key to uniquely identify records.
- Foreign keys are used to maintain relationships between tables.
- `ProjectMembers` resolves the many-to-many relationship between users and projects.
- `AssignedUserId` can be `NULL` because an issue does not have to be assigned immediately.
- `CreatedAt` and `UpdatedAt` fields help track when records are created and modified.
- The design is intended to maintain data consistency and provide a clear structure for the IssuePortal application.