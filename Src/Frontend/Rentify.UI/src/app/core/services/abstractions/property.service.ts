import { Observable } from 'rxjs';
import { Property } from '../../models/property/property.model';
import { PropertyDetails } from '../../models/property/property-details.model';
import { UpdatePropertyDetails } from '../../models/property/update-property-details.model';
import { UpdatePropertyLocation } from '../../models/location/update-property-location.model';
import { PropertyLocation } from '../../models/location/property-location.model';
import { MediaFile } from '../../models/media-file/media-file.model';
import { Unit } from '../../models/unit/unit.model';
import { CreateUnit } from '../../models/unit/create-unit.model';
import { UpdateUnit } from '../../models/unit/update-unit.model';
import { MediaFileVariantType } from '../../models/media-file/media-file-variant-type.model';
import { PaginatedList } from '../../models/response/paginated-list.model';
import { PropertySummary } from '../../models/property/property-summary.model';
import { CreateProperty } from '../../models/property/create-property.model';

export interface PropertyService {
  getPaginatedProperties(page: number, pageSize: number, asOfDate: Date): Observable<PaginatedList<PropertySummary>>;

  createProperty(property: CreateProperty): Observable<Property>;
  getPropertyAggregateById(propertyId: number): Observable<Property>;
  updateProperty(propertyId: number, updatePropertyDetails: UpdatePropertyDetails): Observable<PropertyDetails>;
  deleteProperty(propertyId: number): Observable<PropertyDetails>;
  updatePropertyLocation(propertyId: number, location: UpdatePropertyLocation): Observable<PropertyLocation>;

  uploadMediaFile(file: File, propertyId?: number): Observable<MediaFile>;
  deleteMediaFile(mediaId: number): Observable<MediaFile>;
  markMediaFileAsCover(propertyId: number, mediaId: number): Observable<MediaFile>;
  generatePropertyMediaUrl(mediaId: number, variant: MediaFileVariantType): string;

  createUnit(propertyId: number, unit: CreateUnit): Observable<Unit>;
  updateUnit(propertyId: number, unitId: number, unit: UpdateUnit): Observable<Unit>;
  deleteUnit(propertyId: number, unitId: number): Observable<Unit>;
}
