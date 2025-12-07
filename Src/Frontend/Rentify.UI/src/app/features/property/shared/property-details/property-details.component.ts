import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CircleX, InfoIcon, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { FormComponent } from '../../../../shared/components/form/form.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { PropertyDetails } from '../../../../core/models/property.model';

@Component({
  selector: 'section[appPropertyDetails]',
  templateUrl: './property-details.component.html',
  styleUrls: ['./property-details.component.css'],
  standalone: true,
  imports: [FormsModule, ButtonComponent, FormComponent, PanelComponent],
})
export class PropertyDetailsComponent {
  readonly ICONS = { CircleX, InfoIcon, Save, SquarePen, Trash2 };
  private readonly DEFAULT_PROPERTY_DETAILS: PropertyDetails = {
    name: '',
    streetName: '',
    city: '',
    state: '',
    zipCode: '',
    description: '',
  };

  details: PropertyDetails = this.DEFAULT_PROPERTY_DETAILS;
  panelMode: EditMode = new EditMode('view');

  @Input() set appPropertyDetails(value: PropertyDetails | undefined) {
    this.details = value ?? this.DEFAULT_PROPERTY_DETAILS;
  }
}
