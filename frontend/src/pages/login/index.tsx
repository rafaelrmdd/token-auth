import { FormEvent, useContext, useState } from "react";
import { AuthContext } from "../../context/AuthContext";

export function Login() {

    const { signIn } = useContext(AuthContext);
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = async (e : FormEvent) => {
        e.preventDefault();

        signIn({
            email,
            password
        });
    }

    return (
        <div className="flex items-center justify-center w-screen h-screen bg-gray-100">
            <div className="bg-gray-700 p-12 rounded">
                <form 
                    onSubmit={handleSubmit}
                    className="flex flex-col"
                >
                    <input 
                        type="text" 
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="Enter login"
                        className="text-gray-300 placeholder:text-gray-300 px-4 py-2 outline-0"
                    />
                    <input 
                        type="text" 
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Enter password" 
                        className="text-gray-300 placeholder:text-gray-300 px-4 py-2 mt-4 outline-0"
                    />

                    <button 
                        
                        className="text-gray-300 bg-gray-900 rounded px-4 py-2 hover:cursor-pointer mt-12"
                    >
                        Sign In
                    </button>
                </form>
            </div>
            
        </div>
    )
}