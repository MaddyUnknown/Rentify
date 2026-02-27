import { TenantStatus } from './tenant-status.model';

export interface TenantSummary {
  id: number;
  name: string;
  email: string;
  phoneNumber: string;
  status: TenantStatus;
}
