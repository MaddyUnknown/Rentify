import { Component, ElementRef, Inject, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { Images, Plus, Trash2 } from 'lucide-angular';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { SpinnerLoaderComponent } from '../../../../shared/components/spinner-loader/spinner-loader.component';
import { FileStatusPollingService } from '../../../../shared/services/file-status-polling.service';
import { UIState } from '../../../../shared/models/ui-state.model';
import { MediaFile } from '../../../../core/models/media-file/media-file.model';
import { PropertyService } from '../../../../core/services/abstractions/property.service';
import { PROPERTY_SERVICE_TOKEN } from '../../../../core/services/tokens/property.token';

@Component({
  selector: 'section[appPropertyMedia]',
  templateUrl: './property-media.component.html',
  styleUrls: ['./property-media.component.css'],
  standalone: true,
  imports: [ButtonComponent, PanelComponent, SkeletonLoaderComponent, SpinnerLoaderComponent],
})
export class PropertyMediaComponent implements OnChanges {
  readonly ICONS = { Images, Plus, Trash2 };
  private tempRowID: number = -1;

  images: UIState<MediaFile>[] = [];

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  @Input({ alias: 'appPropertyMedia' }) imageList?: MediaFile[];
  @Input({ required: true }) propertyId!: number;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    private fileStatusPollingService: FileStatusPollingService,
  ) {}

  // #region Lifecycle hooks
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['imageList']) {
      const value: MediaFile[] = changes['imageList'].currentValue ?? [];

      const tempFiles = this.images.filter((img) => img.isNew);

      const newFiles = value.map((img) => ({ data: img, isNew: false })) ?? [];
      this.images = [...newFiles, ...tempFiles];
    }
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

      const newFile: UIState<MediaFile> = {
        data: { id: this.tempRowID--, name: file.name, processingStatus: 'uploading' },
        isNew: true,
      };

      this.images.push(newFile);

      this.propertyService.uploadMediaFile(this.propertyId, file).subscribe({
        next: (image) => {
          newFile.data = image;
          this.fileStatusPollingService.getFileStatusObservable(image.id).subscribe((data) => {
            if (data.processingStatus === 'deleted' || data.processingStatus === 'failed') {
              this.images = this.images.filter((r) => r.data.id !== data.id);
            } else {
              newFile.data = { ...newFile.data, ...data };
            }
          });
        },
        error: (err) => console.log(err),
      });
    }
  }

  onRemoveClick(data: MediaFile) {
    // Disable action
    const existingRowId = this.images.findIndex((r) => r.data.id === data.id);
    if (existingRowId !== -1) {
      this.images[existingRowId].isActionDisabled = true;
    }

    this.propertyService.deleteMediaFile(this.propertyId, data.id).subscribe({
      next: (deletedUnit) => {
        this.images = this.images.filter((r) => r.data.id !== deletedUnit.id);
      },
      error: (err) => console.error(err),
    });
  }
  // #endregion
}
