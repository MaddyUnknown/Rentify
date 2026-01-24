import { MediaFileStatus } from '../../../models/media-file/media-file-status.model';
import { MediaFileVariantStatus } from '../../../models/media-file/media-file-variant-status.model';
import { UnitStatus } from '../../../models/unit/unit-status.model';

export type MockProperty = {
  id: number;
  name: string;
  streetName: string;
  city: string;
  state: string;
  zipCode: string;
  description: string;
  location?: { latitude: number; longitude: number };
};

export type MockUnit = {
  id: number;
  name: string;
  type: string;
  size: number;
  status: UnitStatus;
  propertyId: number;
};

export type MockFiles = {
  id: number;
  name: string;
  contentType: string;
  processingStatus: MediaFileStatus;
  markedAsCover?: boolean;
  thumbnailType?: string;
  thumbnailProcessingStatus?: MediaFileVariantStatus;
  propertyId: number;
};
