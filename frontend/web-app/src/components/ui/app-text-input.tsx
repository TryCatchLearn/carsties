import {Field, FieldError, FieldLabel} from "@/components/ui/field";
import {Input} from "@/components/ui/input";
import {useController, UseControllerProps} from "react-hook-form";
import {Textarea} from "@/components/ui/textarea";

type Props = {
    label?: string;
    type?: string;
    rows?: number;
    placeholder?: string;
    multiline?: boolean;
    minDate?: Date;
} & UseControllerProps

export default function AppTextInput(props: Props) {
    const {label, multiline, rows, placeholder, type, minDate, ...controllerProps} = props;
    const {field, fieldState} = useController({...controllerProps, defaultValue: ''});
    
    return (
        <Field data-invalid={!!fieldState.error}>
            {label &&
            <FieldLabel htmlFor='model'>{label}</FieldLabel>}
            {multiline ? (
                <Textarea
                    {...field}
                    id={props.name}
                    rows={rows}
                    placeholder={placeholder}
                    aria-invalid={!!fieldState.error}
                />
            ) : (
                <Input
                    id={props.name}
                    {...field}
                    onChange={e => {
                        const value = e.target.value;
                        field.onChange(type === 'number' ? Number(value) : value)
                    }}
                    type={type}
                    min={minDate?.toISOString().slice(0,16)}
                    placeholder={placeholder}
                    aria-invalid={!!fieldState.error}
                />
            )}
            
            <FieldError>{fieldState.error?.message}</FieldError>
        </Field>
    );
}