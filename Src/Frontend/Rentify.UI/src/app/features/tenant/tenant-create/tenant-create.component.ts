import { Component, DestroyRef, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CircleX, FileText, Plus, Save, Download, Trash2, Phone, InfoIcon, Camera } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { PanelComponent } from '../../../shared/components/panel/panel.component';
import { ROUTE_SERVICE_TOKEN } from '../../../core/services/tokens/route.token';
import { RouteService } from '../../../core/services/abstractions/route.service';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { SpinnerLoaderComponent } from '../../../shared/components/spinner-loader/spinner-loader.component';
import { AsyncPipe, DatePipe } from '@angular/common';
import { FileTypePipe } from '../../../shared/pipes/file-type.pipe';
import { FileSizePipe } from '../../../shared/pipes/file-size.pipe';
import { MediaFile } from '../../../core/models/media-file/media-file.model';
import { LocalDestroyRef } from '../../../shared/lifecycles/local-destroy-ref';
import { BehaviorSubject, debounceTime, merge, startWith } from 'rxjs';
import { ObservableMap } from '../../../shared/models/observable-map.model';
import { MediaStatusPollingService } from '../../../shared/services/media-status-polling.service';
import { ApiError } from '../../../core/exceptions/api-error';
import { TenantService } from '../../../core/services/abstractions/tenant.service';
import { TENANT_SERVICE_TOKEN } from '../../../core/services/tokens/tenant.token';
import { NewMediaFileRow, MediaFileRow } from '../models/tenant-document-row.model';
import { TenantDetailsForm } from '../models/tenant-details-form.model';
import { TenantEmergencyContactForm } from '../models/tenant-emergency-contact-form.model';
import { TenantCreateForm } from '../models/tenant-create-form.model';
import { TenantMediaFileForm } from '../models/tenant-media-file-form.model';

@Component({
  selector: 'app-tenant-create',
  standalone: true,
  imports: [
    ButtonComponent,
    PanelComponent,
    ReactiveFormsModule,
    RouterLink,
    LucideAngularModule,
    SpinnerLoaderComponent,
    AsyncPipe,
    DatePipe,
    FileTypePipe,
    FileSizePipe,
  ],
  templateUrl: './tenant-create.component.html',
  styleUrl: './tenant-create.component.css',
})
export class TenantCreateComponent implements OnInit {
  readonly ICONS = { Camera, CircleX, FileText, InfoIcon, Phone, Plus, Save, Download, Trash2 };

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  private tenantDocumentState: {
    documents: ObservableMap<number, MediaFileRow>;
    newDocuments: ObservableMap<number, NewMediaFileRow>;
    newDocumentTempId: number;
  };

  disableActions = false;
  tenantForm: FormGroup<TenantCreateForm>;

  documentList$: BehaviorSubject<(NewMediaFileRow | MediaFileRow)[]>;

  constructor(
    private fb: FormBuilder,
    private destroyRef: DestroyRef,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
    @Inject(TENANT_SERVICE_TOKEN) private tenantService: TenantService,
    private fileStatusPollingService: MediaStatusPollingService,
    private router: Router,
  ) {
    this.documentList$ = new BehaviorSubject<(NewMediaFileRow | MediaFileRow)[]>([]);

    this.tenantForm = this.fb.group<TenantCreateForm>({
      details: this.fb.group<TenantDetailsForm>({
        name: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        email: this.fb.nonNullable.control<string>('', { validators: [Validators.required, Validators.email] }),
        phoneNumber: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        dob: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        employment: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        streetName: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        city: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        state: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        zipCode: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        note: this.fb.nonNullable.control<string>(''),
      }),
      emergencyContact: this.fb.group<TenantEmergencyContactForm>({
        id: this.fb.nonNullable.control<number>(0),
        name: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        relationship: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        phoneNumber: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
        email: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      }),
      documents: this.fb.array<FormGroup<TenantMediaFileForm>>([]),
    });

    this.tenantDocumentState = {
      documents: new ObservableMap<number, MediaFileRow>(),
      newDocuments: new ObservableMap<number, NewMediaFileRow>(),
      newDocumentTempId: -1,
    };
  }

