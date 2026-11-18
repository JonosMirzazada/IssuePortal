type StatusBadgeProps = {
  status: string
}

export function StatusBadge({ status }: StatusBadgeProps) {
  const styles: Record<string, string> = {
    Open: "bg-blue-100 text-blue-800",
    InProgress: "bg-yellow-100 text-yellow-800",
    Closed: "bg-green-100 text-green-800",
  }

  const labels: Record<string, string> = {
    Open: "Open",
    InProgress: "In progress",
    Closed: "Closed",
  }

  return (
    <span className={`inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-medium ${styles[status] ?? "bg-gray-100 text-gray-800"}`}>
      <span className="h-1.5 w-1.5 rounded-full bg-current" />
      {labels[status] ?? status}
    </span>
  )
}

type PriorityBadgeProps = {
  priority: string
}

export function PriorityBadge({ priority }: PriorityBadgeProps) {
  const styles: Record<string, string> = {
    Low: "bg-gray-100 text-gray-700",
    Medium: "bg-orange-100 text-orange-800",
    High: "bg-red-100 text-red-800",
  }

  return (
    <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${styles[priority] ?? "bg-gray-100 text-gray-800"}`}>
      {priority}
    </span>
  )
}