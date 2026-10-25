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