export interface ResponseWrapper<T> {
  isSuccess: boolean;
  data?: T;
  errorList?: string[];
}
