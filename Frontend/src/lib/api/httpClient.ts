import axios from 'axios'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.replace(/\/+$/, '') ?? ''

export const httpClient = axios.create({
    baseURL: `${apiBaseUrl}/api/`,
})
