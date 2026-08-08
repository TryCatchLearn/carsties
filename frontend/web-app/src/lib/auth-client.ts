import { createAuthClient } from "better-auth/react"
import {genericOAuthClient} from "better-auth/client/plugins";
export const authClient = createAuthClient({
    baseURL: "http://localhost:3000",
    plugins: [
        genericOAuthClient()
    ]
})