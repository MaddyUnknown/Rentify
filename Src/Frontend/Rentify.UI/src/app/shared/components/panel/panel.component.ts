import { Component, Input, ViewEncapsulation } from '@angular/core';
import { LucideAngularModule, LucideIconData } from 'lucide-angular';

@Component({
  selector: 'app-panel',
  standalone: true,
  imports: [LucideAngularModule],
  templateUrl: './panel.component.html',
  styleUrl: './panel.component.css',
  encapsulation: ViewEncapsulation.None,
  host: {
    class: 'app-panel',
  },
})
export class PanelComponent {
  @Input({ required: true }) icon!: LucideIconData;
  @Input({ required: true }) heading!: string;
}
