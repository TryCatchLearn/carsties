import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { Button } from "@/components/ui/button"
import {ReactElement, useState, useTransition} from "react";

type Props = {
    trigger: ReactElement
    onConfirmAction: () => Promise<boolean | void>
}

export function AppConfirmDialog({ trigger, onConfirmAction }: Props) {
    const [open, setOpen] = useState(false);
    const [isPending, startTransition] = useTransition();
    
    const handleConfirm = () => {
        startTransition(async () => {
            const shouldClose = await onConfirmAction();
            if (shouldClose) setOpen(false);
        })
    }
    
    return (
        <AlertDialog open={open} onOpenChange={setOpen}>
            <AlertDialogTrigger render={trigger} />
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
                    <AlertDialogDescription>
                        This action cannot be undone. This will permanently delete your
                        account from our servers.
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel disabled={isPending}>Cancel</AlertDialogCancel>
                    <AlertDialogAction
                        onClick={handleConfirm}
                        disabled={isPending}
                    >
                        {isPending ? 'Submitting...' : 'Continue'}
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}
