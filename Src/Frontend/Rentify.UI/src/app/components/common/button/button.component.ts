import { Component, ElementRef, HostBinding, Input, Renderer2, ViewEncapsulation } from '@angular/core';
import { LucideAngularModule, LucideIconData } from 'lucide-angular';

@Component({
  selector: 'button[appButton]',
  standalone: true,
  imports: [LucideAngularModule],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css',
  encapsulation: ViewEncapsulation.None,
})
export class ButtonComponent {
  readonly CONTENT_TRANSPARENT_CLASS = 'app-button--transparent';
  readonly CONTENT_TRANSITION_TIME = 200;

  @Input({ required: false })
  type: 'normal' | 'inline' = 'normal';

  @Input({ required: false })
  color: 'primary' | 'danger' | 'text' = 'primary';

  @Input({ required: false })
  set input(value: ButtonComponentInput) {
    if (this.currentInput?.label !== this.input?.label || this.currentInput?.icon !== this.input?.icon) {
      this.renderer.addClass(this.elementRef.nativeElement, this.CONTENT_TRANSPARENT_CLASS);

      setTimeout(() => {
        this.currentInput = value;
        this.renderer.removeClass(this.elementRef.nativeElement, this.CONTENT_TRANSPARENT_CLASS);
      }, this.CONTENT_TRANSITION_TIME);
    } else {
      this.currentInput = value;
    }
  }

  @HostBinding('class')
  get classMap(): string {
    const data = { typeClass: '', colorClass: '' };

    switch (this.type) {
      case 'normal':
        data.typeClass = 'app-button--normal';
        break;
      case 'inline':
        data.typeClass = 'app-button--inline';
        break;
    }

    switch (this.color) {
      case 'primary':
        data.colorClass = 'app-button--primary';
        break;
      case 'danger':
        data.colorClass = 'app-button--danger';
        break;
      case 'text':
        data.colorClass = 'app-button--text';
    }

    return 'app-button ' + data.typeClass + ' ' + data.colorClass;
  }

  currentInput?: ButtonComponentInput;

  constructor(
    private elementRef: ElementRef<HTMLButtonElement>,
    private renderer: Renderer2,
  ) {}
}

export interface ButtonComponentInput {
  icon?: LucideIconData;
  label?: string;
}
