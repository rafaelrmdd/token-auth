import { useContext, useEffect } from "react";
import { AuthContext } from "../../context/AuthContext";
import { api } from "../../services/api/api";

export function Home() {

    const { user } = useContext(AuthContext)

    useEffect(() => {
        api.get('me').then(response => console.log(response)).catch(error => console.log(error));
    }, [])

    return (
        <div>
            <p>{user?.email}</p>
        </div>
    )
}