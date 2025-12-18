import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CircleX, InfoIcon, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { FormComponent } from '../../../../shared/components/form/form.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { PropertyDetails, UpdatePropertyDetails } from '../../../../core/models/property.model';
import { UIEditState } from '../../../../shared/models/edit-ui-state.model';
import { PropertyService } from '../../../../core/services/property.service';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';

@Component({
  selector: 'section[appPropertyDetails]',
  templateUrl: './property-details.component.html',
  styleUrls: ['./property-details.component.css'],
  standalone: true,
  imports: [FormsModule, ButtonComponent, FormComponent, PanelComponent, SkeletonLoaderComponent],
})
export class PropertyDetailsComponent implements OnChanges {
  readonly ICONS = { CircleX, InfoIcon, Save, SquarePen, Trash2 };
  private readonly VALIDATORS: { [K in keyof PropertyDetails]?: (value: any) => boolean } = {
    name: (value: string) => value.length > 0,
    streetName: (value: string) => value.length > 0,
    city: (value: string) => value.length > 0,
    state: (value: string) => value.length > 0,
    zipCode: (value: string) => value.length > 0,
  };

  propertyDetails: UIEditState<PropertyDetails> = {
    data: this.emptyPropertyDetails,
    mode: EditMode.from('view'),
  };

  @Input({ required: true }) propertyId!: number;
  @Input({ alias: 'appPropertyDetails', required: false }) details?: PropertyDetails;
  @Input({ required: false }) loading: boolean = false;

  constructor(private propertyService: PropertyService) {}

  //#region Lifecycle hooks
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['details']) {
      const value: PropertyDetails = changes['details'].currentValue ?? this.emptyPropertyDetails;

      this.propertyDetails = {
        data: this.propertyDetails.mode.isView ? value : this.propertyDetails.data,
        mode: this.propertyDetails.mode,
        previousData: this.propertyDetails.mode.isView ? undefined : value,
      };
    }
  }
  //#endregion

  private get emptyPropertyDetails(): PropertyDetails {
    return {
      name: '',
      streetName: '',
      city: '',
      state: '',
      zipCode: '',
      description: '',
    };
  }

  //#region Validators
  private isUnitValid(unit: PropertyDetails): {
    isValid: boolean;
    propertyHasError: { [K in keyof PropertyDetails]?: boolean };
  } {
    let isValid = true;
    const propertyHasError: Partial<Record<keyof PropertyDetails, boolean>> = {};

    for (const key in this.VALIDATORS) {
      const k = key as keyof PropertyDetails;
      propertyHasError[k] = !this.VALIDATORS[k]!(unit[k]);
      isValid = isValid && !propertyHasError[k];
    }

    return { isValid, propertyHasError };
  }

  private isUnitPropertyValid(propertyName: keyof PropertyDetails, value: any): boolean {
    const validator = this.VALIDATORS[propertyName];
    if (!validator || validator(value)) {
      return true;
    } else {
      return false;
    }
  }
  //#endregion

  //#region Event handlers
  onAddEditClick() {
    if (this.propertyDetails.mode.isView) {
      // (Edit clicked) Toggle to edit mode
      this.propertyDetails.previousData = structuredClone(this.propertyDetails.data);
      this.propertyDetails.mode.toggle();
    } else {
      // Save units
      const validation = this.isUnitValid(this.propertyDetails.data);
      this.propertyDetails.isInvalid = validation.propertyHasError;
      if (validation.isValid) {
        this.updateProperty(this.propertyDetails.data);
      } else {
        const shake: Partial<Record<keyof PropertyDetails, boolean>> = {};
        for (const key in validation.propertyHasError) {
          const k = key as keyof PropertyDetails;
          if (validation.propertyHasError[k]) {
            shake[k] = true;
          }
        }
        this.propertyDetails.doShake = shake;
      }
    }
  }

  onRemoveCloseClick() {
    if (this.propertyDetails.mode.isView) {
      // (Delete clicked) Remove unit
      this.removeProperty(this.propertyDetails.data);
    } else {
      // (Close clicked) Toggle to view mode
      this.propertyDetails.data = this.propertyDetails.previousData ?? this.propertyDetails.data;
      this.propertyDetails.previousData = undefined;
      this.propertyDetails.isInvalid = {};
      this.propertyDetails.mode.toggle();
    }
  }

  onInputChange(propertyName: keyof PropertyDetails) {
    this.propertyDetails.isInvalid = this.propertyDetails.isInvalid ?? {};
    this.propertyDetails.isInvalid[propertyName] = !this.isUnitPropertyValid(
      propertyName,
      this.propertyDetails.data[propertyName],
    );
  }
  //#endregion

  //#region Service Calls
  private updateProperty(data: PropertyDetails) {
    // Disable action
    this.propertyDetails.isActionDisabled = true;

    // Save new unit details
    const updatePropertyDetails: UpdatePropertyDetails = {
      id: this.propertyId,
      name: data.name,
      streetName: data.streetName,
      city: data.city,
      state: data.state,
      zipCode: data.zipCode,
      description: data.description,
    };

    this.propertyService.updateProperty(updatePropertyDetails).subscribe({
      next: (propertyDetails) => {
        const updatedData: UIEditState<PropertyDetails> = {
          data: {
            name: propertyDetails.name,
            streetName: propertyDetails.streetName,
            city: propertyDetails.city,
            state: propertyDetails.state,
            zipCode: propertyDetails.zipCode,
            description: propertyDetails.description,
          },
          mode: EditMode.from('view'),
        };

        //Update received property
        this.propertyDetails = updatedData;
      },
      error: (err) => console.error(err),
    });
  }

  private removeProperty(data: PropertyDetails) {
    // Disable action - TO-Do
    this.propertyDetails.isActionDisabled = true;
    console.log('Delete: ', data);

    setTimeout(() => {
      this.propertyDetails.isActionDisabled = false;
    }, 2000);

    // this.propertyService.deleteProperty(this.propertyId).subscribe({
    //   next: () => {
    //     //What to do on delete? Redirect to property pages.
    //   },
    //   error: (err) => console.error(err),
    //});
  }
  //#endregion
}
