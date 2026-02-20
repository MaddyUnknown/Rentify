import { MediaFile } from '../media-file/media-file.model';
import { GetTenantDetails } from './get-tenant-details.model';
import { TenantEmergencyContact } from './tenant-emergency-contact.model';

export interface Tenant {
  id: number;
  details: GetTenantDetails;
  emergencyContact?: TenantEmergencyContact;
  documents: MediaFile[];
}
