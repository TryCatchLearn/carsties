'use client';

import Image from "next/image";
import {useState} from "react";
import {clsx} from "clsx";

type Props = {
    imageUrl: string;
    thumbnail?: boolean;
}

export default function CarImage({ imageUrl, thumbnail = true }: Props) {
    const [isLoading, setIsLoading] = useState(true);
    
    return (
        <Image
            src={imageUrl}
            alt='Image of car'
            height={thumbnail ? 400 : 1200}
            width={thumbnail ? 400 : 1200}
            className={clsx('object-cover duration-700 ease-in-out aspect-16/10 border-b border-muted-foreground/20', {
                'opacity-0 scale-105': isLoading,
                'opacity-100 scale-100': !isLoading,
                'rounded-xl': !thumbnail
            })}
            priority
            sizes='(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw'
            onLoad={() => setIsLoading(false)}
        />
    );
}