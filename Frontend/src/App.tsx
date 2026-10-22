import { Routes, Route, Navigate, Link, useNavigate } from 'react-router'
import LoginPage from './pages/LoginPage'
import ProjectsPage from './pages/ProjectsPage'
import RegisterPage from './pages/RegisterPage'

function App() {
  const navigate = useNavigate()

  function handleLogout() {
    localStorage.removeItem("token")
    navigate("/login")
  }

  return (
    <div className="p-8">
      <nav className="mb-6 flex gap-4">
        <Link to="/projects" className="text-blue-600 hover:underline">Projects</Link>
        <Link to="/login" className="text-blue-600 hover:underline">Log in</Link>
        <Link to="/register" className="text-blue-600 hover:underline">Register</Link>
        <button onClick={handleLogout} className="text-blue-600 hover:underline">
          Log out
        </button>
      </nav>

      <Routes>
      <Route path="/" element={<Navigate to="/projects" />} />
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route
      path="/projects"
      element={localStorage.getItem("token") ? <ProjectsPage /> : <Navigate to="/login" />}
      />
      </Routes>
      </div>
  )
}

export default App