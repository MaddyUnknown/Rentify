import { Component, DestroyRef, Inject, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CircleX, InfoIcon, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { PropertyService } from '../../../../core/services/abstractions/property.service';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { PROPERTY_SERVICE_TOKEN } from '../../../../core/services/tokens/property.token';
import { GetPropertyDetails } from '../../../../core/models/property/get-property-details.model';
import { ApiError } from '../../../../core/exceptions/api-error';
import { RouteService } from '../../../../core/services/abstractions/route.service';
import { ROUTE_SERVICE_TOKEN } from '../../../../core/services/tokens/route.token';
import { Router } from '@angular/router';

type PropertyDetailsForm = {
  name: FormControl<string>;
  streetName: FormControl<string>;
  city: FormControl<string>;
  state: FormControl<string>;
  zipCode: FormControl<string>;
  description: FormControl<string>;
};

@Component({
  selector: 'section[appPropertyDetails]',
  templateUrl: './property-details.component.html',
  styleUrls: ['./property-details.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, ButtonComponent, PanelComponent, SkeletonLoaderComponent],
})
export class PropertyDetailsComponent implements OnInit, OnChanges {
  readonly ICONS = { CircleX, InfoIcon, Save, SquarePen, Trash2 };

  mode = EditMode.from('view');
  disableActions = false;
  form: FormGroup<PropertyDetailsForm>;
  private originalState?: GetPropertyDetails;

  @Input({ required: true }) propertyId!: number;
  @Input({ alias: 'appPropertyDetails', required: false }) details?: GetPropertyDetails;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private destroyRef: DestroyRef,
    private fb: FormBuilder,
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
    private router: Router,
  ) {
    this.form = this.fb.group<PropertyDetailsForm>({
      name: this.fb.nonNullable.control('', { validators: [Validators.required] }),
      streetName: this.fb.nonNullable.control('', { validators: [Validators.required] }),
      city: this.fb.nonNullable.control('', { validators: [Validators.required] }),
      state: this.fb.nonNullable.control('', { validators: [Validators.required] }),
      zipCode: this.fb.nonNullable.control('', { validators: [Validators.required] }),
      description: this.fb.nonNullable.control(''),
    });
  }

  //#region Helper methods
  isInvalid(control: AbstractControl): boolean {
    return control.invalid && (control.dirty || control.touched);
  }
  //#endregion

  //#region Lifecycle hooks
  ngOnInit(): void {
    const subscription = this.mode.changes.subscribe((mode) =>
      mode === 'view' ? this.form.disable() : this.form.enable(),
    );
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['details']) {
      const value = changes['details'].currentValue;

      this.originalState = value ?? {};
      if (this.mode.isView) this.form.reset(value ?? {});
    }
  }
  //#endregion

  //#region Event handlers
  onAddEditClick() {
    if (this.mode.isView) {
      // (Edit clicked) Toggle to edit mode
      this.mode.toggle();
    } else {
      if (!this.form.valid) {
        this.form.markAllAsTouched();
        return;
      }

      // (Save click ) Save property details
      this.updateProperty(this.form.getRawValue());
    }
  }

  onRemoveCloseClick() {
    if (this.mode.isView) {
      // (Delete clicked) Remove property
      this.removeProperty(this.form.getRawValue());
    } else {
      // (Close clicked) Toggle to view mode
      this.form.reset(this.originalState ?? {});
      this.mode.toggle();
    }
  }
  //#endregion

  //#region Service Calls
  private updateProperty(data: GetPropertyDetails) {
    this.disableActions = true;

    this.propertyService.updateProperty(this.propertyId, data).subscribe({
      next: (propertyDetails) => {
        this.originalState = propertyDetails;
        this.form.reset(propertyDetails);
        this.mode.state = 'view';

        this.disableActions = false;
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        this.disableActions = false;
      },
    });
  }

  private removeProperty(data: GetPropertyDetails) {
    this.disableActions = true;

    setTimeout(() => {
      this.disableActions = false;
    }, 2000);

    // TO-DO: Delete action
    this.propertyService.deleteProperty(this.propertyId).subscribe({
      next: () => {
        this.router.navigate(this.routeService.propeties());
        this.disableActions = false;
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        this.disableActions = false;
      },
    });
  }
  //#endregion
}
