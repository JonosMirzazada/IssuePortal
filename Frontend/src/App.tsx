import { Routes, Route, Navigate, Link, useNavigate, useLocation } from 'react-router'
import ProjectDetailPage from './pages/ProjectDetailPage'
import LoginPage from './pages/LoginPage'
import ProjectsPage from './pages/ProjectsPage'
import RegisterPage from './pages/RegisterPage'
import IssueDetailPage from './pages/IssueDetailPage'
import MyIssuesPage from './pages/MyIssuesPage'
import AdminPage from './pages/AdminPage'
import { getCurrentUserRole, isLoggedIn } from './api'

function App() {
  const navigate = useNavigate()
  useLocation()
  const loggedIn = isLoggedIn()
  const isAdmin = loggedIn && getCurrentUserRole() === "Admin"

  function handleLogout() {
    localStorage.removeItem("token")
    navigate("/login")
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header bar */}
      <header className="border-b border-gray-200 bg-white">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-3">
          <Link to="/" className="text-lg font-bold text-gray-900">
            🐞 IssuePortal
          </Link>

          <nav className="flex flex-wrap items-center gap-4 text-sm">
            {loggedIn ? (
              <>
                <Link to="/projects" className="text-gray-600 hover:text-blue-600">Projects</Link>
                <Link to="/my-issues" className="text-gray-600 hover:text-blue-600">My issues</Link>
                {isAdmin && <Link to="/admin" className="text-gray-600 hover:text-blue-600">Admin</Link>}
                <button
                  onClick={handleLogout}
                  className="rounded border border-gray-300 px-3 py-1 text-gray-700 hover:bg-gray-100"
                >
                  Log out
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="text-gray-600 hover:text-blue-600">Log in</Link>
                <Link
                  to="/register"
                  className="rounded bg-blue-600 px-3 py-1 text-white hover:bg-blue-700"
                >
                  Register
                </Link>
              </>
            )}
          </nav>
        </div>
      </header>

      {/* Page content */}
      <main className="mx-auto max-w-5xl px-4 py-8">
        <Routes>
          <Route path="/" element={<Navigate to="/projects" />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/projects"
            element={loggedIn ? <ProjectsPage /> : <Navigate to="/login" />}
          />
          <Route
            path="/projects/:id"
            element={loggedIn ? <ProjectDetailPage /> : <Navigate to="/login" />}
          />
          <Route
            path="/issues/:id"
            element={loggedIn ? <IssueDetailPage /> : <Navigate to="/login" />}
          />
          <Route
            path="/my-issues"
            element={loggedIn ? <MyIssuesPage /> : <Navigate to="/login" />}
          />
          <Route
            path="/admin"
            element={isAdmin ? <AdminPage /> : <Navigate to="/projects" />}
          />
        </Routes>
      </main>
    </div>
  )
}

export default App