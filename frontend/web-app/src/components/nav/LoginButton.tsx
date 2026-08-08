'use client';

import {Button} from "@/components/ui/button";
import {authClient} from "@/lib/auth-client";

type Props = {
    callbackUrl?: string;
}

export default function LoginButton({ callbackUrl = '/' }: Props) {
    return (
        <Button
            variant='outline'
            size='lg'
            onClick={() => authClient.signIn.oauth2({
                providerId: 'duende',
                callbackURL: callbackUrl
            })}
        >
            Login
        </Button>
    );
}
