import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router'
import { getIssue, createComment, type IssueDetails } from '../api'   // NEW: createComment

function IssueDetailPage() {
  const { id } = useParams()
  const [issue, setIssue] = useState<IssueDetails | null>(null)
  const [error, setError] = useState("")

  // NEW: state for the comment form
  const [content, setContent] = useState("")
  const [formError, setFormError] = useState("")

  useEffect(() => {
    if (!id) return

    getIssue(id)
      .then(data => setIssue(data))
      .catch(() => setError("Could not load the issue."))
  }, [id])

  // NEW: runs when "Add comment" is clicked
  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setFormError("")
    if (!issue) return

    try {
      const newComment = await createComment(issue.id, content)
      setIssue({ ...issue, comments: [...issue.comments, newComment] })
      setContent("")
    } catch {
      setFormError("Could not add the comment.")
    }
  }

  if (error) {
    return <p className="text-red-600">{error}</p>
  }

  if (!issue) {
    return <p className="text-gray-600">Loading...</p>
  }

  return (
    <div>
      <Link to={`/projects/${issue.projectId}`} className="text-sm text-blue-600 hover:underline">
        ← Back to project
      </Link>

      <h1 className="mt-4 text-3xl font-bold">{issue.title}</h1>
      <p className="mt-1 text-sm text-gray-600">
        {issue.status} · {issue.priority}
      </p>
      <p className="mt-4">{issue.description}</p>

      <h2 className="mt-8 text-xl font-semibold">Comments</h2>

      {issue.comments.length === 0 && (
        <p className="mt-2 text-gray-600">No comments yet.</p>
      )}

      <ul className="mt-4 flex flex-col gap-2">
        {issue.comments.map(comment => (
          <li key={comment.id} className="rounded border border-gray-300 p-4">
            <p className="text-sm text-gray-600">
              {comment.userName} · {new Date(comment.createdAt).toLocaleString()}
            </p>
            <p className="mt-1">{comment.content}</p>
          </li>
        ))}
      </ul>

      {/* NEW: the comment form, under the comments */}
      <form onSubmit={handleSubmit} className="mt-6 flex max-w-lg flex-col gap-3">
        <textarea
          placeholder="Write a comment..."
          value={content}
          onChange={(e) => setContent(e.target.value)}
          rows={3}
          className="rounded border border-gray-300 px-3 py-2"
        />

        {formError && <p className="text-sm text-red-600">{formError}</p>}

        <button
          type="submit"
          className="self-start rounded bg-blue-600 px-4 py-2 font-medium text-white hover:bg-blue-700"
        >
          Add comment
        </button>
      </form>
    </div>
  )
}

export default IssueDetailPage