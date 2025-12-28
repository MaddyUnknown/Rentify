export class ApiError extends Error {
  private errors: string[] = [];

  constructor(errors: string[], message?: string) {
    super(message);
    this.name = 'ApiError';
    this.errors = errors;
  }

  get Errors(): string[] {
    return this.errors;
  }
}
