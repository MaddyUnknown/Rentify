import { Component, ElementRef, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { Images, Plus, Trash2 } from 'lucide-angular';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { ImageFile } from '../../../../core/models/file.model';
import { SpinnerLoaderComponent } from '../../../../shared/components/spinner-loader/spinner-loader.component';
import { FileUploadService } from '../../../../core/services/file-upload.service';
import { FileStatusPollingService } from '../../../../shared/services/file-status-polling.service';
import { UIState } from '../../../../shared/models/ui-state.model';

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

  images: UIState<ImageFile>[] = [];

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  @Input({ alias: 'appPropertyMedia' }) imageList?: ImageFile[];
  @Input({ required: true }) propertyId!: number;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private fileUploadService: FileUploadService,
    private fileStatusPollingService: FileStatusPollingService,
  ) {}

  // #region Lifecycle hooks
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['imageList']) {
      const value: ImageFile[] = changes['imageList'].currentValue ?? [];

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

      const newFile: UIState<ImageFile> = {
        data: { id: this.tempRowID--, name: file.name, processingStatus: 'uploading' },
        isNew: true,
      };

      this.images.push(newFile);

      this.fileUploadService.uploadFile(this.propertyId, file).subscribe({
        next: (image) => {
          newFile.data = image;
          this.fileStatusPollingService.getFileStatusObservable(image.id).subscribe((data) => {
            newFile.data = data;
          });
        },
        error: (err) => console.log(err),
      });
    }
  }

  onRemoveClick(data: ImageFile) {
    // Disable action
    const existingRowId = this.images.findIndex((r) => r.data.id === data.id);
    if (existingRowId !== -1) {
      this.images[existingRowId].isActionDisabled = true;
    }

    this.fileUploadService.deleteFile(data.id).subscribe({
      next: (deletedUnit) => {
        this.images = this.images.filter((r) => r.data.id !== deletedUnit.id);
      },
      error: (err) => console.error(err),
    });
  }
  // #endregion
}
