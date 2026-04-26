import { HttpClient, HttpContext } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
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
import { unwrapReponse } from '../../../utils/unwrap-response.util';
import { MediaFile } from '../../../models/media-file/media-file.model';
import { CreateTenant } from '../../../models/tenant/create-tenant.model';
import { MediaFileVariantType } from '../../../models/media-file/media-file-variant-type.model';
import { AUTH_HEADER, SUBSCRIPTION_HEADER } from '../../tokens/http-context.token';

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
      >(this.environmentConfigService.apiBaseURL + `tenants?page=${page}&pageSize=${pageSize}&asOfDate=${asOfDate.toISOString()}`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  createTenant(tenant: CreateTenant): Observable<Tenant> {
    return this.httpClient
      .post<
        ResponseWrapper<Tenant>
      >(this.environmentConfigService.apiBaseURL + `tenants`, tenant, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  getTenantAggregateById(tenantId: number): Observable<Tenant> {
    return this.httpClient
      .get<
        ResponseWrapper<Tenant>
      >(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}/aggregate`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  updateTenantDetails(tenantId: number, details: UpdateTenantDetails): Observable<TenantDetails> {
    return this.httpClient
      .put<
        ResponseWrapper<TenantDetails>
      >(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}`, details, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  deleteTenant(tenantId: number): Observable<TenantDetails> {
    return this.httpClient
      .delete<
        ResponseWrapper<TenantDetails>
      >(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  updateTenantEmergencyContact(
    tenantId: number,
    tenantEmergencyContactId: number,
    contact: UpdateTenantEmergencyContact,
  ): Observable<TenantEmergencyContact> {
    return this.httpClient
      .put<
        ResponseWrapper<TenantEmergencyContact>
      >(this.environmentConfigService.apiBaseURL + `tenants/${tenantId}/emergency-contact/${tenantEmergencyContactId}`, contact, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  updateTenantProfilePic(file: File, tenantId?: number): Observable<MediaFile> {
    const formData = new FormData();
    formData.append('file', file);

    const requestUrl = tenantId ? `tenants/${tenantId}/profile-pic` : 'tenants/profile-pic';

    return this.httpClient
      .post<
        ResponseWrapper<MediaFile>
      >(this.environmentConfigService.apiBaseURL + requestUrl, formData, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  uploadTenantDocument(file: File, tenantId?: number): Observable<MediaFile> {
    const formData = new FormData();
    formData.append('file', file);

    const requestUrl = tenantId ? `tenants/${tenantId}/documents` : 'tenants/documents';

    return this.httpClient
      .post<
        ResponseWrapper<MediaFile>
      >(this.environmentConfigService.apiBaseURL + requestUrl, formData, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  deleteTenantMedia(mediaFile: number): Observable<MediaFile> {
    return this.httpClient
      .delete<
        ResponseWrapper<MediaFile>
      >(this.environmentConfigService.apiBaseURL + `tenants/media/${mediaFile}`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  getTenantMediaUrl(
    mediaId: number,
    variant: MediaFileVariantType | undefined,
  ): Observable<{ url: string; destroyFun: () => void }> {
    return this.httpClient
      .get(
        this.environmentConfigService.apiBaseURL +
          `tenants/media/${mediaId}` +
          (variant === undefined ? '' : `?variantType=${variant}`),
        { responseType: 'blob', context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) },
      )
      .pipe(
        map((blob) => {
          const url = URL.createObjectURL(blob);
          return { url, destroyFun: () => URL.revokeObjectURL(url) };
        }),
      );
  }
}
