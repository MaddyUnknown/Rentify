import { Injectable } from '@angular/core';
import { ImageFile } from '../../core/models/file.model';
import { concatMap, interval, Observable, Subject, Subscription } from 'rxjs';
import { FileUploadService } from '../../core/services/file-upload.service';

//TO-DO: Deal with situation where the file is not returned by te servier. Usualy mean that the data is not present.
@Injectable({
  providedIn: 'root',
})
export class FileStatusPollingService {
  private fileStatusObservables: Map<number, Subject<ImageFile>> = new Map();
  private pollerSub?: Subscription;
  private poller$ = interval(3000).pipe(
    concatMap(() => {
      return this.checkFileStatus();
    }),
  );

  constructor(private fileUploadService: FileUploadService) {}

  getFileStatusObservable(fileId: number): Observable<ImageFile> {
    if (!this.fileStatusObservables.has(fileId)) {
      this.fileStatusObservables.set(fileId, new Subject<ImageFile>());
    }

    this.startPolling();

    return this.fileStatusObservables.get(fileId)!.asObservable();
  }

  private checkFileStatus(): Observable<void> {
    return new Observable((observable) => {
      const fileIdList = [...this.fileStatusObservables.keys()];

      this.fileUploadService.getFilesStatus(fileIdList).subscribe((files) => {
        files.forEach((file) => {
          if (file.processingStatus === 'processed') {
            this.fileStatusObservables.get(file.id)?.next(file);
            this.fileStatusObservables.get(file.id)?.complete();
            this.fileStatusObservables.delete(file.id);
          }
        });

        if (this.fileStatusObservables.size == 0) {
          this.stopPolling();
        }

        observable.next();
        observable.complete();
      });
    });
  }

  private startPolling() {
    if (this.pollerSub) return;

    this.pollerSub = this.poller$.subscribe();
  }

  private stopPolling() {
    if (!this.pollerSub) return;

    this.pollerSub.unsubscribe();
    this.pollerSub = undefined;
  }
}
