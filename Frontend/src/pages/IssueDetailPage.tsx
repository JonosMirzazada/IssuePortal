import { useEffect, useState } from 'react'
import { useParams, Link, useNavigate } from 'react-router'
import {
  getIssue,
  createComment,
  updateIssueStatus,
  updateIssuePriority,
  assignIssue,
  getCurrentUserId,
  getCurrentUserRole,
  deleteIssue,
  type IssueDetails,
} from '../api'
import { StatusBadge, PriorityBadge } from '../components/Badges'   // NEW

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
  const isAdmin = role === "Admin"
  const navigate = useNavigate()

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

  async function handlePriorityChange(newPriority: string) {
    setUpdateError("")
    if (!issue) return

    try {
      await updateIssuePriority(issue, newPriority)
      setIssue({ ...issue, priority: newPriority })
    } catch (err) {
      setUpdateError(err instanceof Error ? err.message : "Could not update the priority.")
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
      {/* Back link, title and badges */}
      <Link to={`/projects/${issue.projectId}`} className="text-sm text-blue-600 hover:underline">
        ← Back to project
      </Link>
      <h1 className="mt-3 text-3xl font-bold text-gray-900">{issue.title}</h1>
      <div className="mt-3 flex gap-2">
        <StatusBadge status={issue.status} />
        <PriorityBadge priority={issue.priority} />
      </div>

      {/* Two columns on big screens */}
      <div className="mt-8 grid gap-6 lg:grid-cols-3">

        {/* LEFT: description + comments */}
        <div className="flex flex-col gap-6 lg:col-span-2">

          {/* Description card */}
          <section className="rounded-lg border border-gray-200 bg-white p-5 shadow-sm">
            <h2 className="text-sm font-semibold uppercase tracking-wide text-gray-500">Description</h2>
            <p className="mt-2 whitespace-pre-wrap text-gray-800">
              {issue.description || "No description."}
            </p>
          </section>

          {/* Comments */}
          <section>
            <h2 className="text-lg font-semibold text-gray-900">
              Comments <span className="text-gray-500">({issue.comments.length})</span>
            </h2>

            {issue.comments.length === 0 ? (
              <p className="mt-3 text-gray-600">No comments yet. Be the first!</p>
            ) : (
              <ul className="mt-3 flex flex-col gap-3">
                {issue.comments.map(comment => (
                  <li key={comment.id} className="flex gap-3 rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
                    <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-blue-100 text-sm font-semibold text-blue-800">
                      {comment.userName.charAt(0).toUpperCase()}
                    </span>
                    <div className="min-w-0">
                      <p className="text-sm">
                        <span className="font-medium text-gray-900">{comment.userName}</span>
                        <span className="text-gray-500"> · {new Date(comment.createdAt).toLocaleString()}</span>
                      </p>
                      <p className="mt-1 whitespace-pre-wrap text-gray-800">{comment.content}</p>
                    </div>
                  </li>
                ))}
              </ul>
            )}

            {/* Comment form card */}
            <form
              onSubmit={handleSubmit}
              className="mt-4 rounded-lg border border-gray-200 bg-white p-4 shadow-sm"
            >
              <textarea
                placeholder="Write a comment..."
                value={content}
                onChange={(e) => setContent(e.target.value)}
                rows={3}
                className="w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:outline-none"
              />
              {formError && <p className="mt-2 text-sm text-red-600">{formError}</p>}
              <div className="mt-2 flex justify-end">
                <button
                  type="submit"
                  className="rounded-md bg-blue-600 px-4 py-2 font-medium text-white hover:bg-blue-700"
                >
                  Add comment
                </button>
              </div>
            </form>
          </section>
        </div>

        {/* RIGHT: details sidebar */}
        <aside className="h-fit rounded-lg border border-gray-200 bg-white p-5 shadow-sm">
          <h2 className="text-lg font-semibold text-gray-900">Details</h2>

          <dl className="mt-4 flex flex-col gap-4 text-sm">
            {/* Status */}
            <div>
              <dt className="text-gray-500">Status</dt>
              <dd className="mt-1">
                {canEdit ? (
                  <select
                    value={issue.status}
                    onChange={(e) => handleStatusChange(e.target.value)}
                    className="w-full rounded-md border border-gray-300 px-2 py-1.5 focus:border-blue-500 focus:outline-none"
                  >
                    <option value="Open">Open</option>
                    <option value="InProgress">In progress</option>
                    <option value="Closed">Closed</option>
                  </select>
                ) : (
                  <StatusBadge status={issue.status} />
                )}
              </dd>
            </div>

            {/* Priority */}
            <div>
              <dt className="text-gray-500">Priority</dt>
              <dd className="mt-1">
                {canEdit ? (
                  <select
                    value={issue.priority}
                    onChange={(e) => handlePriorityChange(e.target.value)}
                    className="w-full rounded-md border border-gray-300 px-2 py-1.5 focus:border-blue-500 focus:outline-none"
                  >
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                  </select>
                ) : (
                  <PriorityBadge priority={issue.priority} />
                )}
              </dd>
            </div>

            {/* Assignee */}
            <div>
              <dt className="text-gray-500">Assignee</dt>
              <dd className="mt-1 text-gray-900">
                {assignedToMe
                  ? "You"
                  : issue.assignedUserId === null
                    ? "Nobody yet"
                    : `User #${issue.assignedUserId}`}
              </dd>
              {canEdit && (
                <button
                  onClick={() => handleAssign(assignedToMe ? null : myId)}
                  className="mt-2 w-full rounded-md border border-gray-300 px-3 py-1.5 text-gray-700 hover:bg-gray-100"
                >
                  {assignedToMe ? "Unassign me" : "Assign to me"}
                </button>
              )}
            </div>
          </dl>

          {updateError && <p className="mt-4 text-sm text-red-600">{updateError}</p>}

          {/* Danger zone, admins only */}
          {isAdmin && (
            <div className="mt-5 border-t border-gray-200 pt-4">
              <button
                onClick={handleDelete}
                className="w-full rounded-md border border-red-300 px-3 py-1.5 text-sm text-red-600 hover:bg-red-50"
              >
                Delete issue
              </button>
            </div>
          )}
        </aside>
      </div>
    </div>
  )
}

export default IssueDetailPage