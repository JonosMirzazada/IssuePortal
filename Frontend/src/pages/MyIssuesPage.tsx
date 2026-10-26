import { useEffect, useState } from 'react'
import { Link } from 'react-router'
import { getMyIssues, type ProjectIssue } from '../api'

function MyIssuesPage() {
  const [issues, setIssues] = useState<ProjectIssue[]>([])
  const [error, setError] = useState("")

  useEffect(() => {
    getMyIssues()
      .then(data => setIssues(data))
      .catch(() => setError("Could not load your issues."))
  }, [])

  return (
    <div>
      <h1 className="text-3xl font-bold">My issues</h1>
      <p className="mt-2 text-gray-600">Issues assigned to you, in all your projects.</p>

      {error && <p className="mt-4 text-red-600">{error}</p>}

      {!error && issues.length === 0 && (
        <p className="mt-6 text-gray-600">No issues are assigned to you.</p>
      )}

      <ul className="mt-6 flex flex-col gap-2">
        {issues.map(issue => (
          <li key={issue.id}>
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

export default MyIssuesPage