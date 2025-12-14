import { ImageFile } from './file.model';
import { Location } from './location.model';
import { PropertyUnit } from './unit.model';

export interface Property {
  id?: number;
  generalDetails?: PropertyDetails;
  imageMetadataList?: ImageFile[];
  units?: PropertyUnit[];
  location?: Location;
}

export interface PropertyDetails {
  name: string;
  streetName: string;
  city: string;
  state: string;
  zipCode: string;
  description: string;
}

export interface UpdatePropertyDetails {
  id: number;
  name: string;
  streetName: string;
  city: string;
  state: string;
  zipCode: string;
  description: string;
}

export interface PropertySummary {
  propertyId: number;
  name: string;
  description?: string;
  address: string;
  numberOfUnits: number;
  numberOfUtility: number;
}
