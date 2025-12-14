export interface ImageFile {
  id: number;
  name: string;
  thumbnailUrl?: string;
  imageUrl?: string;
  processingStatus: FileStatus;
}

export type FileStatus = 'uploading' | 'processing' | 'processed';
