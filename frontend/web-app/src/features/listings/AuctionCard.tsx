import {Card, CardContent} from "@/components/ui/card";
import Link from "next/link";
import CountdownTimer from "@/features/listings/CountdownTimer";
import CarImage from "@/features/listings/CarImage";
import {Auction} from "@/lib/types";

type Props = {
    auction: Auction;
}

export default function AuctionCard({ auction }: Props) {
    return (
        <Link href={`/listings/${auction.id}`} 
              className='transition-transform duration-200 hover:-translate-y-1'>
            <Card className='relative mx-auto w-full pt-0'>
                <CarImage imageUrl={auction.imageUrl} />
                <div className='absolute bottom-18 left-2'>
                    <CountdownTimer auctionEnd={auction.auctionEnd} />
                </div>
                <CardContent className='flex justify-between items-center'>
                    <h3 className='text-muted-foreground'>
                        {auction.make} {auction.model}
                    </h3>
                    <p className='font-semibold text-sm'>{auction.year}</p>
                </CardContent>
            </Card>
        </Link>
        
    );
}