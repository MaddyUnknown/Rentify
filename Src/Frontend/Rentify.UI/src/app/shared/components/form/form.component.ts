import { Component, Input, ViewEncapsulation, ElementRef, Renderer2, OnChanges, SimpleChanges } from '@angular/core';

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
export class FormComponent implements OnChanges {
  constructor(
    private hostElement: ElementRef<HTMLElement>,
    private renderer: Renderer2,
  ) {}

  @Input() isReadOnly?: boolean;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isReadOnly']) {
      this.updateReadOnly(changes['isReadOnly'].currentValue ?? false);
    }
  }

  private updateReadOnly(value: boolean) {
    const readonlyList = this.hostElement.nativeElement.querySelectorAll('input, textarea, select') as NodeListOf<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >;

    readonlyList.forEach((elem) => {
      if (value) {
        this.renderer.setAttribute(elem, 'readonly', '');
      } else {
        this.renderer.removeAttribute(elem, 'readonly');
      }
    });
  }
}
