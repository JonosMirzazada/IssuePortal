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
      <h1 className="text-3xl font-bold">Projects</h1>

      <form onSubmit={handleSubmit} className="mt-6 flex max-w-sm flex-col gap-3">
        <input
          type="text"
          placeholder="Project name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="rounded border border-gray-300 px-3 py-2"
        />
        <input
          type="text"
          placeholder="Description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="rounded border border-gray-300 px-3 py-2"
        />

        {error && <p className="text-sm text-red-600">{error}</p>}

        <button
          type="submit"
          className="rounded bg-blue-600 px-4 py-2 font-medium text-white hover:bg-blue-700"
        >
          Create project
        </button>
      </form>

      {/* NEW: three possible states */}
      {loading ? (
        <p className="mt-6 text-gray-600">Loading projects...</p>
      ) : projects.length === 0 ? (
        <p className="mt-6 text-gray-600">You don't have any projects yet.</p>
      ) : (
        <ul className="mt-6 flex flex-col gap-2">
          {projects.map(project => (
            <li key={project.id}>
              <Link
                to={`/projects/${project.id}`}
                className="block rounded border border-gray-300 p-4 hover:bg-gray-50"
              >
                <p className="font-medium">{project.name}</p>
                <p className="text-sm text-gray-600">{project.description}</p>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

export default ProjectsPage