import { Component, Input } from '@angular/core';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { Images, Plus } from 'lucide-angular';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { PropertyImageMetadata } from '../../../../core/models/property.model';

@Component({
  selector: 'section[appPropertyMedia]',
  templateUrl: './property-media.component.html',
  styleUrls: ['./property-media.component.css'],
  standalone: true,
  imports: [ButtonComponent, PanelComponent],
})
export class PropertyMediaComponent {
  readonly ICONS = { Images, Plus };

  @Input({ alias: 'appPropertyMedia' }) imageList?: PropertyImageMetadata[];
}
