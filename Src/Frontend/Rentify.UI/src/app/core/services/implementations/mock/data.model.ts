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
  uploadedDate: Date;
  size: number;
  processingStatus: MediaFileStatus;
  markedAsCover?: boolean;
  markedAsProfilePic?: boolean;
  thumbnailType?: string;
  thumbnailProcessingStatus?: MediaFileVariantStatus;
  propertyId?: number;
  tenantId?: number;
};

export type MockTenant = {
  id: number;
  name: string;
  email: string;
  phoneNumber: string;
  dob: string;
  employment: string;
  streetName: string;
  city: string;
  state: string;
  zipCode: string;
  note: string;
  profilePicFileId?: number;
};

export type MockTenantEmergencyContact = {
  id: number;
  tenantId: number;
  name: string;
  relationship: string;
  phoneNumber: string;
  email: string;
};
