import {
  Component,
  DestroyRef,
  ElementRef,
  Inject,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { LucideAngularModule, Images, Plus, SquareStar, Star, Trash2 } from 'lucide-angular';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { SpinnerLoaderComponent } from '../../../../shared/components/spinner-loader/spinner-loader.component';
import { MediaStatusPollingService } from '../../../../shared/services/media-status-polling.service';
import { MediaFile } from '../../../../core/models/media-file/media-file.model';
import { PropertyService } from '../../../../core/services/abstractions/property.service';
import { PROPERTY_SERVICE_TOKEN } from '../../../../core/services/tokens/property.token';
import { MediaFileVariantType } from '../../../../core/models/media-file/media-file-variant-type.model';
import { ApiError } from '../../../../core/exceptions/api-error';
import { BehaviorSubject, debounceTime, merge, startWith } from 'rxjs';
import { LocalDestroyRef } from '../../../../shared/lifecycles/local-destroy-ref';
import { patchMapWithList } from '../../../../shared/utils/patch-util';
import { AsyncPipe } from '@angular/common';
import { ObservableMap } from '../../../../shared/models/observable-map.model';

type NewMediaFileRow = {
  kind: 'new';
  data: MediaFile;
};

type MediaFileRow = {
  kind: 'existing';
  data: MediaFile;
  disableActions: boolean;
  destoryPollingRef: LocalDestroyRef;
};

@Component({
  selector: 'section[appPropertyMedia]',
  templateUrl: './property-media.component.html',
  styleUrls: ['./property-media.component.css'],
  standalone: true,
  imports: [
    ButtonComponent,
    PanelComponent,
    SkeletonLoaderComponent,
    SpinnerLoaderComponent,
    AsyncPipe,
    LucideAngularModule,
  ],
})
export class PropertyMediaComponent implements OnChanges, OnInit, OnDestroy {
  readonly ICONS = { Images, Plus, Star, SquareStar, Trash2 };
  private tempRowID: number = -1;

  private images: ObservableMap<number, MediaFileRow>;
  private newImages: ObservableMap<number, NewMediaFileRow>;

  imageList$: BehaviorSubject<(NewMediaFileRow | MediaFileRow)[]>;

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  @Input({ alias: 'appPropertyMedia' }) imageList?: MediaFile[];
  @Input({ required: true }) propertyId!: number;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private destroyRef: DestroyRef,
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    private fileStatusPollingService: MediaStatusPollingService,
  ) {
    this.images = new ObservableMap<number, MediaFileRow>();
    this.newImages = new ObservableMap<number, NewMediaFileRow>();
    this.imageList$ = new BehaviorSubject<(NewMediaFileRow | MediaFileRow)[]>([]);
  }

  // #region Lifecycle hooks
  ngOnInit(): void {
    // Code to sync-up list of rows when elements are added/removed
    const subscription = merge(this.images.valueChange, this.newImages.valueChange)
      .pipe(startWith(null), debounceTime(100))
      .subscribe(() => {
        this.syncImageList();
      });

    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['imageList']) {
      const value: MediaFile[] = changes['imageList'].currentValue ?? [];

      patchMapWithList(
        this.images,
        value,
        (img) => img.id,
        this.createImageRow.bind(this),
        this.mergeImageRow.bind(this),
        this.deleteImageRow.bind(this),
      );
    }
  }

  ngOnDestroy(): void {
    this.images.forEach((row) => row.destoryPollingRef.destroy());
  }
  // #endregion

  // #region Helper methods
  private createNewImageRow(mediaFile: MediaFile): NewMediaFileRow {
    const data: NewMediaFileRow = { kind: 'new', data: mediaFile };
    return data;
  }

  private createImageRow(mediaFile: MediaFile): MediaFileRow {
    const data: MediaFileRow = {
      kind: 'existing',
      data: mediaFile,
      disableActions: false,
      destoryPollingRef: new LocalDestroyRef(),
    };

    this.setupPolling(data);
    return data;
  }

  private mergeImageRow(mediaFileRow: MediaFileRow, mediaFile: MediaFile): MediaFileRow {
    // Destroy previous polling if any
    mediaFileRow.destoryPollingRef.destroy();
    mediaFileRow.data = mediaFile;
    this.setupPolling(mediaFileRow);
    return mediaFileRow;
  }

  private deleteImageRow(mediaFileRow: MediaFileRow) {
    mediaFileRow.destoryPollingRef.destroy();
  }

  private setupPolling(mediaFileRow: MediaFileRow) {
    if (mediaFileRow.data.processingStatus !== 'uploading' && mediaFileRow.data.processingStatus !== 'uploaded') return;

    const subscription = this.fileStatusPollingService.getStatusObservable(mediaFileRow.data.id).subscribe({
      next: (data) => {
        if (data.processingStatus === 'deleted' || data.processingStatus === 'failed') {
          const existingRow = this.images.get(data.id);
          if (existingRow) this.deleteImageRow(existingRow);
          this.images.delete(data.id);
        } else {
          const existingRow = this.images.get(data.id);
          if (existingRow) existingRow.data = data;
        }
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        // Only stop polling
        const existingRow = this.images.get(mediaFileRow.data.id);
        if (existingRow) existingRow.destoryPollingRef.destroy();
      },
    });

    mediaFileRow.destoryPollingRef.onDestroy(() => subscription.unsubscribe());
  }

  private syncImageList() {
    const imageList = [...this.images.values(), ...this.newImages.values()];
    this.imageList$.next(imageList);
  }

  generatePropertyUrl(mediaFile: MediaFile, variant: MediaFileVariantType): string {
    return this.propertyService.generatePropertyMediaUrl(mediaFile.id, variant);
  }
  // #endregion

  // #region Event handlers
  onFileInputClick() {
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
        markedAsCover: false,
      };
      const newMediaFileRow: NewMediaFileRow = this.createNewImageRow(newMediaFile);
      this.newImages.set(newFileId, newMediaFileRow);

      this.propertyService.uploadMediaFile(file, this.propertyId).subscribe({
        next: (image) => {
          this.newImages.delete(newFileId);
          this.images.set(image.id, this.createImageRow(image));
        },
        error: (err) => {
          if (err instanceof ApiError) {
            console.log('API Error', err.Errors);
          } else {
            console.error(err);
          }

          this.newImages.delete(newFileId);
        },
      });
    }
  }

  onMarkAsCover(row: MediaFileRow) {
    // Disable action
    const existingRow = this.images.get(row.data.id);
    if (existingRow) existingRow.disableActions = true;

    this.propertyService.markMediaFileAsCover(this.propertyId, row.data.id).subscribe({
      next: (image) => {
        for (let [_, image] of this.images) {
          image.data.markedAsCover = false;
        }

        const existingRow = this.images.get(row.data.id);
        if (existingRow) {
          existingRow.data.markedAsCover = true;
          existingRow.disableActions = false;
          console.log(existingRow);
        }
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        const existingRow = this.images.get(row.data.id);
        if (existingRow) existingRow.disableActions = false;
      },
    });
  }

  onRemoveClick(row: MediaFileRow) {
    // Disable action
    const existingRow = this.images.get(row.data.id);
    if (existingRow) existingRow.disableActions = true;

    this.propertyService.deleteMediaFile(row.data.id).subscribe({
      next: (deletedUnit) => {
        const existingRow = this.images.get(deletedUnit.id);
        if (existingRow) this.deleteImageRow(existingRow);
        this.images.delete(deletedUnit.id);
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        const existingRow = this.images.get(row.data.id);
        if (existingRow) existingRow.disableActions = false;
      },
    });
  }
  // #endregion
}
