import { Component, Input } from '@angular/core';
import { PanelComponent } from '../../../common/panel/panel.component';
import { Images, Plus } from 'lucide-angular';
import { ButtonComponent } from '../../../common/button/button.component';
import { PropertyImageMetadata } from '../../../../models/property.model';

@Component({
  selector: 'section[appPropertyMedia]',
  templateUrl: './property-media.component.html',
  styleUrls: ['./property-media.component.css'],
  standalone: true,
  imports: [ButtonComponent, PanelComponent],
})
export class PropertyMediaComponent {
  readonly ICONS = { Images, Plus };

  @Input({ alias: 'appPropertyMedia', required: true }) imageList!: PropertyImageMetadata[];
}
