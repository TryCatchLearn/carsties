import {Auction, PagedResult} from "@/lib/types";

export type ListingSearchParams = {
    pageNumber?: string | string[];
    pageSize?: string | string[];
    searchTerm?: string | string[];
    orderBy?: string | string[];
    filterBy?: string | string[];
}

const baseUrl = process.env.BASE_API_URL || 'http://localhost:6001';

export async function getListings(params: ListingSearchParams = {}): Promise<PagedResult<Auction>> {
    const { pageNumber, pageSize, searchTerm, orderBy, filterBy } = params;
    
    const query = new URLSearchParams({
        pageNumber: pageNumber?.toString() || String(1),
        pageSize: pageSize?.toString() || String(8),
    });
    
    if (searchTerm) query.set("searchTerm", searchTerm.toString());
    query.set('orderBy', orderBy?.toString() || 'endingSoon');
    query.set('filterBy', filterBy?.toString() || 'live');
    
    const res = await fetch(`${baseUrl}/search?${query}`);

    if (!res.ok) throw new Error('Failed to fetch data');

    return res.json();
}

export async function getListingDetails(id: string): Promise<Auction> {
    const res = await fetch(`${baseUrl}/auctions/${id}`);
    
    if (!res.ok) throw new Error('Failed to fetch data');
    
    return res.json();
}