import { useEffect, useState } from 'react'
import { Link } from 'react-router'
import { getProjects, createProject, type Project } from '../api'

function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([])
  const [loading, setLoading] = useState(true)
  const [name, setName] = useState("")
  const [description, setDescription] = useState("")
  const [error, setError] = useState("")

  useEffect(() => {
    getProjects()
      .then(data => setProjects(data))
      .catch(() => setError("Could not load projects."))
      .finally(() => setLoading(false))
  }, [])

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError("")

    try {
      const newProject = await createProject(name, description)
      setProjects([...projects, newProject])
      setName("")
      setDescription("")
    } catch {
      setError("Could not create the project.")
    }
  }

  return (
    <div>
      {/* Page title */}
      <h1 className="text-3xl font-bold text-gray-900">Projects</h1>
      <p className="mt-1 text-gray-600">Your projects and the ones you're a member of.</p>

      {/* "New project" card */}
      <form
        onSubmit={handleSubmit}
        className="mt-6 rounded-lg border border-gray-200 bg-white p-5 shadow-sm"
      >
        <h2 className="text-lg font-semibold text-gray-900">New project</h2>

        <div className="mt-3 flex flex-col gap-3 sm:flex-row">
          <input
            type="text"
            placeholder="Project name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="flex-1 rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:outline-none"
          />
          <input
            type="text"
            placeholder="Description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="flex-1 rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:outline-none"
          />
          <button
            type="submit"
            className="rounded-md bg-blue-600 px-4 py-2 font-medium text-white hover:bg-blue-700"
          >
            Create
          </button>
        </div>

        {error && <p className="mt-3 text-sm text-red-600">{error}</p>}
      </form>

      {/* The list of projects */}
      {loading ? (
        <p className="mt-8 text-gray-600">Loading projects...</p>
      ) : projects.length === 0 ? (
        <p className="mt-8 text-gray-600">You don't have any projects yet.</p>
      ) : (
        <ul className="mt-8 grid gap-4 sm:grid-cols-2">
          {projects.map(project => (
            <li key={project.id}>
              <Link
                to={`/projects/${project.id}`}
                className="block h-full rounded-lg border border-gray-200 bg-white p-5 shadow-sm transition hover:border-blue-300 hover:shadow-md"
              >
                <p className="font-semibold text-gray-900">{project.name}</p>
                <p className="mt-1 text-sm text-gray-600">
                  {project.description || "No description"}
                </p>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default ProjectsPage