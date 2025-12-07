export class EditMode {
  private _state: EditModeType = 'view';

  constructor(value: EditModeType) {
    this._state = value;
  }

  get state(): EditModeType {
    return this._state;
  }
  set state(value: EditModeType) {
    this._state = value;
  }

  get isView(): boolean {
    return this._state === 'view';
  }

  get isEdit(): boolean {
    return this._state === 'edit';
  }

  toggle() {
    this._state = this._state == 'view' ? 'edit' : 'view';
  }

  static from(value: EditModeType) {
    return new EditMode(value);
  }
}

type EditModeType = 'view' | 'edit';
