import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CircleX, InfoIcon, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../common/panel/panel.component';
import { FormComponent } from '../../../common/form/form.component';
import { ButtonComponent } from '../../../common/button/button.component';
import { FormMode } from '../../../../utils/form/form-mode';
import { PropertyDetails } from '../../../../models/property.model';

@Component({
  selector: 'section[appPropertyDetails]',
  templateUrl: './property-details.component.html',
  styleUrls: ['./property-details.component.css'],
  standalone: true,
  imports: [FormsModule, ButtonComponent, FormComponent, PanelComponent],
})
export class PropertyDetailsComponent {
  readonly ICONS = { CircleX, InfoIcon, Save, SquarePen, Trash2 };

  panelMode: FormMode;

  @Input({ alias: 'appPropertyDetails', required: true }) details!: PropertyDetails;

  constructor() {
    this.panelMode = new FormMode();
  }
}
