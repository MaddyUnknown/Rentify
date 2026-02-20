import { Component, DestroyRef, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CircleX, Phone, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { TENANT_SERVICE_TOKEN } from '../../../../core/services/tokens/tenant.token';
import { TenantService } from '../../../../core/services/abstractions/tenant.service';
import { TenantEmergencyContact } from '../../../../core/models/tenant/tenant-emergency-contact.model';
import { ApiError } from '../../../../core/exceptions/api-error';
import { Inject } from '@angular/core';

type TenantEmergencyContactForm = {
  name: FormControl<string>;
  relationship: FormControl<string>;
  phoneNumber: FormControl<string>;
  email: FormControl<string>;
};

@Component({
  selector: 'section[appTenantEmergencyContact]',
  templateUrl: './tenant-emergency-contact.component.html',
  styleUrl: './tenant-emergency-contact.component.css',
  standalone: true,
  imports: [PanelComponent, ButtonComponent, SkeletonLoaderComponent, ReactiveFormsModule],
})
export class TenantEmergencyContactComponent implements OnInit, OnChanges {
  readonly ICONS = { CircleX, Phone, Save, SquarePen };

  mode = EditMode.from('view');
  disableActions = false;
  form: FormGroup<TenantEmergencyContactForm>;
  private originalState?: TenantEmergencyContact;

  @Input({ required: true }) tenantId!: number;
  @Input({ alias: 'appTenantEmergencyContact', required: false }) contact?: TenantEmergencyContact;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private destroyRef: DestroyRef,
    private fb: FormBuilder,
    @Inject(TENANT_SERVICE_TOKEN) private tenantService: TenantService,
  ) {
    this.form = this.fb.group<TenantEmergencyContactForm>({
      name: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      relationship: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      phoneNumber: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      email: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
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
    if (changes['contact']) {
      const value = changes['contact'].currentValue ?? undefined;

      this.originalState = value ?? {};
      if (this.mode.isView) this.form.reset(value ?? {});
    }
  }
  //#endregion

  //#region Event handlers
  onSaveClick() {
    if (this.mode.isView) return;

    if (!this.form.valid) {
      this.form.markAllAsTouched();
      return;
    }

    this.updateEmergencyContact(this.form.getRawValue());
  }

  onEditCloseClick() {
    if (this.mode.isView) {
      this.mode.toggle();
      return;
    } else {
      this.form.reset(this.originalState ?? {});
      this.mode.toggle();
    }
  }
  //#endregion

  //#region Service Calls
  private updateEmergencyContact(data: TenantEmergencyContact) {
    this.disableActions = true;

    this.tenantService.updateTenantEmergencyContact(this.tenantId, data).subscribe({
      next: (contact) => {
        this.originalState = contact;
        this.form.reset(contact);
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

  private removeEmergencyContact() {
    this.disableActions = true;

    this.tenantService.deleteTenantEmergencyContact(this.tenantId).subscribe({
      next: () => {
        this.form.reset({});
        this.originalState = undefined;
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
