import { MediaFileStatus } from './media-file-status.model';
import { MediaFileVariant } from './media-file-variant.model';

export interface MediaFile {
  id: number;
  name?: string;
  contentType?: string;
  uploadedDate?: Date;
  length?: number;
  markedAsCover?: boolean;
  processingStatus: MediaFileStatus;
  thumbnail?: MediaFileVariant;
  cover?: MediaFileVariant;
}