  // #region Lifecycle hooks
  ngOnInit() {
    // Code to sync-up list of rows when elements are added/removed
    const subscription = merge(
      this.tenantDocumentState.documents.valueChange,
      this.tenantDocumentState.newDocuments.valueChange,
    )
      .pipe(startWith(null), debounceTime(100))
      .subscribe(() => {
        this.syncImageList();
      });

    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
  // #endregion

  // #region Helper methods
  tenantsRoute() {
    return this.routeService.tenants();
  }

  isInvalid(control: AbstractControl): boolean {
    return control.invalid && (control.dirty || control.touched);
  }
  // #endregion

  // #region Event handlers
  onSaveClick() {
    if (!this.tenantForm.valid) {
      this.tenantForm.markAllAsTouched();
      return;
    }

    if (this.tenantDocumentState.newDocuments.size > 0) return;

    this.tenantService.createTenant(this.tenantForm.getRawValue()).subscribe({
      next: (tenant) => {
        this.disableActions = false;
        this.router.navigate(this.routeService.tenant(tenant.id));
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

    this.disableActions = true;
    setTimeout(() => {
      this.disableActions = false;
    }, 800);
  }
  // #endregion

  // #region Helper methods - Documents
  private createNewDocumentRow(mediaFile: MediaFile): NewMediaFileRow {
    const data: NewMediaFileRow = { kind: 'new', data: mediaFile };
    return data;
  }

  private createDocumentRow(mediaFile: MediaFile): MediaFileRow {
    const data: MediaFileRow = {
      kind: 'existing',
      data: mediaFile,
      disableActions: false,
      destoryPollingRef: new LocalDestroyRef(),
    };

    this.setupPolling(data);
    return data;
  }

  private mergeDocumentRow(mediaFileRow: MediaFileRow, mediaFile: MediaFile): MediaFileRow {
    // Destroy previous polling if any
    mediaFileRow.destoryPollingRef.destroy();
    mediaFileRow.data = mediaFile;
    this.setupPolling(mediaFileRow);
    return mediaFileRow;
  }

  private deleteDocumentRow(mediaFileRow: MediaFileRow) {
    mediaFileRow.destoryPollingRef.destroy();
  }

  private setupPolling(mediaFileRow: MediaFileRow) {
    if (mediaFileRow.data.processingStatus !== 'uploading' && mediaFileRow.data.processingStatus !== 'uploaded') return;

    const subscription = this.fileStatusPollingService.getStatusObservable(mediaFileRow.data.id).subscribe({
      next: (data) => {
        if (data.processingStatus === 'deleted' || data.processingStatus === 'failed') {
          const existingRow = this.tenantDocumentState.documents.get(data.id);
          if (existingRow) this.deleteDocumentRow(existingRow);
          this.tenantDocumentState.documents.delete(data.id);
        } else {
          const existingRow = this.tenantDocumentState.documents.get(data.id);
          if (existingRow) existingRow.data = { ...existingRow.data, ...data };
        }
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        // Only stop polling
        const existingRow = this.tenantDocumentState.documents.get(mediaFileRow.data.id);
        if (existingRow) existingRow.destoryPollingRef.destroy();
      },
    });

    mediaFileRow.destoryPollingRef.onDestroy(() => subscription.unsubscribe());
  }

  private syncImageList() {
    const documentList = [
      ...this.tenantDocumentState.documents.values(),
      ...this.tenantDocumentState.newDocuments.values(),
    ];
    this.documentList$.next(documentList);
  }
  // #endregion

  // #region Event handlers - Documents
  onAddClick() {
    // Placeholder for add action
    this.fileInput?.nativeElement.click();
  }

  onFileSelected() {
    const files = this.fileInput?.nativeElement.files;
    if (!files || files.length === 0) return;

    for (let i = 0; i < files.length; i++) {
      const file = files.item(i);
      if (!file) continue;

      const newFileId = this.tenantDocumentState.newDocumentTempId--;
      const newMediaFile: MediaFile = {
        id: newFileId,
        name: file.name,
        processingStatus: 'uploading',
      };

      const newMediaFileRow: NewMediaFileRow = this.createNewDocumentRow(newMediaFile);
      this.tenantDocumentState.newDocuments.set(newFileId, newMediaFileRow);

      this.tenantService.uploadTenantDocument(file).subscribe({
        next: (file) => {
          this.tenantDocumentState.newDocuments.delete(newFileId);
          this.tenantDocumentState.documents.set(file.id, this.createDocumentRow(file));

          //Push to form
          this.tenantForm.controls.documents.push(
            this.fb.nonNullable.group<TenantMediaFileForm>({
              id: this.fb.nonNullable.control(file.id),
            }),
          );
        },
        error: (err) => {
          if (err instanceof ApiError) {
            console.log('API Error', err.Errors);
          } else {
            console.error(err);
          }

          this.tenantDocumentState.newDocuments.delete(newFileId);
        },
      });
    }
  }

  onRemoveClick(doc: MediaFileRow | NewMediaFileRow) {
    if (doc.kind === 'new') return;

    const row = this.tenantDocumentState.documents.get(doc.data.id);
    if (row) {
      row.disableActions = true;
    }

    this.tenantService.deleteTenantDocument(doc.data.id).subscribe({
      next: (deletedDoc) => {
        const row = this.tenantDocumentState.documents.get(deletedDoc.id);
        if (row) this.deleteDocumentRow(row);
        this.tenantDocumentState.documents.delete(deletedDoc.id);

        //Remove from form
        const index = this.tenantForm.controls.documents.controls.findIndex(
          (x) => x.controls.id.value === deletedDoc.id,
        );
        if (index !== -1) {
          this.tenantForm.controls.documents.removeAt(index);
        }
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        const row = this.tenantDocumentState.documents.get(doc.data.id);
        if (row) {
          row.disableActions = false;
        }
      },
    });
  }
  // #endregion
}
