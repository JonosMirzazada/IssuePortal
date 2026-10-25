import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router'   // CHANGED: added Link
import { getProject, createIssue, type ProjectDetails } from '../api'

function ProjectDetailPage() {
  const { id } = useParams()
  const [project, setProject] = useState<ProjectDetails | null>(null)
  const [error, setError] = useState("")

  const [title, setTitle] = useState("")
  const [description, setDescription] = useState("")
  const [priority, setPriority] = useState("Medium")
  const [formError, setFormError] = useState("")

  useEffect(() => {
    if (!id) return

    getProject(id)
      .then(data => setProject(data))
      .catch(() => setError("Could not load the project."))
  }, [id])

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

  if (error) {
    return <p className="text-red-600">{error}</p>
  }

  if (!project) {
    return <p className="text-gray-600">Loading...</p>
  }

  return (
    <div>
      <h1 className="text-3xl font-bold">{project.name}</h1>
      <p className="mt-2 text-gray-600">{project.description}</p>

      <form onSubmit={handleSubmit} className="mt-8 flex max-w-sm flex-col gap-3">
        <h2 className="text-xl font-semibold">New issue</h2>
        <input
          type="text"
          placeholder="Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="rounded border border-gray-300 px-3 py-2"
        />
        <input
          type="text"
          placeholder="Description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="rounded border border-gray-300 px-3 py-2"
        />
        <select
          value={priority}
          onChange={(e) => setPriority(e.target.value)}
          className="rounded border border-gray-300 px-3 py-2"
        >
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
        </select>

        {formError && <p className="text-sm text-red-600">{formError}</p>}

        <button
          type="submit"
          className="rounded bg-blue-600 px-4 py-2 font-medium text-white hover:bg-blue-700"
        >
          Create issue
        </button>
      </form>

      <h2 className="mt-8 text-xl font-semibold">Issues</h2>

      {project.issues.length === 0 && (
        <p className="mt-2 text-gray-600">No issues in this project yet.</p>
      )}

      <ul className="mt-4 flex flex-col gap-2">
        {project.issues.map(issue => (
          <li key={issue.id}>
            {/* CHANGED: the whole box is now a link to the issue page */}
            <Link
              to={`/issues/${issue.id}`}
              className="block rounded border border-gray-300 p-4 hover:bg-gray-50"
            >
              <p className="font-medium">{issue.title}</p>
              <p className="text-sm text-gray-600">
                {issue.status} · {issue.priority}
              </p>
            </Link>
          </li>
        ))}
      </ul>
    </div>
  )
}

export default ProjectDetailPage