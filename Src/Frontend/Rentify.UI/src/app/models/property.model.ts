import { Location } from './location.model';

export interface PropertyDetails {
  name: string;
  streetName: string;
  city: string;
  state: string;
  zipCode: string;
  description?: string;
}

export interface PropertyImageMetadata {
  fileName: string;
  thumbnailUrl: string;
  imageUrl: string;
}

export interface Property {
  id: number;
  generalDetails: PropertyDetails;
  imageMetadataList: PropertyImageMetadata[];
  location?: Location;
}

export interface PropertySummary {
  propertyId: number;
  name: string;
  description?: string;
  address: string;
  numberOfUnits: number;
  numberOfUtility: number;
}
