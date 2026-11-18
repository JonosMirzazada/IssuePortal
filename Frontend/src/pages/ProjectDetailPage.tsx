import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router'
import {
  getProject,
  createIssue,
  getProjectMembers,
  addProjectMember,
  getUsers,
  getCurrentUserRole,
  type ProjectDetails,
  type ProjectMember,
  type User,
} from '../api'
import { StatusBadge, PriorityBadge } from '../components/Badges'   // NEW

function ProjectDetailPage() {
  const { id } = useParams()
  const [project, setProject] = useState<ProjectDetails | null>(null)
  const [error, setError] = useState("")

  const [title, setTitle] = useState("")
  const [description, setDescription] = useState("")
  const [priority, setPriority] = useState("Medium")
  const [formError, setFormError] = useState("")

  const [members, setMembers] = useState<ProjectMember[]>([])
  const [users, setUsers] = useState<User[]>([])
  const [selectedUserId, setSelectedUserId] = useState("")
  const [memberError, setMemberError] = useState("")

  const isAdmin = getCurrentUserRole() === "Admin"

  useEffect(() => {
    if (!id) return

    getProject(id)
      .then(data => setProject(data))
      .catch(() => setError("Could not load the project."))

    getProjectMembers(Number(id))
      .then(data => setMembers(data))
      .catch(() => setMemberError("Could not load the members."))

    if (isAdmin) {
      getUsers()
        .then(data => setUsers(data))
        .catch(() => setMemberError("Could not load users."))
    }
  }, [id, isAdmin])

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setFormError("")
    if (!project) return

    try {
      const newIssue = await createIssue(project.id, title, description, priority)
      setProject({ ...project, issues: [...project.issues, newIssue] })
      setTitle("")
      setDescription("")
      setPriority("Medium")
    } catch {
      setFormError("Could not create the issue.")
    }
  }

  async function handleAddMember(e: React.FormEvent) {
    e.preventDefault()
    setMemberError("")
    if (!project || selectedUserId === "") return

    try {
      const newMember = await addProjectMember(project.id, Number(selectedUserId))
      setMembers([...members, newMember])
      setSelectedUserId("")
    } catch (err) {
      setMemberError(err instanceof Error ? err.message : "Could not add the member.")
    }
  }

  if (error) {
    return <p className="text-red-600">{error}</p>
  }

  if (!project) {
    return <p className="text-gray-600">Loading...</p>
  }

  const nonMembers = users.filter(user => !members.some(m => m.userId === user.id))

  return (
    <div>
      {/* Back link + title */}
      <Link to="/projects" className="text-sm text-blue-600 hover:underline">
        ← All projects
      </Link>
      <h1 className="mt-3 text-3xl font-bold text-gray-900">{project.name}</h1>
      <p className="mt-1 text-gray-600">{project.description || "No description"}</p>

      {/* Two columns on big screens: issues (wide) + sidebar (narrow) */}
      <div className="mt-8 grid gap-6 lg:grid-cols-3">

        {/* LEFT: issues */}
        <div className="flex flex-col gap-6 lg:col-span-2">

          {/* New issue card */}
          <form
            onSubmit={handleSubmit}
            className="rounded-lg border border-gray-200 bg-white p-5 shadow-sm"
          >
            <h2 className="text-lg font-semibold text-gray-900">New issue</h2>

            <div className="mt-3 flex flex-col gap-3">
              <input
                type="text"
                placeholder="Title"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                className="rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:outline-none"
              />
              <textarea
                placeholder="Description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                rows={2}
                className="rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:outline-none"
              />
              <div className="flex gap-3">
                <select
                  value={priority}
                  onChange={(e) => setPriority(e.target.value)}
                  className="rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:outline-none"
                >
                  <option value="Low">Low priority</option>
                  <option value="Medium">Medium priority</option>
                  <option value="High">High priority</option>
                </select>
                <button
                  type="submit"
                  className="rounded-md bg-blue-600 px-4 py-2 font-medium text-white hover:bg-blue-700"
                >
                  Create issue
                </button>
              </div>
            </div>

            {formError && <p className="mt-3 text-sm text-red-600">{formError}</p>}
          </form>

          {/* Issue list */}
          <div>
            <h2 className="text-lg font-semibold text-gray-900">
              Issues <span className="text-gray-500">({project.issues.length})</span>
            </h2>

            {project.issues.length === 0 ? (
              <p className="mt-3 text-gray-600">No issues in this project yet.</p>
            ) : (
              <ul className="mt-3 flex flex-col gap-2">
                {project.issues.map(issue => (
                  <li key={issue.id}>
                    <Link
                      to={`/issues/${issue.id}`}
                      className="flex items-center justify-between gap-4 rounded-lg border border-gray-200 bg-white p-4 shadow-sm transition hover:border-blue-300 hover:shadow-md"
                    >
                      <span className="font-medium text-gray-900">{issue.title}</span>
                      <span className="flex shrink-0 gap-2">
                        <StatusBadge status={issue.status} />
                        <PriorityBadge priority={issue.priority} />
                      </span>
                    </Link>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </div>

        {/* RIGHT: members sidebar */}
        <aside className="h-fit rounded-lg border border-gray-200 bg-white p-5 shadow-sm">
          <h2 className="text-lg font-semibold text-gray-900">
            Members <span className="text-gray-500">({members.length})</span>
          </h2>

          <ul className="mt-3 flex flex-col gap-3">
            {members.map(member => (
              <li key={member.userId} className="flex items-center gap-3">
                <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-blue-100 text-sm font-semibold text-blue-800">
                  {member.userName.charAt(0).toUpperCase()}
                </span>
                <span className="min-w-0">
                  <span className="block truncate text-sm font-medium text-gray-900">{member.userName}</span>
                  <span className="block truncate text-xs text-gray-500">{member.email}</span>
                </span>
              </li>
            ))}
          </ul>

          {isAdmin && (
            <form onSubmit={handleAddMember} className="mt-4 flex flex-col gap-2 border-t border-gray-200 pt-4">
              <select
                value={selectedUserId}
                onChange={(e) => setSelectedUserId(e.target.value)}
                className="rounded-md border border-gray-300 px-2 py-1.5 text-sm focus:border-blue-500 focus:outline-none"
              >
                <option value="">Choose a user...</option>
                {nonMembers.map(user => (
                  <option key={user.id} value={user.id}>
                    {user.name} ({user.email})
                  </option>
                ))}
              </select>
              <button
                type="submit"
                className="rounded-md bg-blue-600 px-3 py-1.5 text-sm font-medium text-white hover:bg-blue-700"
              >
                Add member
              </button>
            </form>
          )}

          {memberError && <p className="mt-2 text-sm text-red-600">{memberError}</p>}
        </aside>
      </div>
    </div>
  )
}

export default ProjectDetailPage