import { Directive, Input, TemplateRef } from '@angular/core';

@Directive({
  selector: 'ng-template[appTableColumn]',
  standalone: true,
})
export class TableColumnDirective<T> {
  @Input({ alias: 'appTableColumn', required: true }) displayName!: string;
  @Input({ required: false }) columnAlignment: 'left' | 'center' | 'right' = 'left';
  @Input({ required: true }) type!: T;

  constructor(public template: TemplateRef<{ $implicit: T }>) {}

  static ngTemplateContextGuard<T>(dir: TableColumnDirective<T>, ctx: unknown): ctx is { $implicit: T } {
    return true;
  }
}
