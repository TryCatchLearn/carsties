'use client';

import {useRouter} from "next/navigation";
import {Auction} from "@/lib/types";
import {deleteListing} from "@/features/listings/actions";
import {toast} from "@/components/ui/toast";
import {AppConfirmDialog} from "@/components/ui/app-confirm-dialog";
import {Button} from "@/components/ui/button";

type Props = {
    auction: Auction;
}

export default function DeleteButton({ auction }: Props) {
    const router = useRouter()
    
    const handleDelete = async () => {
        const result = await deleteListing(auction.id);
        
        if (!result.ok) {
            toast.add({
                type: 'error',
                title: result.status,
                description: result.error,
            })
        } else {
            router.push('/');
        }
    }
    
    return (
        <AppConfirmDialog 
            trigger={<Button variant='destructive'>Delete</Button>} 
            onConfirmAction={handleDelete} 
        />
    );
}