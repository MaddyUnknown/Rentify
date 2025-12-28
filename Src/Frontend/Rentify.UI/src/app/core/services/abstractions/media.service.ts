import { Observable } from 'rxjs';
import { MediaFile } from '../../models/media-file/media-file.model';

export interface MediaService {
  getMediaFileStatus(mediaFileIds: number[]): Observable<MediaFile[]>;
}
