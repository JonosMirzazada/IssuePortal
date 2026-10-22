import { useEffect, useState } from 'react'
import { useParams } from 'react-router'
import { getProject, type ProjectDetails } from '../api'

function ProjectDetailPage() {
  const { id } = useParams()
  const [project, setProject] = useState<ProjectDetails | null>(null)
  const [error, setError] = useState("")

  useEffect(() => {
    if (!id) return

    getProject(id)
      .then(data => setProject(data))
      .catch(() => setError("Could not load the project."))
  }, [id])

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

      <h2 className="mt-8 text-xl font-semibold">Issues</h2>

      {project.issues.length === 0 && (
        <p className="mt-2 text-gray-600">No issues in this project yet.</p>
      )}

      <ul className="mt-4 flex flex-col gap-2">
        {project.issues.map(issue => (
          <li key={issue.id} className="rounded border border-gray-300 p-4">
            <p className="font-medium">{issue.title}</p>
            <p className="text-sm text-gray-600">
              {issue.status} · {issue.priority}
            </p>
          </li>
        ))}
      </ul>
    </div>
  )
}

export default ProjectDetailPage