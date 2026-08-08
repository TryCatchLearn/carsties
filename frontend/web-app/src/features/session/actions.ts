'use server';

import {auth} from "@/lib/auth";
import {headers} from "next/headers";

export async function getAuthTest() {
    const {accessToken} = await auth.api.getAccessToken({
        body: {providerId: 'duende'},
        headers: await headers()
    });
    
    const res = await fetch(`${process.env.BASE_API_URL}/auctions/test`, {
        method: 'POST',
        headers: {Authorization: `Bearer ${accessToken}`},
        body: JSON.stringify({})
    });
    
    return {status: res.status, body: await res.text()}
}