export class FormMode {
  private mode: FormModeType = 'view';

  toggle() {
    this.mode = this.mode == 'view' ? 'edit' : 'view';
  }

  set state(value: FormModeType) {
    this.mode = value;
  }

  get state(): FormModeType {
    return this.mode;
  }

  get isEdit(): boolean {
    return this.mode == 'edit';
  }

  get isView(): boolean {
    return this.mode == 'view';
  }
}

type FormModeType = 'view' | 'edit';
