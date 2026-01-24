import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { ButtonComponent } from '../button/button.component';
import { ChevronLeft, ChevronRight } from 'lucide-angular';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css'],
  standalone: true,
  imports: [ButtonComponent],
})
export class PaginationComponent implements OnChanges {
  readonly ICONS = { ChevronLeft, ChevronRight };
  readonly MAX_VISIBLE_PAGES = 10;

  @Input({ required: true }) totalItems!: number;
  @Input({ required: true }) pageSize!: number;
  @Input({ required: true }) currentPage!: number;

  @Output() pageChange = new EventEmitter<number>();

  pages: number[] = [];

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['currentPage']) {
      this.updatePages();
    }
  }

  onPageClick(page: number) {
    if (page < 1 || page > this.totalPages || page == this.currentPage) return;
    this.pageChange.emit(page);
  }

  private updatePages() {
    const half = Math.floor(this.MAX_VISIBLE_PAGES / 2);
    let start = Math.max(1, this.currentPage - half);
    let end = Math.min(this.totalPages, start + this.MAX_VISIBLE_PAGES - 1);

    if (end - start + 1 < this.MAX_VISIBLE_PAGES) {
      start = Math.max(1, end - this.MAX_VISIBLE_PAGES + 1);
    }

    this.pages = Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }
}
