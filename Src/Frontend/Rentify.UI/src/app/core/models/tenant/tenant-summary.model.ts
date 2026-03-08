import { MediaFile } from '../media-file/media-file.model';
import { TenantStatus } from './tenant-status.model';

export interface TenantSummary {
  id: number;
  name: string;
  email: string;
  phoneNumber: string;
  profilePic?: MediaFile;
  status: TenantStatus;
}
