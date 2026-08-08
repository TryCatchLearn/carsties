'use client'

import {ArrowDownAZ, ClockArrowDown, ClockPlus, Flame, OctagonPause, Timer} from "lucide-react";
import {ButtonGroup} from "@/components/ui/button-group";
import {Button} from "@/components/ui/button";
import {usePathname, useRouter, useSearchParams} from "next/navigation";

const orderButtons = [
    {label: 'Alphabetical', icon: ArrowDownAZ, value: 'make'},
    {label: 'End date', icon: ClockArrowDown, value: 'endingSoon'},
    {label: 'Recently added', icon: ClockPlus, value: 'new'},
]

const filterButtons = [
    {label: 'Live Auctions', icon: Flame, value: 'live'},
    {label: 'Ending < 10 hours', icon: OctagonPause, value: 'endingSoon'},
    {label: 'Completed', icon: Timer, value: 'finished'},
]

export default function Filters() {
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    
    const activeOrderBy = searchParams.get('orderBy') ?? 'endingSoon';
    const activeFilterBy = searchParams.get('filterBy') ?? 'live';
    
    const setParams = (key: 'orderBy' | 'filterBy', value: string) => {
        const params = new URLSearchParams(searchParams);
        const current = params.get(key);
        
        if (current === value) {
            params.delete(key);
        } else {
            params.set(key, value);
        }
        
        params.set('pageNumber', '1');
        router.push(`${pathname}?${params.toString()}`);
    }
    
    return (
        <div className='flex justify-between items-center mb-4 -mt-4 p-3 border-b border-muted-foreground'>
            <div className='text-2xl font-semibold'>
                All listings
            </div>
            <div className='flex items-center gap-3'>
                <span className='uppercase text-sm text-muted-foreground'>Filter by</span>
                <ButtonGroup>
                    {filterButtons.map(({label, icon: Icon, value}) => (
                        <Button 
                            key={value} 
                            variant={activeFilterBy === value ? 'default' : 'outline'}
                            onClick={() => setParams('filterBy', value)}
                        >
                            <Icon />
                            {label}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
            <div className='flex items-center gap-3'>
                <span className='uppercase text-sm text-muted-foreground'>Order by</span>
                <ButtonGroup>
                    {orderButtons.map(({label, icon: Icon, value}) => (
                        <Button
                            key={value}
                            variant={activeOrderBy === value ? 'default' : 'outline'}
                            onClick={() => setParams('orderBy', value)}
                        >
                            <Icon />
                            {label}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
        </div>
    );
}