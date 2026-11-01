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
    <div className="p-8">
      <nav className="mb-6 flex gap-4">
        {loggedIn ? (
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
    </div>
  )
}

export default App