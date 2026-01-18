import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PropertyService } from '../../../core/services/abstractions/property.service';
import { PropertyDetailsComponent } from '../property-maintenance/property-details/property-details.component.old';
import { PropertyMediaComponent } from '../property-maintenance/property-media/property-media.component';
import { PropertyUnitsComponent } from '../property-maintenance/property-units/property-units.component';
import { PropertyLocationComponent } from '../property-maintenance/property-location/property-location.component';
import { PROPERTY_SERVICE_TOKEN } from '../../../core/services/tokens/property.token';
import { PropertyStateService } from '../services/property-state.service';
import { ApiError } from '../../../core/exceptions/api-error';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { Save } from 'lucide-angular';

@Component({
  selector: 'app-property-create',
  standalone: true,
  imports: [
    PropertyDetailsComponent,
    PropertyMediaComponent,
    PropertyUnitsComponent,
    PropertyLocationComponent,
    ButtonComponent,
  ],
  providers: [PropertyStateService],
  templateUrl: './property-create.component.html',
  styleUrl: './property-create.component.css',
})
export class PropertyCreateComponent implements OnInit, OnDestroy {
  readonly ICONS = { Save };

  isSubmitting: boolean = false;

  constructor(
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    private propertyStateService: PropertyStateService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    // Set service to create mode
    this.propertyStateService.setMode('create');
    this.propertyStateService.resetState();
  }

  ngOnDestroy(): void {
    // Clean up state when component is destroyed
    this.propertyStateService.resetState();
  }

  // Validate all sections before submission
  private validateForm(): { isValid: boolean; errors: string[] } {
    const errors: string[] = [];
    const state = this.propertyStateService.getFullState();

    // Validate property details
    if (!state.generalDetails) {
      errors.push('Property details are required');
      return { isValid: false, errors };
    }

    if (!state.generalDetails.name || state.generalDetails.name.trim().length === 0) {
      errors.push('Property name is required');
    }
    if (!state.generalDetails.streetName || state.generalDetails.streetName.trim().length === 0) {
      errors.push('Street name is required');
    }
    if (!state.generalDetails.city || state.generalDetails.city.trim().length === 0) {
      errors.push('City is required');
    }
    if (!state.generalDetails.state || state.generalDetails.state.trim().length === 0) {
      errors.push('State is required');
    }
    if (!state.generalDetails.zipCode || state.generalDetails.zipCode.trim().length === 0) {
      errors.push('Zip code is required');
    }

    // Validate units (if any)
    for (const unit of state.units) {
      if (!unit.name || unit.name.trim().length === 0) {
        errors.push(`Unit name is required for one or more units`);
        break;
      }
      if (!unit.type || unit.type.trim().length === 0) {
        errors.push(`Unit type is required for one or more units`);
        break;
      }
      if (!unit.size || unit.size <= 0) {
        errors.push(`Unit size must be greater than 0 for one or more units`);
        break;
      }
    }

    return {
      isValid: errors.length === 0,
      errors,
    };
  }

  // Submit handler - collects all data from service and calls API
  onSubmit() {
    const validation = this.validateForm();
    if (!validation.isValid) {
      console.error('Validation errors:', validation.errors);
      // TODO: Show validation errors to user via a notification service
      return;
    }

    this.isSubmitting = true;

    const state = this.propertyStateService.getFullState();

    // TODO: Implement createProperty API call
    // This is a placeholder - you'll need to add the createProperty method to PropertyService
    console.log('Creating property with data:', state);

    // Placeholder for API call
    // this.propertyService.createProperty({
    //   generalDetails: {
    //     name: state.generalDetails!.name,
    //     streetName: state.generalDetails!.streetName,
    //     city: state.generalDetails!.city,
    //     state: state.generalDetails!.state,
    //     zipCode: state.generalDetails!.zipCode,
    //     description: state.generalDetails!.description,
    //   },
    //   units: state.units.map(u => ({
    //     name: u.name,
    //     type: u.type,
    //     size: u.size,
    //   })),
    //   location: state.location,
    // }).subscribe({
    //   next: (property) => {
    //     // Upload media files after property is created (if any)
    //     // this.uploadMediaFiles(property.id);
    //     this.router.navigate(['/properties', property.id]);
    //   },
    //   error: (err) => {
    //     this.isSubmitting = false;
    //     if (err instanceof ApiError) {
    //       console.log('API Error', err.Errors);
    //     } else {
    //       console.error(err);
    //     }
    //   },
    // });

    // Temporary: simulate API call
    setTimeout(() => {
      this.isSubmitting = false;
      console.log('Property created successfully (simulated)');
      // this.router.navigate(['/properties']);
    }, 2000);
  }
}
