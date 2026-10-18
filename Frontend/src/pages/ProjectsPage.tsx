import { useEffect, useState } from 'react'
import { getProjects, type Project } from '../api'

function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([])

  useEffect(() => {
    getProjects()
      .then(data => setProjects(data))
      .catch(() => console.log("Could not load projects"))
  }, [])

  return (
    <div>
      <h1 className="text-3xl font-bold">Projects</h1>

      {projects.length === 0 && (
        <p className="mt-2 text-gray-600">You don't have any projects yet.</p>
      )}

      <ul className="mt-4 flex flex-col gap-2">
        {projects.map(project => (
          <li key={project.id} className="rounded border border-gray-300 p-4">
            <p className="font-medium">{project.name}</p>
            <p className="text-sm text-gray-600">{project.description}</p>
          </li>
        ))}
      </ul>
    </div>
  )
}

export default ProjectsPage