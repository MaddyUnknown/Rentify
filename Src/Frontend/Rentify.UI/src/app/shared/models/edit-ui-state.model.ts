import { EditMode } from './edit-mode.model';
import { UIState } from './ui-state.model';

export interface UIEditState<T> extends UIState<T> {
  mode: EditMode;
  // Used to store the data before editing
  previousData?: T;
  //Use `isInvalid` instead of `isValid` since null should be treated as valid
  isInvalid?: {
    [K in keyof T]?: boolean;
  };
  doShake?: {
    [K in keyof T]?: boolean;
  };
}
