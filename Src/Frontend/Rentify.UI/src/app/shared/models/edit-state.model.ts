import { EditMode } from './edit-mode.model';

export interface EditState<T> {
  data: T;
  mode: EditMode;
  // Used to store the data before editing
  previousData?: T;
  isNew?: boolean;
  isActionDisabled?: boolean;
  //Use `isInvalid` instead of `isValid` since null should be treated as valid
  isInvalid?: {
    [K in keyof T]?: boolean;
  };
  doShake?: {
    [K in keyof T]?: boolean;
  };
}
