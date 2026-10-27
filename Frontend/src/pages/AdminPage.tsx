import { useEffect, useState } from 'react'
import { getUsers, updateUserRole, type User } from '../api'

function AdminPage() {
  const [users, setUsers] = useState<User[]>([])
  const [error, setError] = useState("")

  useEffect(() => {
    getUsers()
      .then(data => setUsers(data))
      .catch(err => setError(err instanceof Error ? err.message : "Could not load users."))
  }, [])

  async function handleRoleChange(userId: number, newRole: string) {
    setError("")

    try {
      await updateUserRole(userId, newRole)
      setUsers(users.map(user =>
        user.id === userId ? { ...user, role: newRole } : user
      ))
    } catch (err) {
      setError(err instanceof Error ? err.message : "Could not change the role.")
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold">Admin</h1>
      <p className="mt-2 text-gray-600">Manage users and their roles.</p>

      {error && <p className="mt-4 text-sm text-red-600">{error}</p>}

      <table className="mt-6 w-full text-left">
        <thead>
          <tr className="border-b border-gray-300">
            <th className="py-2">Name</th>
            <th className="py-2">Email</th>
            <th className="py-2">Role</th>
          </tr>
        </thead>
        <tbody>
          {users.map(user => (
            <tr key={user.id} className="border-b border-gray-200">
              <td className="py-2">{user.name}</td>
              <td className="py-2 text-gray-600">{user.email}</td>
              <td className="py-2">
                <select
                  value={user.role}
                  onChange={(e) => handleRoleChange(user.id, e.target.value)}
                  className="rounded border border-gray-300 px-2 py-1"
                >
                  <option value="User">User</option>
                  <option value="Developer">Developer</option>
                  <option value="Admin">Admin</option>
                </select>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default AdminPage