import { Observable } from 'rxjs';
import { Location } from '../models/location.model';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class GeoLocationService {
  public getCurrentLocation(): Observable<Location> {
    return new Observable((observer) => {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const { latitude, longitude } = position.coords;
          observer.next({ latitude, longitude });
          observer.complete();
        },
        () => {
          observer.error(new Error('Could not get current location'));
        },
      );
    });
  }
}
