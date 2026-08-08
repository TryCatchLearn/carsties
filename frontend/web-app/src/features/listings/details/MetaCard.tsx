type Props = {
    label: string;
    value: string | number;
}

export default function MetaCard({label, value}: Props) {
    return (
        <div className='rounded-xl border border-foreground/40 bg-background/75 p-4 flex flex-col flex-1'>
            <span className='text-xs uppercase text-muted-foreground'>{label}</span>
            <p className='mt-2 text-sm font-mediumd'>{value}</p>
        </div>
    );
}