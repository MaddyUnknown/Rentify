import {
  Component,
  ElementRef,
  HostBinding,
  Input,
  OnChanges,
  Renderer2,
  SimpleChanges,
  ViewEncapsulation,
} from '@angular/core';
import { LucideAngularModule, LucideIconData } from 'lucide-angular';

@Component({
  selector: 'button[appButton]',
  standalone: true,
  imports: [LucideAngularModule],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css',
  encapsulation: ViewEncapsulation.None,
})
export class ButtonComponent implements OnChanges {
  readonly CONTENT_TRANSPARENT_CLASS = 'app-button--transparent';
  readonly CONTENT_TRANSITION_TIME = 200;

  @Input({ required: false })
  type: 'normal' | 'inline' | 'none' = 'none';

  @Input({ required: false })
  color: 'primary' | 'danger' | 'text' | 'outline' | 'none' = 'none';

  @Input({ required: false })
  iconClass?: string;

  @Input({ required: false }) input?: ButtonComponentInput;

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
        break;
      case 'outline':
        data.colorClass = 'app-button--outline';
        break;
    }

    return 'app-button ' + data.typeClass + ' ' + data.colorClass;
  }

  currentInput?: ButtonComponentInput;

  constructor(
    private elementRef: ElementRef<HTMLButtonElement>,
    private renderer: Renderer2,
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['input']) {
      if (
        this.currentInput === undefined ||
        (this.currentInput?.label === this.input?.label && this.currentInput?.icon === this.input?.icon)
      ) {
        this.currentInput = this.input;
      } else {
        this.renderer.addClass(this.elementRef.nativeElement, this.CONTENT_TRANSPARENT_CLASS);
        setTimeout(() => {
          this.currentInput = this.input;
          this.renderer.removeClass(this.elementRef.nativeElement, this.CONTENT_TRANSPARENT_CLASS);
        }, this.CONTENT_TRANSITION_TIME);
      }
    }
  }
}

export interface ButtonComponentInput {
  icon?: LucideIconData;
  label?: string;
}
