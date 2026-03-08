import { Location } from '../location/location.model';
import { MediaFile } from '../media-file/media-file.model';
import { Unit } from '../unit/unit.model';
import { GetPropertyCoverPic } from './get-property-cover-pic.model';
import { GetPropertyDetails } from './get-property-details.model';

export interface Property {
  id: number;
  generalDetails: GetPropertyDetails;
  mediaFiles: MediaFile[];
  coverPicMetadata: GetPropertyCoverPic;
  units: Unit[];
  location?: Location;
}
