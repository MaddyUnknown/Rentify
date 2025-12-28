import { Inject, Injectable } from '@angular/core';
import { concatMap, interval, Observable, Subject, Subscription } from 'rxjs';
import { MediaService } from '../../core/services/abstractions/media.service';
import { MEDIA_SERVICE_TOKEN } from '../../core/services/tokens/media';
import { MediaFile } from '../../core/models/media-file/media-file.model';

//TO-DO: Deal with situation where the file is not returned by te servier. Usualy mean that the data is not present.
@Injectable({
  providedIn: 'root',
})
export class FileStatusPollingService {
  private fileStatusObservables: Map<number, Subject<MediaFile>> = new Map();
  private pollerSub?: Subscription;
  private poller$ = interval(3000).pipe(
    concatMap(() => {
      return this.checkFileStatus();
    }),
  );

  constructor(@Inject(MEDIA_SERVICE_TOKEN) private fileUploadService: MediaService) {}

  getFileStatusObservable(fileId: number): Observable<MediaFile> {
    if (!this.fileStatusObservables.has(fileId)) {
      this.fileStatusObservables.set(fileId, new Subject<MediaFile>());
    }

    this.startPolling();

    return this.fileStatusObservables.get(fileId)!.asObservable();
  }

  private checkFileStatus(): Observable<void> {
    return new Observable((observable) => {
      const fileIdList = [...this.fileStatusObservables.keys()];

      this.fileUploadService.getMediaFileStatus(fileIdList).subscribe((files) => {
        files.forEach((file) => {
          this.fileStatusObservables.get(file.id)?.next(file);

          if (
            file.processingStatus === 'processed' ||
            file.processingStatus === 'failed' ||
            file.processingStatus === 'deleted'
          ) {
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
