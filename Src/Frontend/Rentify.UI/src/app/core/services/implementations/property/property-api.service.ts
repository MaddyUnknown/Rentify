import { map, Observable, of } from 'rxjs';
import { PropertyService } from '../../abstractions/property.service';
import { Property } from '../../../models/property/property.model';
import { UpdatePropertyDetails } from '../../../models/property/update-property-details.model';
import { PropertyDetails } from '../../../models/property/property-details.model';
import { UpdatePropertyLocation } from '../../../models/location/update-property-location.model';
import { PropertyLocation } from '../../../models/location/property-location.model';
import { Unit } from '../../../models/unit/unit.model';
import { CreateUnit } from '../../../models/unit/create-unit.model';
import { UpdateUnit } from '../../../models/unit/update-unit.model';
import { MediaFile } from '../../../models/media-file/media-file.model';
import { HttpClient, HttpContext } from '@angular/common/http';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../tokens/environement-config.token';
import { EnvironmentConfigJsonService } from '../environement-config/environment-config-json.service';
import { Inject, Injectable } from '@angular/core';
import { ResponseWrapper } from '../../../models/response/response-wrapper.model';
import { unwrapReponse } from '../../../utils/unwrap-response.util';
import { MediaFileVariantType } from '../../../models/media-file/media-file-variant-type.model';
import { PaginatedList } from '../../../models/response/paginated-list.model';
import { PropertySummary } from '../../../models/property/property-summary.model';
import { CreateProperty } from '../../../models/property/create-property.model';
import { AUTH_HEADER, SUBSCRIPTION_HEADER } from '../../tokens/http-context.token';

@Injectable()
export class PropertyApiService implements PropertyService {
  constructor(
    private httpClient: HttpClient,
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private environmentConfigService: EnvironmentConfigJsonService,
  ) {}

  getPaginatedProperties(page: number, pageSize: number, asOfDate: Date): Observable<PaginatedList<PropertySummary>> {
    return this.httpClient
      .get<
        ResponseWrapper<PaginatedList<PropertySummary>>
      >(this.environmentConfigService.apiBaseURL + `properties?page=${page}&pageSize=${pageSize}&asOfDate=${asOfDate.toISOString()}`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  createProperty(property: CreateProperty): Observable<Property> {
    return this.httpClient
      .post<
        ResponseWrapper<Property>
      >(this.environmentConfigService.apiBaseURL + `properties`, property, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  getPropertyAggregateById(propertyId: number): Observable<Property> {
    return this.httpClient
      .get<
        ResponseWrapper<Property>
      >(this.environmentConfigService.apiBaseURL + `properties/${propertyId}/aggregate`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  updateProperty(propertyId: number, updatePropertyDetails: UpdatePropertyDetails): Observable<PropertyDetails> {
    return this.httpClient
      .put<
        ResponseWrapper<PropertyDetails>
      >(this.environmentConfigService.apiBaseURL + `properties/${propertyId}`, updatePropertyDetails, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  deleteProperty(propertyId: number): Observable<PropertyDetails> {
    return this.httpClient
      .delete<
        ResponseWrapper<PropertyDetails>
      >(this.environmentConfigService.apiBaseURL + `properties/${propertyId}`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  updatePropertyLocation(propertyId: number, location: UpdatePropertyLocation): Observable<PropertyLocation> {
    return this.httpClient
      .put<
        ResponseWrapper<PropertyLocation>
      >(this.environmentConfigService.apiBaseURL + `properties/${propertyId}/location`, location, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  uploadMediaFile(file: File, propertyId?: number): Observable<MediaFile> {
    const formData = new FormData();
    formData.append('file', file);

    const requestUrl = propertyId ? `properties/${propertyId}/media` : 'properties/media';

    return this.httpClient
      .post<
        ResponseWrapper<MediaFile>
      >(this.environmentConfigService.apiBaseURL + requestUrl, formData, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  deleteMediaFile(mediaId: number): Observable<MediaFile> {
    return this.httpClient
      .delete<
        ResponseWrapper<MediaFile>
      >(this.environmentConfigService.apiBaseURL + `properties/media/${mediaId}`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  markMediaFileAsCover(propertyId: number, mediaId: number): Observable<MediaFile> {
    return this.httpClient
      .put<
        ResponseWrapper<MediaFile>
      >(this.environmentConfigService.apiBaseURL + `properties/${propertyId}/cover-media`, { mediaFileId: mediaId }, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  createUnit(propertyId: number, unit: CreateUnit): Observable<Unit> {
    return this.httpClient
      .post<
        ResponseWrapper<Unit>
      >(this.environmentConfigService.apiBaseURL + `properties/${propertyId}/units`, unit, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  updateUnit(propertyId: number, unitId: number, unit: UpdateUnit): Observable<Unit> {
    return this.httpClient
      .put<
        ResponseWrapper<Unit>
      >(this.environmentConfigService.apiBaseURL + `properties/${propertyId}/units/${unitId}`, unit, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  deleteUnit(propertyId: number, unitId: number): Observable<Unit> {
    return this.httpClient
      .delete<
        ResponseWrapper<Unit>
      >(this.environmentConfigService.apiBaseURL + `properties/${propertyId}/units/${unitId}`, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }

  getPropertyMediaUrl(
    mediaId: number,
    variant: MediaFileVariantType | undefined,
  ): Observable<{ url: string; destroyFun: () => void }> {
    return this.httpClient
      .get(
        this.environmentConfigService.apiBaseURL +
          `properties/media/${mediaId}` +
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
