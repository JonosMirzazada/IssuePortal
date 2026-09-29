# IssuePortal - Requirements Specification

## 1. Project Overview

IssuePortal is a web-based issue management system designed to help development teams organize, track, and resolve software issues.

The system allows users to create projects, report issues, assign tasks, communicate through comments, and track progress.


## 2. Problem Statement

Software development teams often need a structured way to manage bugs, tasks, and feature requests.

Without a centralized system, issues can be lost, communication becomes difficult, and project progress is harder to track.

IssuePortal provides one platform where teams can manage their work efficiently.

## 3. Goals

The main goals of IssuePortal are:

- Provide a simple way to manage software issues.
- Improve communication inside development teams.
- Track progress of tasks and bugs.
- Provide clear ownership of responsibilities.


## 4. User Roles

Status legend: ✅ implemented in the backend · ⏳ not implemented yet

Every user can only access the projects they are a member of. Admins can access all projects.

### Admin

Responsibilities:

- ✅ Manage users (list, update, delete)
- ✅ Create and manage projects, including their members
- ✅ Assign roles (`PUT /api/users/{id}/role`)
- ⏳ Manage system settings

### Developer

Responsibilities:

- ✅ View assigned issues (`GET /api/issues?assignedToMe=true`)
- ✅ Update issue status. Developers can update all fields of an issue.
- ✅ Add comments
- ⏳ Upload files (planned as the "File attachments" future feature)

### User

Responsibilities:

- ✅ Create issues
- ✅ View project information
- ✅ Add comments

Users cannot update issues. Only Developers and Admins can.


## 5. Core Features (MVP)

The backend implements all MVP features. The frontend has not been started yet.

### Authentication

Users can:

- ✅ Register an account
- ✅ Log in (JWT, valid for 2 hours)
- ✅ Log out. The client discards the token, so no backend endpoint is needed.


### Projects

Users can:

- ✅ Create projects. The creator becomes a member automatically.
- ✅ View projects they are a member of
- ✅ Manage project members (Admin)


### Issues

Users can:

- ✅ Create issues
- ✅ Update issues (Developer, Admin)
- ✅ Delete issues (Admin)
- ✅ Assign issues to a member of the project
- ✅ Change issue status: `Open`, `InProgress`, or `Closed` (Developer, Admin)
- ✅ Filter issues by project, status, priority, and assignee


### Comments

Users can:

- ✅ Add comments
- ✅ View discussion history. `GET /api/issues/{id}` includes the comments.

## 6. Future Features

- Real-time notifications
- Email notifications
- File attachments
- Dashboard analytics
- Advanced search
- Dark mode
- Mobile application
