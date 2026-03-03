import {
  Component,
  DestroyRef,
  Input,
  Inject,
  OnChanges,
  OnInit,
  SimpleChanges,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { FileText, Plus, Download, Trash2, LucideAngularModule } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { MediaFile } from '../../../../core/models/media-file/media-file.model';
import { LocalDestroyRef } from '../../../../shared/lifecycles/local-destroy-ref';
import { BehaviorSubject, debounceTime, merge, startWith } from 'rxjs';
import { ObservableMap } from '../../../../shared/models/observable-map.model';
import { AsyncPipe, DatePipe } from '@angular/common';
import { FileSizePipe } from '../../../../shared/pipes/file-size.pipe';
import { FileTypePipe } from '../../../../shared/pipes/file-type.pipe';
import { patchMapWithList } from '../../../../shared/utils/patch-util';
import { MediaStatusPollingService } from '../../../../shared/services/media-status-polling.service';
import { ApiError } from '../../../../core/exceptions/api-error';
import { TENANT_SERVICE_TOKEN } from '../../../../core/services/tokens/tenant.token';
import { TenantService } from '../../../../core/services/abstractions/tenant.service';
import { SpinnerLoaderComponent } from '../../../../shared/components/spinner-loader/spinner-loader.component';
import { NewMediaFileRow, MediaFileRow } from '../../models/tenant-document-row.model';

@Component({
  selector: 'section[appTenantDocuments]',
  templateUrl: './tenant-documents.component.html',
  styleUrl: './tenant-documents.component.css',
  standalone: true,
  imports: [
    AsyncPipe,
    PanelComponent,
    ButtonComponent,
    SkeletonLoaderComponent,
    LucideAngularModule,
    DatePipe,
    FileSizePipe,
    FileTypePipe,
    SpinnerLoaderComponent,
  ],
})
export class TenantDocumentsComponent implements OnInit, OnChanges {
  readonly ICONS = { FileText, Plus, Download, Trash2 };

  private documents: ObservableMap<number, MediaFileRow>;
  private newDocuments: ObservableMap<number, NewMediaFileRow>;
  private tempRowID: number = -1;

  documentList$: BehaviorSubject<(NewMediaFileRow | MediaFileRow)[]>;

  @Input({ required: true }) tenantId!: number;
  @Input({ alias: 'appTenantDocuments', required: false }) documentList?: MediaFile[];
  @Input({ required: false }) loading: boolean = false;

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  constructor(
    private destroyRef: DestroyRef,
    @Inject(TENANT_SERVICE_TOKEN) private tenantService: TenantService,
    private fileStatusPollingService: MediaStatusPollingService,
  ) {
    this.documents = new ObservableMap<number, MediaFileRow>();
    this.newDocuments = new ObservableMap<number, NewMediaFileRow>();
    this.documentList$ = new BehaviorSubject<(NewMediaFileRow | MediaFileRow)[]>([]);
  }

  // #region Lifecycle hooks
  ngOnInit() {
    // Code to sync-up list of rows when elements are added/removed
    const subscription = merge(this.documents.valueChange, this.newDocuments.valueChange)
      .pipe(startWith(null), debounceTime(100))
      .subscribe(() => {
        this.syncImageList();
      });

    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['documentList']) {
      const value: MediaFile[] = changes['documentList'].currentValue ?? [];

      patchMapWithList(
        this.documents,
        value,
        (img) => img.id,
        this.createDocumentRow.bind(this),
        this.mergeDocumentRow.bind(this),
        this.deleteDocumentRow.bind(this),
      );
    }
  }
  // #endregion

  // #region Helper methods
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
          const existingRow = this.documents.get(data.id);
          if (existingRow) this.deleteDocumentRow(existingRow);
          this.documents.delete(data.id);
        } else {
          const existingRow = this.documents.get(data.id);
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
        const existingRow = this.documents.get(mediaFileRow.data.id);
        if (existingRow) existingRow.destoryPollingRef.destroy();
      },
    });

    mediaFileRow.destoryPollingRef.onDestroy(() => subscription.unsubscribe());
  }

  private syncImageList() {
    const documentList = [...this.documents.values(), ...this.newDocuments.values()];
    this.documentList$.next(documentList);
  }
  // #endregion

  // #region Event handlers
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

      const newFileId = this.tempRowID--;
      const newMediaFile: MediaFile = {
        id: newFileId,
        name: file.name,
        processingStatus: 'uploading',
      };

      const newMediaFileRow: NewMediaFileRow = this.createNewDocumentRow(newMediaFile);
      this.newDocuments.set(newFileId, newMediaFileRow);

      this.tenantService.uploadTenantDocument(file, this.tenantId).subscribe({
        next: (file) => {
          this.newDocuments.delete(newFileId);
          this.documents.set(file.id, this.createDocumentRow(file));
        },
        error: (err) => {
          if (err instanceof ApiError) {
            console.log('API Error', err.Errors);
          } else {
            console.error(err);
          }

          this.newDocuments.delete(newFileId);
        },
      });
    }
  }

  onRemoveClick(doc: MediaFileRow | NewMediaFileRow) {
    if (doc.kind === 'new') return;

    const row = this.documents.get(doc.data.id);
    if (row) {
      row.disableActions = true;
    }

    this.tenantService.deleteTenantMedia(doc.data.id).subscribe({
      next: (deletedDoc) => {
        const row = this.documents.get(deletedDoc.id);
        if (row) this.deleteDocumentRow(row);
        this.documents.delete(deletedDoc.id);
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        const row = this.documents.get(doc.data.id);
        if (row) {
          row.disableActions = false;
        }
      },
    });
  }
  // #endregion
}
