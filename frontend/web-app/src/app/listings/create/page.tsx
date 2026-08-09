import {Card, CardContent, CardDescription, CardHeader, CardTitle} from "@/components/ui/card";
import {Separator} from "@/components/ui/separator";
import AuctionForm from "@/features/listings/AuctionForm";

export default function CreatePage() {
    return (
        <Card className='w-3/4 mx-auto'>
            <CardHeader>
                <CardTitle className='text-2xl font-semibold'>Sell your car</CardTitle>
                <CardDescription>Fill out the following form to sell you car</CardDescription>
            </CardHeader>
            <Separator />
            <CardContent>
                <AuctionForm />
            </CardContent>
        </Card>
    );
}