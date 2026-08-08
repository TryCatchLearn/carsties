import {getListingDetails} from "@/features/listings/actions";
import NotFound from "next/dist/client/components/builtin/not-found";
import CountdownTimer from "@/features/listings/CountdownTimer";
import CarImage from "@/features/listings/CarImage";
import {Card, CardDescription, CardHeader, CardTitle} from "@/components/ui/card";
import MetaCard from "@/features/listings/details/MetaCard";

export default async function ListingDetailedPage(props: PageProps<"/listings/[id]">) {
    const {id} = await props.params;
    const auction = await getListingDetails(id);
    
    if (!auction) return NotFound();
    
    return (
        <div className='flex flex-col'>
            <div className='flex justify-between'>
                <h3 className='text-2xl font-semibold'>
                    {auction.make} {auction.model}
                </h3>
                <div className='flex items-center gap-3'>
                    <span className='text-muted-foreground uppercase'>
                        Time remaining
                    </span>
                    <CountdownTimer auctionEnd={auction.auctionEnd} />
                </div>
            </div>
            <div className='flex gap-6 mt-3'>
                <div className='flex w-1/2 flex-col'>
                    <CarImage imageUrl={auction.imageUrl} thumbnail={false} />
                    <div className='flex flex-col gap-3 mt-3'>
                        <div className='flex gap-3'>
                            <MetaCard label='Mileage' value={auction.mileage} />
                            <MetaCard label='Color' value={auction.color} />
                            <MetaCard label='Year' value={auction.year} />
                            <MetaCard label='Seller' value={auction.seller} />
                        </div>
                        <MetaCard label='Description' value={auction.description} />
                    </div>
                </div>
                <div className='flex flex-col w-1/2'>
                    <Card className='max-h-[80vh]'>
                        <CardHeader>
                            <CardTitle>Bid panel</CardTitle>
                            <CardDescription>Coming soon...</CardDescription>
                        </CardHeader>
                    </Card>
                </div>
               
            </div>
        </div>
    );
}