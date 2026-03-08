import { FormControl } from '@angular/forms';

export type TenantDetailsForm = {
  name: FormControl<string>;
  email: FormControl<string>;
  phoneNumber: FormControl<string>;
  dob: FormControl<string>;
  employment: FormControl<string>;
  streetName: FormControl<string>;
  city: FormControl<string>;
  state: FormControl<string>;
  zipCode: FormControl<string>;
  note: FormControl<string>;
};
