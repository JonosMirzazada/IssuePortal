import { Routes, Route, Navigate, Link } from 'react-router'
import LoginPage from './pages/LoginPage'
import ProjectsPage from './pages/ProjectsPage'


function App() {
  return (
    <div className="p-8">

    <nav className="mb-6 flex gap-4">
      <Link to="/projects" className="text-blue-600 hover:underline">Projects</Link>
      <Link to="/login" className="text-blue-600 hover:underline">Log in</Link>
    </nav>

      <Routes>
        <Route path="/" element={<Navigate to="/projects" />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/projects" element={<ProjectsPage />} />
      </Routes>
    </div>
  )
}

export default App