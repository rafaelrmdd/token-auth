import { useContext } from "react"
import { AuthContext } from "../../context/AuthContext"

export function Home() {

    const { user } = useContext(AuthContext)

    return (
        <div>
            <p>{user?.email}</p>
        </div>
    )
}