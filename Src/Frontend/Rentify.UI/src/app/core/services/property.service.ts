import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property, PropertyDetails, UpdatePropertyDetails } from '../models/property.model';
import { data } from './mock-data/data';

@Injectable({
  providedIn: 'root',
})
export class PropertyService {
  constructor(private http: HttpClient) {}

  getPropertyById(propertyId: number): Observable<Property> {
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

  updateProperty(property: UpdatePropertyDetails): Observable<PropertyDetails> {
    return new Observable<PropertyDetails>((observer) => {
      setTimeout(() => {
        const id = data.properties.findIndex((u) => u.id === property.id);

        if (id === -1) {
          observer.error('Property not found');
          return;
        }

        data.properties[id] = { ...data.properties[id], ...property };

        observer.next(data.properties[id]);
        observer.complete();
      }, data.apiLatency);
    });
  }

  deleteProperty(propertyId: number): Observable<PropertyDetails> {
    return new Observable<PropertyDetails>((observer) => {
      setTimeout(() => {
        const id = data.properties.findIndex((u) => u.id === propertyId);

        if (id === -1) {
          observer.error('Property not found');
          return;
        }

        const deletedProperty = data.properties[id];
        data.properties.splice(id, 1);

        observer.next(deletedProperty);
        observer.complete();
      }, data.apiLatency);
    });
  }
}
