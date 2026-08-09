import {getListingDetails} from "@/features/listings/actions";
import {getCurrentUser} from "@/lib/auth";
import NotFound from "next/dist/client/components/builtin/not-found";
import {redirect} from "next/navigation";
import {Card, CardContent, CardDescription, CardHeader, CardTitle} from "@/components/ui/card";
import {Separator} from "@/components/ui/separator";
import AuctionForm from "@/features/listings/AuctionForm";

export default async function EditPage(props: PageProps<"/listings/[id]/edit">) {
    const {id} = await props.params;
    const result = await getListingDetails(id);
    const user = await getCurrentUser();

    if (!result.ok && result.status === 404) return NotFound();
    if (!result.ok) throw new Error(result.error);
    
    if (result.ok && result.data.seller !== user?.username) 
        return redirect(`/listings/${id}`);
    
    return (
        <Card className='w-3/4 mx-auto'>
            <CardHeader>
                <CardTitle className='text-2xl font-semibold'>Update your listing</CardTitle>
                <CardDescription>Whilst there are no bids you can edit your listing</CardDescription>
            </CardHeader>
            <Separator />
            <CardContent>
                <AuctionForm auction={result.data} />
            </CardContent>
        </Card>
    );
}