'use server';

import {fetchWrapper} from "@/lib/fetch-wrapper";

export async function getAuthTest() {
    return await fetchWrapper<string>('/auctions/test', {
        method: 'POST',
        body: JSON.stringify({})
    })
}