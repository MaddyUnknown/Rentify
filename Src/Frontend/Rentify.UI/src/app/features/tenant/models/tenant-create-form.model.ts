import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { TenantDetailsForm } from './tenant-details-form.model';
import { TenantEmergencyContactForm } from './tenant-emergency-contact-form.model';
import { TenantMediaFileForm } from './tenant-media-file-form.model';

export type TenantCreateForm = {
  profilePicId: FormControl<number | undefined>;
  details: FormGroup<TenantDetailsForm>;
  emergencyContact: FormGroup<TenantEmergencyContactForm>;
  documents: FormArray<FormGroup<TenantMediaFileForm>>;
};
