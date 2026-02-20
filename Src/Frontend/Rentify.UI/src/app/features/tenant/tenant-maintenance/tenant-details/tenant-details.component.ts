import { Component, DestroyRef, Inject, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Camera, CircleX, InfoIcon, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouteService } from '../../../../core/services/abstractions/route.service';
import { ROUTE_SERVICE_TOKEN } from '../../../../core/services/tokens/route.token';
import { Router } from '@angular/router';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { TENANT_SERVICE_TOKEN } from '../../../../core/services/tokens/tenant.token';
import { TenantService } from '../../../../core/services/abstractions/tenant.service';
import { GetTenantDetails } from '../../../../core/models/tenant/get-tenant-details.model';
import { ApiError } from '../../../../core/exceptions/api-error';

type TenantDetailsForm = {
  name: FormControl<string>;
  email: FormControl<string>;
  phoneNumber: FormControl<string>;
  dob: FormControl<string>;
  employment: FormControl<string>;
  streetName: FormControl<string>;
  city: FormControl<string>;
  state: FormControl<string>;
  zipCode: FormControl<string>;
  note: FormControl<string>;
};

@Component({
  selector: 'section[appTenantDetails]',
  templateUrl: './tenant-details.component.html',
  styleUrl: './tenant-details.component.css',
  standalone: true,
  imports: [PanelComponent, ButtonComponent, SkeletonLoaderComponent, ReactiveFormsModule],
})
export class TenantDetailsComponent implements OnInit, OnChanges {
  readonly ICONS = { Camera, CircleX, InfoIcon, Save, SquarePen, Trash2 };

  mode = EditMode.from('view');
  disableActions = false;
  form: FormGroup<TenantDetailsForm>;
  private originalState?: GetTenantDetails;

  @Input({ required: true }) tenantId!: number;
  @Input({ alias: 'appTenantDetails' }) details?: GetTenantDetails;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private destroyRef: DestroyRef,
    private fb: FormBuilder,
    @Inject(TENANT_SERVICE_TOKEN) private tenantService: TenantService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
    private router: Router,
  ) {
    this.form = this.fb.group<TenantDetailsForm>({
      name: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      email: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      phoneNumber: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      dob: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      employment: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      streetName: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      city: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      state: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      zipCode: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      note: this.fb.nonNullable.control<string>(''),
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
  onEditSaveClick() {
    if (this.mode.isView) {
      this.mode.toggle();
      return;
    }

    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    this.updateTenantDetails(this.form.getRawValue());
  }

  onCloseClearClick() {
    if (this.mode.isView) {
      this.removeTenant();
      return;
    }

    this.form.reset(this.originalState ?? {});
    this.mode.toggle();
  }
  //#endregion

  //#region Service Calls
  private updateTenantDetails(data: GetTenantDetails) {
    this.disableActions = true;

    this.tenantService.updateTenantDetails(this.tenantId, data).subscribe({
      next: (tenantDetails) => {
        this.originalState = tenantDetails;
        this.form.reset(tenantDetails);
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

  private removeTenant() {
    this.disableActions = true;

    this.tenantService.deleteTenant(this.tenantId).subscribe({
      next: () => {
        this.router.navigate(this.routeService.tenants());
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
