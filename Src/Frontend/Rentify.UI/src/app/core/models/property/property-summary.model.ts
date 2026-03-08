import { MediaFile } from '../media-file/media-file.model';

export interface PropertySummary {
  id: number;
  name: string;
  address: string;
  numberOfUnits: number;
  numberOfVacantUnits: number;
  coverPic?: MediaFile;
}
