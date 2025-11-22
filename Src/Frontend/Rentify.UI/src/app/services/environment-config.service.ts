import { Injectable } from '@angular/core';
import { environement } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class EnvironmentConfigService {
  get defaultPropertyLatLon(): { latitude: number; longitude: number } {
    return {
      latitude: environement.defaultPropertyLatLon.latitude ?? 0,
      longitude: environement.defaultPropertyLatLon.longitude ?? 0,
    };
  }
}
