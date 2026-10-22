import { useEffect, useState } from 'react'
import { getProjects, createProject, type Project } from '../api'   // NEW: createProject
import { Link } from 'react-router'

function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([])
  const [name, setName] = useState("")               // NEW
  const [description, setDescription] = useState("") // NEW
  const [error, setError] = useState("")             // NEW

  useEffect(() => {
    getProjects()
      .then(data => setProjects(data))
      .catch(() => console.log("Could not load projects"))
  }, [])

  // NEW: runs when the "Create" button is clicked
  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError("")

    try {
      const newProject = await createProject(name, description)
      setProjects([...projects, newProject])   // add it to the list
      setName("")                               // empty the fields
      setDescription("")
    } catch {
      setError("Could not create the project.")
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold">Projects</h1>

      {/* NEW: the form */}
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

      {projects.length === 0 && (
        <p className="mt-6 text-gray-600">You don't have any projects yet.</p>
      )}

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
    </div>
  )
}

export default ProjectsPage