import axios, { AxiosInstance, AxiosError } from "axios";
import { parseCookies, setCookie } from "nookies";

interface FailedRequest {
    onSuccess: (token: string) => void;
    onFailure: (err: AxiosError) => void;
}

let cookies = parseCookies();
let isRefreshing = false;
let failedRequestsQueue : FailedRequest[] = [];

export const api : AxiosInstance  = axios.create({
    baseURL: 'http://localhost:5194/api',
    headers: {
        Authorization: `Bearer ${cookies['auth.token']}`
    }
})  

api.interceptors.response.use(response => {
    return response;
}, (error : AxiosError) => {
    if (error.response?.status === 401) {
        if (error.response.statusText === 'Unauthorized') {
            cookies = parseCookies();

            const { 'auth.refreshToken': refreshToken } = cookies;
            const originalConfig = error.config;

            if (!isRefreshing) {
                isRefreshing = true;

                api.post('refresh', {
                    refreshToken: refreshToken,
                }).then(response => {
                    const { jwt } = response.data;
                    console.log("re" ,{jwt: jwt, refreshToken: response.data.refreshToken});
                
                    setCookie(undefined, 'auth.token', jwt, {
                        maxAge: 60 * 60 * 24 * 30, // 30 days
                        path: "/"
                    });
        
                    setCookie(undefined, 'auth.refreshToken', response.data.refreshToken, {
                        maxAge: 60 * 60 * 24 * 30, 
                        path: "/"
                    });
    
                    api.defaults.headers['Authorization'] = `Bearer ${jwt}`

                    failedRequestsQueue.forEach(request => request.onSuccess(jwt))
                    failedRequestsQueue = []
                }).catch(err => {
                    failedRequestsQueue.forEach(request => request.onFailure(err))
                    failedRequestsQueue = []
                }).finally(() => {
                    isRefreshing = false;
                })
            }

            return new Promise((resolve, reject) => {
                failedRequestsQueue.push({
                    onSuccess: (jwt: string) => {
                        if (originalConfig) {
                            originalConfig.headers['Authorization'] = `Bearer ${jwt}`;

                            resolve(api(originalConfig));
                        }      
                    },
                    onFailure: (err : AxiosError) => {
                        reject(err);
                    }
                })
            })
        } else {
            //deslogar usuario
        }
    }
})