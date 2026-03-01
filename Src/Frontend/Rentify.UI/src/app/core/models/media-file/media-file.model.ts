import { MediaFileStatus } from './media-file-status.model';
import { MediaFileVariantType } from './media-file-variant-type.model';
import { MediaFileVariant } from './media-file-variant.model';

export interface MediaFile {
  id: number;
  name?: string;
  contentType?: string;
  uploadedDate?: Date;
  length?: number;
  markedAsCover?: boolean;
  processingStatus: MediaFileStatus;
  variants?: Partial<Record<MediaFileVariantType, MediaFileVariant>>;
}
