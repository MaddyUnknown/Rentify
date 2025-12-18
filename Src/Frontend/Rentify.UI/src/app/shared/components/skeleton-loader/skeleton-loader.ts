import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-skeleton-loader',
  templateUrl: './skeleton-loader.html',
  styleUrls: ['./skeleton-loader.css'],
  standalone: true,
})
export class SkeletonLoaderComponent {
  @Input({ required: true }) type: 'box' | 'img' | 'input' = 'box';
}
