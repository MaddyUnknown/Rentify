import { Inject, Injectable } from '@angular/core';
import { exhaustMap, finalize, interval, Observable, Subject, Subscription } from 'rxjs';
import { MediaService } from '../../core/services/abstractions/media.service';
import { MEDIA_SERVICE_TOKEN } from '../../core/services/tokens/media.token';
import { MediaFile } from '../../core/models/media-file/media-file.model';
import { ApiError } from '../../core/exceptions/api-error';

type MediaObservableValueType = { count: number; subject: Subject<MediaFile> };

@Injectable({
  providedIn: 'root',
})
export class MediaStatusPollingService {
  private mediaObservableMap: Map<number, MediaObservableValueType>;
  private poller$: Observable<void>;
  private pollerSub?: Subscription;

  constructor(@Inject(MEDIA_SERVICE_TOKEN) private fileUploadService: MediaService) {
    this.mediaObservableMap = new Map<number, MediaObservableValueType>();

    // Exhaust Map to avoid new request unless the current one is completed
    this.poller$ = interval(3000).pipe(
      exhaustMap(() => {
        return this.checkFileStatus();
      }),
    );
  }

  getStatusObservable(fileId: number): Observable<MediaFile> {
    let observableVal = this.mediaObservableMap.get(fileId);
    if (!observableVal) {
      observableVal = { count: 0, subject: new Subject<MediaFile>() };
      this.mediaObservableMap.set(fileId, observableVal);
    }

    observableVal.count++;
    this.startPolling();

    return this.mediaObservableMap
      .get(fileId)!
      .subject.asObservable()
      .pipe(
        finalize(() => {
          observableVal.count--;
          if (observableVal.count == 0) {
            this.mediaObservableMap.delete(fileId);
          }

          if (this.mediaObservableMap.size == 0) {
            this.stopPolling();
          }
        }),
      );
  }

  private checkFileStatus(): Observable<void> {
    return new Observable((observable) => {
      const fileIdList = [...this.mediaObservableMap.keys()];

      this.fileUploadService.getMediaFileStatus(fileIdList).subscribe({
        next: (files) => {
          //Not found in the result - mark as deleted/error processing
          fileIdList.forEach((fileId) => {
            if (!files.find((file) => file.id == fileId)) {
              this.mediaObservableMap.get(fileId)?.subject?.error(new Error('File status not found'));
            }
          });

          // Found in result - update
          files.forEach((file) => {
            this.mediaObservableMap.get(file.id)?.subject?.next(file);
            if (
              file.processingStatus === 'processed' ||
              file.processingStatus === 'failed' ||
              file.processingStatus === 'deleted'
            ) {
              this.mediaObservableMap.get(file.id)?.subject?.complete();
            }
          });

          observable.next();
          observable.complete();
        },
        error: (err) => {
          if (err instanceof ApiError) {
            console.log('API Error', err.Errors);
          } else {
            console.error(err);
          }
        },
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
