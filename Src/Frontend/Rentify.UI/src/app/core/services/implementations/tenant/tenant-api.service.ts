import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TenantService } from '../../abstractions/tenant.service';
import { PaginatedList } from '../../../models/response/paginated-list.model';
import { TenantSummary } from '../../../models/tenant/tenant-summary.model';
import { Tenant } from '../../../models/tenant/tenant.model';
import { TenantDetails } from '../../../models/tenant/tenant-details.model';
import { UpdateTenantDetails } from '../../../models/tenant/update-tenant-details.model';
import { UpdateTenantEmergencyContact } from '../../../models/tenant/update-tenant-emergency-contact.model';
import { TenantEmergencyContact } from '../../../models/tenant/tenant-emergency-contact.model';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../tokens/environement-config.token';
import { EnvironmentConfigJsonService } from '../environement-config/environment-config-json.service';
import { ResponseWrapper } from '../../../models/response/response-wrapper.model';
import { processResponse } from '../../../utils/process-response.util';
import { MediaFile } from '../../../models/media-file/media-file.model';
import { CreateTenant } from '../../../models/tenant/create-tenant.model';

@Injectable()
export class TenantApiService implements TenantService {
  constructor(
    private httpClient: HttpClient,
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private environmentConfigService: EnvironmentConfigJsonService,
  ) {}

  getPaginatedTenants(page: number, pageSize: number, asOfDate: Date): Observable<PaginatedList<TenantSummary>> {
    return this.httpClient
      .get<
        ResponseWrapper<PaginatedList<TenantSummary>>
      >(this.environmentConfigService.apiBaseURL + `tenants?page=${page}&pageSize=${pageSize}&asOfDate=${asOfDate.toISOString()}`)
      .pipe(processResponse());
  }

  createTenant(tenant: CreateTenant): Observable<Tenant> {
    return this.httpClient
      .post<ResponseWrapper<Tenant>>(this.environmentConfigService.apiBaseURL + `tenants`, tenant)
      .pipe(processResponse());
  }

  getTenantAggregateById(tenantId: number): Observable<Tenant> {
    return this.httpClient
      .get<ResponseWrapper<Tenant>>(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}/aggregate`)
      .pipe(processResponse());
  }

  updateTenantDetails(tenantId: number, details: UpdateTenantDetails): Observable<TenantDetails> {
    return this.httpClient
      .put<ResponseWrapper<TenantDetails>>(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}`, details)
      .pipe(processResponse());
  }

  deleteTenant(tenantId: number): Observable<TenantDetails> {
    return this.httpClient
      .delete<ResponseWrapper<TenantDetails>>(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}`)
      .pipe(processResponse());
  }

  updateTenantEmergencyContact(
    tenantId: number,
    contact: UpdateTenantEmergencyContact,
  ): Observable<TenantEmergencyContact> {
    return this.httpClient
      .put<
        ResponseWrapper<TenantEmergencyContact>
      >(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}/emergency-contact`, contact)
      .pipe(processResponse());
  }

  deleteTenantEmergencyContact(tenantId: number): Observable<TenantEmergencyContact> {
    return this.httpClient
      .delete<
        ResponseWrapper<TenantEmergencyContact>
      >(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}/emergency-contact`)
      .pipe(processResponse());
  }

  uploadTenantDocument(file: File, tenantId?: number): Observable<MediaFile> {
    const formData = new FormData();
    formData.append('file', file);

    const requestUrl = tenantId ? `tenants/${tenantId}/documents` : 'tenants/documents';

    return this.httpClient
      .post<ResponseWrapper<MediaFile>>(this.environmentConfigService.apiBaseURL + requestUrl, document)
      .pipe(processResponse());
  }

  deleteTenantDocument(mediaFile: number): Observable<MediaFile> {
    return this.httpClient
      .delete<ResponseWrapper<MediaFile>>(this.environmentConfigService.apiBaseURL + `tenants/documents/${mediaFile}`)
      .pipe(processResponse());
  }
}
