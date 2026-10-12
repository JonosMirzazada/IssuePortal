import { useState } from 'react'

function LoginPage() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")

  // NEW: runs when the form is submitted
  function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    console.log("Email:", email, "Password:", password)
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