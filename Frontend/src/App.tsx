import { Routes, Route, Navigate, Link, useNavigate, useLocation } from 'react-router'
import ProjectDetailPage from './pages/ProjectDetailPage'
import LoginPage from './pages/LoginPage'
import ProjectsPage from './pages/ProjectsPage'
import RegisterPage from './pages/RegisterPage'
import IssueDetailPage from './pages/IssueDetailPage'
import MyIssuesPage from './pages/MyIssuesPage'
import AdminPage from './pages/AdminPage'
import { getCurrentUserRole } from './api'

function App() {
  const navigate = useNavigate()
  useLocation()                                     // re-check on every page change
  const isAdmin = getCurrentUserRole() === "Admin"
  const isLoggedIn = localStorage.getItem("token") !== null

  function handleLogout() {
    localStorage.removeItem("token")
    navigate("/login")
  }

  return (
    <div className="p-8">
      {/* CHANGED: different links depending on whether you're logged in */}
      <nav className="mb-6 flex gap-4">
        {isLoggedIn ? (
          <>
            <Link to="/projects" className="text-blue-600 hover:underline">Projects</Link>
            <Link to="/my-issues" className="text-blue-600 hover:underline">My issues</Link>
            {isAdmin && <Link to="/admin" className="text-blue-600 hover:underline">Admin</Link>}
            <button onClick={handleLogout} className="text-blue-600 hover:underline">
              Log out
            </button>
          </>
        ) : (
          <>
            <Link to="/login" className="text-blue-600 hover:underline">Log in</Link>
            <Link to="/register" className="text-blue-600 hover:underline">Register</Link>
          </>
        )}
      </nav>

      <Routes>
        <Route path="/" element={<Navigate to="/projects" />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route
          path="/projects"
          element={isLoggedIn ? <ProjectsPage /> : <Navigate to="/login" />}
        />
        <Route
          path="/projects/:id"
          element={isLoggedIn ? <ProjectDetailPage /> : <Navigate to="/login" />}
        />
        <Route
          path="/issues/:id"
          element={isLoggedIn ? <IssueDetailPage /> : <Navigate to="/login" />}
        />
        <Route
          path="/my-issues"
          element={isLoggedIn ? <MyIssuesPage /> : <Navigate to="/login" />}
        />
        <Route
          path="/admin"
          element={isAdmin ? <AdminPage /> : <Navigate to="/projects" />}
        />
      </Routes>
    </div>
  )
}

export default App