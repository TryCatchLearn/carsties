import {CarFront} from "lucide-react";
import Link from "next/link";
import SearchInput from "@/components/nav/SearchInput";

export default function NavBar() {
    return (
        <header className='sticky top-0 z-50 p-3 bg-background items-center shadow-lg flex justify-between'>
            <Link href='/' className='flex items-center gap-2 text-3xl font-semibold text-red-500'>
                <CarFront size={50} />
                <div>Carsties Auctions</div>
            </Link>
            <SearchInput />
            <div>Login</div>
        </header>
    )
}