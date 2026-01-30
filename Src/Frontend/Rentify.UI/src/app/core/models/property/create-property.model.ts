import { Location } from '../location/location.model';
import { CreateMediaLink } from '../media-file/create-media-link.model';
import { CreateUnit } from '../unit/create-unit.model';
import { CreatePropertyDetails } from './create-property-details.model';

export interface CreateProperty {
  details: CreatePropertyDetails;
  media: CreateMediaLink[];
  units: CreateUnit[];
  location?: Location;
}
