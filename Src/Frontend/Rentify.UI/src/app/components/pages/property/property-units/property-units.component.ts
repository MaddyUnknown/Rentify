import { Component, Input } from '@angular/core';
import { Blocks, Plus, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../common/panel/panel.component';
import { ButtonComponent } from '../../../common/button/button.component';

@Component({
  selector: 'section[appPropertyUnits]',
  templateUrl: './property-units.component.html',
  styleUrls: ['./property-units.component.css'],
  standalone: true,
  imports: [ButtonComponent, PanelComponent],
})
export class PropertyUnitsComponent {
  readonly ICONS = { Blocks, Plus, SquarePen, Trash2 };

  @Input({ alias: 'appPropertyUnits', required: true }) units!: string;
}
