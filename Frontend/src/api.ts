const API_URL = "http://localhost:5126"

export async function login(email: string, password: string): Promise<string> {
  const response = await fetch(`${API_URL}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  })

  if (!response.ok) {
    throw new Error("Invalid email or password.")
  }

  const data = await response.json()
  return data.token
}


export type Project = {
  id: number
  name: string
  description: string
  createdAt: string
}

export async function getProjects(): Promise<Project[]> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/projects`, {
    headers: { "Authorization": `Bearer ${token}` },
  })

  if (!response.ok) {
    throw new Error("Could not load projects.")
  }

  return await response.json()
}

export async function createProject(name: string, description: string): Promise<Project> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/projects`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`,
    },
    body: JSON.stringify({ name, description }),
  })

  if (!response.ok) {
    throw new Error("Could not create the project.")
  }

  return await response.json()
}
export async function register(name: string, email: string, password: string): Promise<void> {
  const response = await fetch(`${API_URL}/api/auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name, email, password }),
  })

  if (!response.ok) {
    const data = await response.json()
    throw new Error(data.message)
  }
}

export type ProjectIssue = {
  id: number
  title: string
  status: string
  priority: string
}

export type ProjectDetails = Project & {
  issues: ProjectIssue[]
}

export async function getProject(id: string): Promise<ProjectDetails> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/projects/${id}`, {
    headers: { "Authorization": `Bearer ${token}` },
  })

  if (!response.ok) {
    throw new Error("Could not load the project.")
  }

  return await response.json()
}

export async function createIssue(
  projectId: number,
  title: string,
  description: string,
  priority: string
): Promise<ProjectIssue> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/issues`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`,
    },
    body: JSON.stringify({ projectId, title, description, priority }),
  })

  if (!response.ok) {
    throw new Error("Could not create the issue.")
  }

  return await response.json()
}
export type Comment = {
  id: number
  content: string
  createdAt: string
  userName: string
}

export type IssueDetails = {
  id: number
  title: string
  description: string
  status: string
  priority: string
  projectId: number
  assignedUserId: number | null
  comments: Comment[]
}

export async function getIssue(id: string): Promise<IssueDetails> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/issues/${id}`, {
    headers: { "Authorization": `Bearer ${token}` },
  })

  if (!response.ok) {
    throw new Error("Could not load the issue.")
  }

  return await response.json()
}

export async function createComment(issueId: number, content: string): Promise<Comment> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/comments`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`,
    },
    body: JSON.stringify({ issueId, content }),
  })

  if (!response.ok) {
    throw new Error("Could not add the comment.")
  }

  return await response.json()
}

export async function updateIssueStatus(issue: IssueDetails, status: string): Promise<void> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/issues/${issue.id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`,
    },
    body: JSON.stringify({
      title: issue.title,
      description: issue.description,
      status: status,
      priority: issue.priority,
      projectId: issue.projectId,
      assignedUserId: issue.assignedUserId,
    }),
  })  

  if (!response.ok) {
    throw new Error(`Could not update the status (${response.status}).`)
  }
}

export async function getMyIssues(): Promise<ProjectIssue[]> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/issues?assignedToMe=true`, {
    headers: { "Authorization": `Bearer ${token}` },
  })

  if (!response.ok) {
    throw new Error("Could not load your issues.")
  }

  return await response.json()
}
function getTokenData(): Record<string, string> | null {
  const token = localStorage.getItem("token")
  if (!token) return null

  const payload = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")
  return JSON.parse(atob(payload))
}

export function getCurrentUserId(): number | null {
  const data = getTokenData()
  if (!data) return null
  return Number(data["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"])
}


export function getCurrentUserRole(): string | null {
  const data = getTokenData()
  if (!data) return null
  return data["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
}

export async function assignIssue(issue: IssueDetails, userId: number | null): Promise<void> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/issues/${issue.id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`,
    },
    body: JSON.stringify({
      title: issue.title,
      description: issue.description,
      status: issue.status,
      priority: issue.priority,
      projectId: issue.projectId,
      assignedUserId: userId,
    }),
  })

  if (!response.ok) {
    throw new Error(`Could not assign the issue (${response.status}).`)
  }
}

export type User = {
  id: number
  name: string
  email: string
  role: string
}

export async function getUsers(): Promise<User[]> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/users`, {
    headers: { "Authorization": `Bearer ${token}` },
  })

  if (!response.ok) {
    throw new Error(`Could not load users (${response.status}).`)
  }

  return await response.json()
}

export async function updateUserRole(userId: number, role: string): Promise<void> {
  const token = localStorage.getItem("token")

  const response = await fetch(`${API_URL}/api/users/${userId}/role`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      "Authorization": `Bearer ${token}`,
    },
    body: JSON.stringify({ role }),
  })

  if (!response.ok) {
    throw new Error(`Could not change the role (${response.status}).`)
  }
}
