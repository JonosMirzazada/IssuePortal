import { useEffect, useState } from 'react'
import { useParams, Link, useNavigate } from 'react-router'   // CHANGED: + useNavigate
import {
  getIssue,
  createComment,
  updateIssueStatus,
  assignIssue,
  getCurrentUserId,
  getCurrentUserRole,
  deleteIssue,
  type IssueDetails,
} from '../api'

function IssueDetailPage() {
  const { id } = useParams()
  const [issue, setIssue] = useState<IssueDetails | null>(null)
  const [error, setError] = useState("")

  const [content, setContent] = useState("")
  const [formError, setFormError] = useState("")
  const [updateError, setUpdateError] = useState("")

  const myId = getCurrentUserId()
  const role = getCurrentUserRole()
  const canEdit = role === "Developer" || role === "Admin"
  const isAdmin = role === "Admin"     // NEW
  const navigate = useNavigate()       // NEW

  useEffect(() => {
    if (!id) return

    getIssue(id)
      .then(data => setIssue(data))
      .catch(() => setError("Could not load the issue."))
  }, [id])

  async function handleStatusChange(newStatus: string) {
    setUpdateError("")
    if (!issue) return

    try {
      await updateIssueStatus(issue, newStatus)
      setIssue({ ...issue, status: newStatus })
    } catch (err) {
      setUpdateError(err instanceof Error ? err.message : "Could not update the status.")
    }
  }

  async function handleAssign(userId: number | null) {
    setUpdateError("")
    if (!issue) return

    try {
      await assignIssue(issue, userId)
      setIssue({ ...issue, assignedUserId: userId })
    } catch (err) {
      setUpdateError(err instanceof Error ? err.message : "Could not assign the issue.")
    }
  }

  // NEW: delete the issue, then go back to its project
  async function handleDelete() {
    if (!issue) return
    if (!window.confirm(`Delete "${issue.title}"? This can't be undone.`)) return

    try {
      await deleteIssue(issue.id)
      navigate(`/projects/${issue.projectId}`)
    } catch (err) {
      setUpdateError(err instanceof Error ? err.message : "Could not delete the issue.")
    }
  }

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

  const assignedToMe = issue.assignedUserId === myId

  return (
    <div>
      <Link to={`/projects/${issue.projectId}`} className="text-sm text-blue-600 hover:underline">
        ← Back to project
      </Link>

      <h1 className="mt-4 text-3xl font-bold">{issue.title}</h1>

      <div className="mt-2 flex items-center gap-3 text-sm">
        {canEdit ? (
          <select
            value={issue.status}
            onChange={(e) => handleStatusChange(e.target.value)}
            className="rounded border border-gray-300 px-2 py-1"
          >
            <option value="Open">Open</option>
            <option value="InProgress">In progress</option>
            <option value="Closed">Closed</option>
          </select>
        ) : (
          <span className="font-medium">{issue.status}</span>
        )}
        <span className="text-gray-600">Priority: {issue.priority}</span>
      </div>

      <div className="mt-2 flex items-center gap-3 text-sm">
        <span className="text-gray-600">
          {assignedToMe
            ? "Assigned to you"
            : issue.assignedUserId === null
              ? "Unassigned"
              : `Assigned to user #${issue.assignedUserId}`}
        </span>

        {canEdit && (
          assignedToMe ? (
            <button onClick={() => handleAssign(null)} className="text-blue-600 hover:underline">
              Unassign
            </button>
          ) : (
            <button onClick={() => handleAssign(myId)} className="text-blue-600 hover:underline">
              Assign to me
            </button>
          )
        )}
      </div>

      {updateError && <p className="mt-2 text-sm text-red-600">{updateError}</p>}

      {/* NEW: only admins may delete */}
      {isAdmin && (
        <button
          onClick={handleDelete}
          className="mt-4 rounded border border-red-600 px-3 py-1 text-sm text-red-600 hover:bg-red-50"
        >
          Delete issue
        </button>
      )}

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