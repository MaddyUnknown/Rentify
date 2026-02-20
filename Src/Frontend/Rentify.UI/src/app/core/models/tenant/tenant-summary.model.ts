export interface TenantSummary {
  id: number;
  name: string;
  email: string;
  phone: string;
  status: 'active' | 'inactive';
}
