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

export interface PropertyService {
  getPropertyAggregateById(propertyId: number): Observable<Property>;
  updateProperty(propertyId: number, updatePropertyDetails: UpdatePropertyDetails): Observable<PropertyDetails>;
  deleteProperty(propertyId: number): Observable<PropertyDetails>;
  updatePropertyLocation(propertyId: number, location: UpdatePropertyLocation): Observable<PropertyLocation>;

  uploadMediaFile(propertyId: number, file: File): Observable<MediaFile>;
  deleteMediaFile(propertyId: number, mediaId: number): Observable<MediaFile>;

  createUnit(propertyId: number, unit: CreateUnit): Observable<Unit>;
  updateUnit(propertyId: number, unitId: number, unit: UpdateUnit): Observable<Unit>;
  deleteUnit(propertyId: number, unitId: number): Observable<Unit>;

  generatePropertyMediaUrl(propertyId: number, mediaId: number, variant: MediaFileVariantType): string;
}
