export interface UIState<T> {
  data: T;
  isNew?: boolean;
  isActionDisabled?: boolean;
}
