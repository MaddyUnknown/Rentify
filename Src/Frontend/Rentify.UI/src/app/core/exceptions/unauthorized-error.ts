import { ApiError } from './api-error';

export class UnauthorizedError extends ApiError {
  constructor(errors?: string[], message?: string) {
    super(errors ?? ['Unauthorized user'], message);
    this.name = 'UnauthorizedError';
  }
}
