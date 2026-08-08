"use client"

import {
    CarFront,
    CreditCardIcon,
    LogOutIcon,
    SettingsIcon, Trophy,
    UserIcon,
} from "lucide-react"

import { Button } from "@/components/ui/button"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import Link from "next/link";
import {usePathname, useRouter, useSearchParams} from "next/navigation";
import {authClient} from "@/lib/auth-client";
import {SessionUser} from "@/lib/auth";

type Props = {
    user: SessionUser
}

export function UserMenu({user}: Props) {
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    
    const setParams = (key: 'seller' | 'winner', value: string) => {
        const params = new URLSearchParams(searchParams);
        
        if (key === 'seller' && params.has('winner')) params.delete('winner');
        if (key === 'winner' && params.has('seller')) params.delete('seller');
        
        params.set(key, value);
        params.set('pageNumber', '1');
        
        const dest = pathname === '/' ? pathname : '/';
        
        router.push(`${dest}?${params.toString()}`);
    }
    
    const signOut = () => {
        void authClient.signOut({
            fetchOptions: {
                onSuccess: () => {
                    router.push("/");
                    router.refresh();
                }
            }
        })
    }
    
    return (
        <DropdownMenu>
            <DropdownMenuTrigger render={
                <Button variant="outline">{user.name}</Button>} 
            />
            <DropdownMenuContent>
                <DropdownMenuItem onClick={() => setParams('seller', user.username)}>
                    <UserIcon />
                    My Auctions
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => setParams('winner', user.username)}>
                    <Trophy />
                    Auctions won
                </DropdownMenuItem>
                <DropdownMenuItem>
                    <CarFront />
                    Sell my car
                </DropdownMenuItem>
                <DropdownMenuItem>
                    <Link href='/session' className='flex gap-2 items-center'>
                        <SettingsIcon />
                        Session
                    </Link>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={signOut} variant="destructive">
                    <LogOutIcon />
                    Log out
                </DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    )
}
