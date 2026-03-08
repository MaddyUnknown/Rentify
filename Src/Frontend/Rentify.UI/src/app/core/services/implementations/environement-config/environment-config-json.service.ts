import { Injectable } from '@angular/core';
import { environement } from '../../../../environments/environment';

@Injectable()
export class EnvironmentConfigJsonService {
  get defaultPropertyLatLon(): { latitude: number; longitude: number } {
    return {
      latitude: environement.defaultPropertyLatLon.latitude ?? 0,
      longitude: environement.defaultPropertyLatLon.longitude ?? 0,
    };
  }

  get apiBaseURL(): string {
    return environement.apiBaseURL;
  }

  get thumbnailImagePath(): { propertyMediaProcessing: string; propertyNotFound: string; tenantNotFound: string } {
    return environement.thumbnailImagePath;
  }
}
