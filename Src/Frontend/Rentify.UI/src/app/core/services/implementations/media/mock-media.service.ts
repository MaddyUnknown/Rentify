import { Observable } from 'rxjs';
import { data } from '../mock/data';
import { MediaService } from '../../abstractions/media.service';
import { MediaFile } from '../../../models/media-file/media-file.model';
import { Injectable } from '@angular/core';

@Injectable()
export class MediaMockService implements MediaService {
  getMediaFileStatus(mediaFileIds: number[]): Observable<MediaFile[]> {
    return new Observable<MediaFile[]>((observer) => {
      setTimeout(() => {
        const files = data.files
          .filter((img) => mediaFileIds.includes(img.id))
          .map(
            ({ id, name, contentType, processingStatus, thumbnailType, thumbnailProcessingStatus, markedAsCover }) => {
              if (processingStatus === 'processed' || processingStatus === 'deleted' || processingStatus === 'failed') {
                return {
                  id,
                  name,
                  contentType,
                  processingStatus,
                  markedAsCover: markedAsCover ?? false,
                  thumbnail:
                    thumbnailType && thumbnailProcessingStatus
                      ? {
                          contentType: thumbnailType,
                          processingStatus: thumbnailProcessingStatus,
                        }
                      : undefined,
                };
              } else {
                return {
                  id,
                  processingStatus,
                };
              }
            },
          );

        observer.next(files);
        observer.complete();
      });
    });
  }
}
