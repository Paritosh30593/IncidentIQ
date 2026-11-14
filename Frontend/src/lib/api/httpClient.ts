import axios from 'axios'
import { InteractionRequiredAuthError } from '@azure/msal-browser'
import { protectedResources } from '../../config/authConfig'
import { msalInstance } from '../auth/msalInstance'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL?.replace(/\/+$/, '') ?? ''

export const httpClient = axios.create({
    baseURL: `${apiBaseUrl}/api/`,
})

httpClient.interceptors.request.use(async (config) => {
    const account = msalInstance.getActiveAccount()
        ?? msalInstance.getAllAccounts()[0]

    if (!account)
        return config

    const request = {
        scopes: protectedResources.api.scopes,
        account
    }

    try {
        const { accessToken } = await msalInstance.acquireTokenSilent(request)
        config.headers.Authorization = `Bearer ${accessToken}`
    }
    catch (error) {
        if (error instanceof InteractionRequiredAuthError) {
            await msalInstance.acquireTokenRedirect(request)
        }
        throw error
    }

    return config
})
