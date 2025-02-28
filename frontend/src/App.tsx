import { Login } from "./pages/login"
import { Home } from "./pages/home"
import { Route, Routes } from "react-router"
import { AuthContextProvider } from "./context/AuthContext"

function App() {

  return ( 
    <AuthContextProvider>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/home" element={<Home />} />
      </Routes> 
    </AuthContextProvider>
 
  )
}

export default App
