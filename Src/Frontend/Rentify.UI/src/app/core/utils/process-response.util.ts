import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ResponseWrapper } from '../models/response/response-wrapper.model';
import { ApiError } from '../exceptions/api-error';

export function processResponse<T>() {
  return (source$: Observable<ResponseWrapper<T>>) =>
    source$.pipe(
      map((response) => {
        if (response.isSuccess) {
          return response.data!;
        } else if (response.errorList) {
          throw new ApiError(response.errorList!);
        } else {
          throw new ApiError(['Error while processing request']);
        }
      }),
      catchError((error) => {
        if (error?.error?.isSuccess === false && error?.error?.errorList) {
          return throwError(() => new ApiError(error.error.errorList));
        } else {
          return throwError(() => new ApiError(['Error while processing request']));
        }
      }),
    );
}
