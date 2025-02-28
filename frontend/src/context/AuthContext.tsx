import { createContext, ReactNode, useEffect, useState } from "react";
import { instance } from "../services/api/api";
import { AxiosResponse } from "axios";
import { useNavigate } from "react-router";
import { parseCookies, setCookie } from "nookies";

type User = {
    email: string;
    permissions: string[];
    roles: string[];
}

type SignInCredentials = {
    email: string;
    password: string;
}

type AuthContextProps = {
    signIn : (credentials : SignInCredentials) => Promise<void>;
    isAuthenticated: boolean;
    user: User | undefined;
}

type AuthProviderProps = {
    children: ReactNode
}

// eslint-disable-next-line react-refresh/only-export-components
export const AuthContext = createContext({} as AuthContextProps);

export function AuthContextProvider ({children} : AuthProviderProps) {

    useEffect(() => {
        const { 'auth.token': token } = parseCookies();

        if (token) {
            instance.get('me').then(response => console.log('response: ', response))
        }
    }, [])

    const navigate = useNavigate();
    const [user, setUser] = useState<User>();
    const isAuthenticated = !!user;
    
    async function signIn({email, password} : SignInCredentials) {
        try{
            const response : AxiosResponse = await instance.post('session', {
                email: email,
                permissions: ["teste"],
                password: password,
                roles: ["teste"]
            })

            const { jwt, refreshToken, permissions, roles } = response.data;

            setCookie(undefined, 'auth.token', jwt, {
                maxAge: 60 * 60 * 24 * 30, // 30 days
                path: "/"
            });

            setCookie(undefined, 'auth.refreshToken', refreshToken, {
                maxAge: 60 * 60 * 24 * 30,
                path: "/"
            });

            setUser({
                email,
                permissions,
                roles
            })

            console.log('response: ', response);
            navigate("/home")

        }catch(error) {
            console.log('error: ', error)
        }
    }

    return(
        <AuthContext.Provider value={{ signIn, isAuthenticated, user }}>
            {children}
        </AuthContext.Provider>
    )
}