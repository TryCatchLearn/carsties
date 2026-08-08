import Listings from "@/features/listings/Listings";
import {AppPagination} from "@/components/ui/app-pagination";
import {getListings} from "@/features/listings/actions";
import EmptyState from "@/components/EmptyState";
import Filters from "@/features/listings/Filters";

export default async function Home(props: PageProps<"/">) {
    const searchParams = await props.searchParams;

    const data = await getListings(searchParams);
    return (
        <div className='flex flex-col flex-1'>
            {data.totalCount === 0 ? (
                <div className='flex flex-1 justify-center items-center'>
                    <EmptyState/>
                </div>
                
            ) : (
                <>
                    <Filters />
                    <Listings auctions={data.results}/>
                    <AppPagination
                        page={Number(searchParams["pageNumber"]) || 1}
                        pageSize={Number(searchParams["pageSize"]) || 8}
                        totalCount={data.totalCount}
                    />
                </>
            )}

        </div>
    );
}
