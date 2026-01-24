export interface PaginatedList<T> {
  totalItems: number;
  currentPage: number;
  items: T[];
}
