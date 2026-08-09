'use server'

import {Auction, PagedResult} from "@/lib/types";
import {FieldValues} from "react-hook-form";
import {auth} from "@/lib/auth";
import {headers} from "next/headers";
import {fetchWrapper} from "@/lib/fetch-wrapper";

export type ListingSearchParams = {
    pageNumber?: string | string[];
    pageSize?: string | string[];
    searchTerm?: string | string[];
    orderBy?: string | string[];
    filterBy?: string | string[];
    seller?: string;
    winner?: string;
}

const baseUrl = process.env.BASE_API_URL || 'http://localhost:6001';

export async function getListings(params: ListingSearchParams = {}) {
    const { pageNumber, pageSize, searchTerm, orderBy, filterBy, seller, winner } = params;
    
    const query = new URLSearchParams({
        pageNumber: pageNumber?.toString() || String(1),
        pageSize: pageSize?.toString() || String(8),
    });
    
    if (searchTerm) query.set("searchTerm", searchTerm.toString());
    query.set('orderBy', orderBy?.toString() || 'endingSoon');
    query.set('filterBy', filterBy?.toString() || 'live');
    
    if (winner) query.set('winner', winner);
    if (seller) query.set('seller', seller);
    
    return fetchWrapper<PagedResult<Auction>>(`/search?${query}`);
}

export async function getListingDetails(id: string) {
    return fetchWrapper<Auction>(`/auctions/${id}`);
}

export async function createListing(values: FieldValues) {
    return fetchWrapper<Auction>('/auctions', {
        method: 'POST',
        body: JSON.stringify(values),
    })
}

export async function updateListing(values: FieldValues) {
    return fetchWrapper<void>(`/auctions/${values.id}`, {
        method: 'PUT',
        body: JSON.stringify(values),
    })
}

export async function deleteListing(id: string) {
    return fetchWrapper<void>(`/auctions/${id}`, {
        method: 'DELETE',
    })
}