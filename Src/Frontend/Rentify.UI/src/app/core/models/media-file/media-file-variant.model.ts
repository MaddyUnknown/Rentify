import { MediaFileVariantStatus } from './media-file-variant-status.model';

export interface MediaFileVariant {
  contentType: string;
  processingStatus: MediaFileVariantStatus;
}
