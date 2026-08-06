# IssuePortal - System Design

# 1. System Architecture

IssuePortal follows a three-layer architecture:

1. Frontend
2. Backend
3. Database

---

# 2. Frontend

## Technology

React + TypeScript + Tailwind CSS

## Responsibilities

The frontend is responsible for:

- Providing the user interface
- Handling user interactions
- Sending requests to the backend API
- Displaying information received from the backend



---

# 3. Backend

## Technology

C# + ASP.NET Core Web API

## Responsibilities

The backend is responsible for:

- Handling HTTP requests
- Implementing business logic
- Authenticating users
- Validating data
- Communicating with the database

---

# 4. Database

## Technology

PostgreSQL

## Responsibilities

The database is responsible for:

- Storing application data
- Managing relationships between data
- Ensuring data consistency


---

# 5. Main Entities

## User

Represents a person using the system.

Properties:

- Id
- Name
- Email
- Password
- Role


## Project

Represents a software project.

Properties:

- Id
- Name
- Description
- CreatedDate


## Issue

Represents a task or bug.

Properties:

- Id
- Title
- Description
- Status
- Priority
- CreatedDate


## Comment

Represents a discussion message.

Properties:

- Id
- Message
- CreatedDate


---

# 6. Relationships

## User - Project

One user can participate in many projects.


## Project - Issue

One project can contain many issues.


## Issue - Comment

One issue can contain many comments.