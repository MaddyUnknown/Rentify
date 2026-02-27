import { FormControl } from '@angular/forms';

export type TenantEmergencyContactForm = {
  id: FormControl<number>;
  name: FormControl<string>;
  relationship: FormControl<string>;
  phoneNumber: FormControl<string>;
  email: FormControl<string>;
};
