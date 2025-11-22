import { Component, Input, ViewEncapsulation, ElementRef } from '@angular/core';

@Component({
  selector: '[appForm]',
  standalone: true,
  imports: [],
  templateUrl: './form.component.html',
  styleUrl: './form.component.css',
  encapsulation: ViewEncapsulation.None,
  host: {
    class: 'app-form',
  },
})
export class FormComponent {
  private _isReadOnly: boolean = false;

  constructor(private hostElement: ElementRef<HTMLFormElement>) {}

  @Input() set isReadOnly(value: boolean) {
    if (this._isReadOnly == value) return;
    this._isReadOnly = value;
    this.updateReadOnly(value);
  }

  updateReadOnly(value: boolean) {
    const elementList = this.hostElement.nativeElement.querySelectorAll('input, textarea') as NodeListOf<
      HTMLInputElement | HTMLTextAreaElement
    >;
    elementList.forEach((elem) => (elem.readOnly = value));
  }
}
