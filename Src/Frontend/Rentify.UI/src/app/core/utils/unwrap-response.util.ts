import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ResponseWrapper } from '../models/response/response-wrapper.model';
import { ApiError } from '../exceptions/api-error';

export function unwrapReponse<T>() {
  return (source$: Observable<ResponseWrapper<T>>) =>
    source$.pipe(
      map((response) => {
        if (!response.isSuccess) throw new ApiError(response.errorList ?? ['Error while processing request']);
        return response.data!;
      }),
    );
}
