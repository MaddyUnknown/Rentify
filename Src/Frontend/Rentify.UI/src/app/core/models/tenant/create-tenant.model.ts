import { CreateMediaLink } from '../media-file/create-media-link.model';
import { CreateTenantDetails } from './create-tenant-details.model';
import { CreateTenantEmergencyContact } from './create-tenant-emergency-contact.model';

export interface CreateTenant {
  profilePicId: number | undefined;
  details: CreateTenantDetails;
  emergencyContact: CreateTenantEmergencyContact;
  documents: CreateMediaLink[];
}
