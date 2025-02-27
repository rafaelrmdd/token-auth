import { FormEvent, useState } from "react";
import { instance } from "../../services/api/api";
import { AxiosResponse } from "axios";
import { useNavigate } from "react-router";

export function Login() {

    const [data, setData] = useState({})

    // useEffect(() => {
    //     const postAuth = async () => {
    //         const response : AxiosResponse = await instance.post('session', {
    //             email: "admin@gmail.com",
    //             permissions: ["teste"],
    //             password: "123456",
    //             roles: ["teste"]
    //         })
            
    //         if (!response) {
    //             console.log("Deu erro")
    //         }

    //         setData(response.data)
    //         console.log('response: ', response);
    //         console.log('data: ', data);
    //     }

    //     postAuth();
    // }, [])

    const navigate = useNavigate();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');

    const handleSubmit = async (e : FormEvent) => {
        e.preventDefault();

        try{
            const response : AxiosResponse = await instance.post('session', {
                email: email,
                permissions: ["teste"],
                password: password,
                roles: ["teste"]
            })

            if(response.status === 200){
                setData(response.data)
                console.log('response: ', response);
                console.log('data: ', data);

                navigate("/home")
            }

        }catch(error) {
            console.log('error: ', error)
        }

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