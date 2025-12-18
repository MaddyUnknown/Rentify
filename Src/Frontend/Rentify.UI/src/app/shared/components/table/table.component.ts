import { Component, ContentChildren, HostBinding, Input, QueryList } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgTemplateOutlet } from '@angular/common';
import { TableColumnDirective } from './table-column.directive';

@Component({
  selector: 'table[appTable]',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css'],
  standalone: true,
  imports: [FormsModule, NgTemplateOutlet],
})
export class TableComponent<T, ID> {
  // Used for grid template layout
  @HostBinding('style.--num-of-column')
  get numOfColumn() {
    return this.columns.length ?? 0;
  }

  @Input({ required: true }) data!: T[];

  @ContentChildren(TableColumnDirective) columns!: QueryList<TableColumnDirective<T>>;
}
