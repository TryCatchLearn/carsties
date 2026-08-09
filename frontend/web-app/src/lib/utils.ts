import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"
import {format} from "date-fns";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function toDatetimeLocal(date: string) {
  return format(new Date(date), "yyyy-MM-dd'T'HH:mm")
}

export function debounce<F extends (...args: Parameters<F>) => ReturnType<F>>(
    func: F,
    waitFor: number,
) {
  let timeout: ReturnType<typeof setTimeout>;

  const debounced = (...args: Parameters<F>): void => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), waitFor);
  };
  
  debounced.cancel = () => clearTimeout(timeout);
  
  return debounced;
}