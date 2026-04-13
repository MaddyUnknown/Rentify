import { MediaFile } from '../../../core/models/media-file/media-file.model';
import { LocalDestroyRef } from '../../../shared/lifecycles/local-destroy-ref';

export type NewMediaFileRow = {
  kind: 'new';
  data: MediaFile;
};

export type MediaFileRow = {
  kind: 'existing';
  data: MediaFile;
  disableActions: boolean;
  destoryPollingRef: LocalDestroyRef;
  destroyImageRef?: LocalDestroyRef;
  imageUrl?: string;
};
