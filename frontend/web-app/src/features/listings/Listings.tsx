import AuctionCard from "@/features/listings/AuctionCard";
import {Auction} from "@/lib/types";

type Props = {
    auctions: Auction[];
}

export default async function Listings({auctions}: Props) {

    
    return (
        <div className='grid grid-cols-4 gap-6'>
            {auctions.map(auction => (
                <AuctionCard auction={auction} key={auction.id} />
            ))}
        </div>
    );
}