import { Observable } from 'rxjs';

export function generateBase64Content(file: File): Observable<string> {
  return new Observable<string>((observer) => {
    const reader = new FileReader();
    reader.onload = () => {
      observer.next(reader.result as string);
      observer.complete();
    };

    reader.onerror = (error) => observer.error(error);
    reader.readAsDataURL(file);
  });
}
