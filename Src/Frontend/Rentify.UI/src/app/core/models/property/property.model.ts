import { Location } from '../location/location.model';
import { MediaFile } from '../media-file/media-file.model';
import { Unit } from '../unit/unit.model';
import { GetPropertyDetails } from './get-property-details.model';

export interface Property {
  id: number;
  generalDetails: GetPropertyDetails;
  mediaFiles: MediaFile[];
  units: Unit[];
  location?: Location;
}
