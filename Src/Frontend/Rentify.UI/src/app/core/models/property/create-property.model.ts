import { Location } from '../location/location.model';
import { CreateUnit } from '../unit/create-unit.model';
import { CreatePropertyDetails } from './create-property-details.model';

export interface CreateProperty {
  details: CreatePropertyDetails;
  mediaFileIds: number[];
  units: CreateUnit[];
  location?: Location;
  requestedCoverPicId?: number;
}
