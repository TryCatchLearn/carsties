'use client'

import {useState, useTransition} from "react";
import {Button} from "@/components/ui/button";
import {getAuthTest} from "@/features/session/actions";
import {FetchResult} from "@/lib/fetch-wrapper";

export default function AuthTestButton() {
    const [result, setResult] = useState<FetchResult<string> | null>(null);
    const [isPending, startTransition] = useTransition();
    
    return (
        <div className='flex flex-col gap-3'>
            <Button
                disabled={isPending}
                onClick={() => {
                    setResult(null)
                    startTransition(async () => {
                        setResult(await getAuthTest())
                    })
                }}
            >
                {isPending ? 'Testing endpoint...' : 'Test auth'}
            </Button>
            {result && (
                <div className='rounded border border-foreground p-3 flex flex-col gap-3'>
                    <div>HTTP {result.ok ? 200 : result.status}</div>
                    <pre>{result.ok ? result.data : result.error}</pre>
                </div>
            )}
        </div>
    );
}