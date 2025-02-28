import axios, { AxiosInstance } from "axios";
import { parseCookies } from "nookies";

const cookies = parseCookies()

export const instance : AxiosInstance  = axios.create({
    baseURL: 'http://localhost:5194/api',
    headers: {
        Authorization: `Bearer ${cookies['auth.token']}`
    }
})  