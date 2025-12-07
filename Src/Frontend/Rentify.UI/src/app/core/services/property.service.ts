import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property } from '../models/property.model';
import { data } from './mock-data/data';

@Injectable({
  providedIn: 'root',
})
export class PropertyService {
  constructor(private http: HttpClient) {}

  getPropertyDetailsById(propertyId: number): Observable<Property> {
    return new Observable<Property>((observer) => {
      setTimeout(() => {
        const property = data.properties.find((p) => p.id === propertyId);

        const unitList = data.units
          .filter((unit) => unit.propertyId === propertyId)
          .map(({ id, name, type, size, status }) => ({ id, name, type, size, status }));

        const imgList = data.images
          .filter((img) => img.propertyId === propertyId)
          .map(({ fileName, imageUrl, thumbnailUrl }) => ({ fileName, imageUrl, thumbnailUrl }));

        if (!property) {
          observer.error('Property not found');
          return;
        }

        observer.next({
          id: propertyId,
          generalDetails: {
            name: property.name,
            streetName: property.streetName,
            city: property.city,
            state: property.state,
            zipCode: property.zipCode,
            description: property.description,
          },
          imageMetadataList: imgList,
          location: property.location ? { ...property.location } : undefined,
          units: unitList,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }
}
