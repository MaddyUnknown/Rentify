import { BehaviorSubject, Observable } from 'rxjs';

export class EditMode {
  private _state$;

  constructor(value: EditModeType) {
    this._state$ = new BehaviorSubject<EditModeType>(value);
  }

  get state(): EditModeType {
    return this._state$.value;
  }
  set state(value: EditModeType) {
    this._state$.next(value);
  }

  get isView(): boolean {
    return this._state$.value === 'view';
  }

  get isEdit(): boolean {
    return this._state$.value === 'edit';
  }

  toggle() {
    this._state$.next(this._state$.value == 'view' ? 'edit' : 'view');
  }

  get changes(): Observable<EditModeType> {
    return this._state$.asObservable();
  }

  static from(value: EditModeType) {
    return new EditMode(value);
  }
}

type EditModeType = 'view' | 'edit';
