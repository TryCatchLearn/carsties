import Listings from "@/features/listings/Listings";
import {AppPagination} from "@/components/ui/app-pagination";
import {getListings} from "@/features/listings/actions";
import EmptyState from "@/components/EmptyState";
import Filters from "@/features/listings/Filters";

export default async function Home(props: PageProps<"/">) {
    const searchParams = await props.searchParams;

    const result = await getListings(searchParams);
    
    if (!result.ok) throw new Error(result.error)
    
    return (
        <div className='flex flex-col flex-1'>
            <Filters />
            {result.data.totalCount === 0 ? (
                <div className='flex flex-1 justify-center items-center'>
                    <EmptyState/>
                </div>
                
            ) : (
                <>
                    <Listings auctions={result.data.results}/>
                    <AppPagination
                        page={Number(searchParams["pageNumber"]) || 1}
                        pageSize={Number(searchParams["pageSize"]) || 8}
                        totalCount={result.data.totalCount}
                    />
                </>
            )}

        </div>
    );
}
