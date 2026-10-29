import { login } from '../api'
import { useNavigate } from 'react-router'
import { useState } from 'react'


function LoginPage() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [error, setError] = useState("")
  const navigate = useNavigate()

  // NEW: runs when the form is submitted
  async function handleSubmit(e: React.FormEvent) {
  e.preventDefault()
  setError("")                                // NEW: clear any old error

  try {
    const token = await login(email, password)
    console.log("Logged in! Token:", token)
     localStorage.setItem("token", token)   // NEW: save the token in the browser
    navigate("/projects")                  
  } catch (err) {
  setError(err instanceof Error ? err.message : "Login failed.")
}
}

  return (
    <div className="max-w-sm">
      <h1 className="text-3xl font-bold">Log in</h1>

      {/* NEW: onSubmit={handleSubmit} */}
      <form onSubmit={handleSubmit} className="mt-6 flex flex-col gap-4">
        <div>
          <label className="block text-sm font-medium">Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="mt-1 w-full rounded border border-gray-300 px-3 py-2"
          />
        </div>
          {error && <p className="text-sm text-red-600">{error}</p>}   {/* ← NEW LINE HERE */}
        <div>
          <label className="block text-sm font-medium">Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="mt-1 w-full rounded border border-gray-300 px-3 py-2"
          />
        </div>

        <button
          type="submit"
          className="rounded bg-blue-600 px-4 py-2 font-medium text-white hover:bg-blue-700"
        >
          Log in
        </button>
      </form>
    </div>
  )
}

export default LoginPage