import {
  Component,
  DestroyRef,
  ElementRef,
  Inject,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { Camera, CircleX, InfoIcon, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouteService } from '../../../../core/services/abstractions/route.service';
import { ROUTE_SERVICE_TOKEN } from '../../../../core/services/tokens/route.token';
import { Router } from '@angular/router';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { TENANT_SERVICE_TOKEN } from '../../../../core/services/tokens/tenant.token';
import { TenantService } from '../../../../core/services/abstractions/tenant.service';
import { GetTenantDetails } from '../../../../core/models/tenant/get-tenant-details.model';
import { ApiError } from '../../../../core/exceptions/api-error';
import { TenantDetailsForm } from '../../models/tenant-details-form.model';
import { MediaFile } from '../../../../core/models/media-file/media-file.model';
import { MediaFileRow, NewMediaFileRow } from '../../models/tenant-document-row.model';
import { MediaFileVariantType } from '../../../../core/models/media-file/media-file-variant-type.model';
import { LocalDestroyRef } from '../../../../shared/lifecycles/local-destroy-ref';
import { MediaStatusPollingService } from '../../../../shared/services/media-status-polling.service';
import { SpinnerLoaderComponent } from '../../../../shared/components/spinner-loader/spinner-loader.component';

@Component({
  selector: 'section[appTenantDetails]',
  templateUrl: './tenant-details.component.html',
  styleUrl: './tenant-details.component.css',
  standalone: true,
  imports: [PanelComponent, ButtonComponent, SkeletonLoaderComponent, ReactiveFormsModule, SpinnerLoaderComponent],
})
export class TenantDetailsComponent implements OnInit, OnChanges {
  readonly ICONS = { Camera, CircleX, InfoIcon, Save, SquarePen, Trash2 };

  mode = EditMode.from('view');
  disableActions = false;
  form: FormGroup<TenantDetailsForm>;
  profilePicState: NewMediaFileRow | MediaFileRow | undefined;

  private originalState?: GetTenantDetails;

  @ViewChild('profilePicInput') profilePicInput?: ElementRef<HTMLInputElement>;

  @Input({ required: true }) tenantId!: number;
  @Input({ alias: 'appTenantDetails' }) details?: GetTenantDetails;
  @Input({ required: false }) profilePic?: MediaFile;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private destroyRef: DestroyRef,
    private fb: FormBuilder,
    @Inject(TENANT_SERVICE_TOKEN) private tenantService: TenantService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
    private fileStatusPollingService: MediaStatusPollingService,
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

    if (changes['profilePic']) {
      const value = changes['profilePic'].currentValue;

      if (this.profilePicState?.kind == 'existing') this.deleteMediaRow(this.profilePicState);
      this.profilePicState = value ? this.createMediaRow(value) : undefined;
    }
  }
  //#endregion

  //#region Helper methods
  generateTenantMediaUrl(mediaFile: MediaFile, variant: MediaFileVariantType): string {
    return this.tenantService.generateTenantMediaUrl(mediaFile.id, variant);
  }

  private createNewMediaRow(mediaFile: MediaFile): NewMediaFileRow {
    const data: NewMediaFileRow = { kind: 'new', data: mediaFile };
    return data;
  }

  private createMediaRow(mediaFile: MediaFile): MediaFileRow {
    const data: MediaFileRow = {
      kind: 'existing',
      data: mediaFile,
      disableActions: false,
      destoryPollingRef: new LocalDestroyRef(),
    };

    this.setupProfilePicPolling(data);
    return data;
  }

  private deleteMediaRow(mediaFileRow: MediaFileRow) {
    mediaFileRow.destoryPollingRef.destroy();
  }

  private setupProfilePicPolling(mediaFileRow: MediaFileRow) {
    if (mediaFileRow.data.processingStatus !== 'uploading' && mediaFileRow.data.processingStatus !== 'uploaded') return;

    const subscription = this.fileStatusPollingService.getStatusObservable(mediaFileRow.data.id).subscribe({
      next: (data) => {
        if (data.processingStatus === 'deleted' || data.processingStatus === 'failed') {
          if (this.profilePicState?.kind == 'existing') this.deleteMediaRow(this.profilePicState);
        } else {
          if (this.profilePicState !== undefined) {
            this.profilePicState.data = { ...this.profilePicState.data, ...data };
          } else {
            this.profilePicState = mediaFileRow;
          }
        }
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        // Only stop polling
        if (this.profilePicState?.kind === 'existing') this.profilePicState.destoryPollingRef.destroy();
      },
    });

    if (this.profilePicState?.kind === 'existing')
      this.profilePicState.destoryPollingRef.onDestroy(() => subscription.unsubscribe());
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

  onProfilePicClick() {
    // Placeholder for add action
    this.profilePicInput?.nativeElement.click();
  }

  onProfilePicSelected() {
    const files = this.profilePicInput?.nativeElement.files;
    console.log(files);
    if (!files || files.length === 0) return;

    const file = files.item(0);
    if (!file) return;

    const newFileId = -1;
    const newMediaFile: MediaFile = {
      id: newFileId,
      name: file.name,
      processingStatus: 'uploading',
    };

    const deletedProfilePicState = this.profilePicState;
    this.profilePicState = this.createNewMediaRow(newMediaFile);

    // Update profile pic
    this.tenantService.updateTenantProfilePic(file, this.tenantId).subscribe({
      next: (file) => {
        this.profilePicState = this.createMediaRow(file);
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        this.profilePicState = deletedProfilePicState;
      },
    });
  }
  //#endregion
}
