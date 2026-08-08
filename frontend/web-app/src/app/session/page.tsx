import {getCurrentUser} from "@/lib/auth";
import {Card, CardContent, CardFooter, CardHeader} from "@/components/ui/card";
import {Button} from "@/components/ui/button";
import AuthTestButton from "@/features/session/AuthTestButton";

export default async function SessionPage() {
    const user = await getCurrentUser();
    
    return (
        <Card>
            <CardHeader className='text-2xl font-semibold border-b'>
                User session data
            </CardHeader>
            <CardContent className='min-h-40 p-4'>
                {user ? (
                    <pre>{JSON.stringify(user, null, 2)}</pre>
                ) : (
                    <div>User not signed in</div>
                )}
            </CardContent>
            <CardFooter className='flex flex-col items-start'>
                <h2 className='text-xl font-semibold'>Test API Call</h2>
                <div className='mt-3'>
                    <AuthTestButton />
                </div>
            </CardFooter>
        </Card>
    );
}