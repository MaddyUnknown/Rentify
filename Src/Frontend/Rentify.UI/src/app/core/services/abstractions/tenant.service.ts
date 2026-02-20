import { Observable } from 'rxjs';
import { PaginatedList } from '../../models/response/paginated-list.model';
import { TenantSummary } from '../../models/tenant/tenant-summary.model';
import { Tenant } from '../../models/tenant/tenant.model';
import { UpdateTenantDetails } from '../../models/tenant/update-tenant-details.model';
import { TenantDetails } from '../../models/tenant/tenant-details.model';
import { TenantEmergencyContact } from '../../models/tenant/tenant-emergency-contact.model';
import { UpdateTenantEmergencyContact } from '../../models/tenant/update-tenant-emergency-contact.model';
import { MediaFile } from '../../models/media-file/media-file.model';

export interface TenantService {
  getPaginatedTenants(page: number, pageSize: number, asOfDate: Date): Observable<PaginatedList<TenantSummary>>;

  getTenantAggregateById(tenantId: number): Observable<Tenant>;
  updateTenantDetails(tenantId: number, details: UpdateTenantDetails): Observable<TenantDetails>;
  deleteTenant(tenantId: number): Observable<TenantDetails>;

  updateTenantEmergencyContact(
    tenantId: number,
    contact: UpdateTenantEmergencyContact,
  ): Observable<TenantEmergencyContact>;
  deleteTenantEmergencyContact(tenantId: number): Observable<TenantEmergencyContact>;

  uploadTenantDocument(file: File, tenantId?: number): Observable<MediaFile>;
  deleteTenantDocument(mediaId: number): Observable<MediaFile>;
}
