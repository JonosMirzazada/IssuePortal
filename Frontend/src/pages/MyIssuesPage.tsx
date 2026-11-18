import { useEffect, useState } from 'react'
import { Link } from 'react-router'
import { getMyIssues, type ProjectIssue } from '../api'
import { StatusBadge, PriorityBadge } from '../components/Badges'   // NEW

function MyIssuesPage() {
  const [issues, setIssues] = useState<ProjectIssue[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState("")

  useEffect(() => {
    getMyIssues()
      .then(data => setIssues(data))
      .catch(() => setError("Could not load your issues."))
      .finally(() => setLoading(false))
  }, [])

  return (
    <div>
      <h1 className="text-3xl font-bold text-gray-900">My issues</h1>
      <p className="mt-1 text-gray-600">Issues assigned to you, in all your projects.</p>

      {loading ? (
        <p className="mt-8 text-gray-600">Loading your issues...</p>
      ) : error ? (
        <p className="mt-8 text-red-600">{error}</p>
      ) : issues.length === 0 ? (
        <p className="mt-8 text-gray-600">No issues are assigned to you.</p>
      ) : (
        <ul className="mt-8 flex flex-col gap-3">
          {issues.map(issue => (
            <li key={issue.id}>
              <Link
                to={`/issues/${issue.id}`}
                className="flex items-center justify-between gap-4 rounded-lg border border-gray-200 bg-white p-4 shadow-sm transition hover:border-blue-300 hover:shadow-md"
              >
                <span className="font-medium text-gray-900">{issue.title}</span>

                {/* NEW: colored badges instead of plain text */}
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
  )
}

export default MyIssuesPage