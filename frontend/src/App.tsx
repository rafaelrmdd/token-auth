import { Login } from "./pages/login"
import { Home } from "./pages/home"
import { Route, Routes } from "react-router"

function App() {

  return (
    
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/home" element={<Home />} />
      
    </Routes>
      
  )
}

export default App
