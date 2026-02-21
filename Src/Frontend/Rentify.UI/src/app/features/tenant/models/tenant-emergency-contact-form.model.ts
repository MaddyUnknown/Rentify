import { FormControl } from '@angular/forms';

export type TenantEmergencyContactForm = {
  name: FormControl<string>;
  relationship: FormControl<string>;
  phoneNumber: FormControl<string>;
  email: FormControl<string>;
};
