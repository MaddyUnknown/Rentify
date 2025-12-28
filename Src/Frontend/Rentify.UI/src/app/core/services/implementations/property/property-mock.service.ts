import { Observable } from 'rxjs';
import { data } from '../mock/data';
import { PropertyService } from '../../abstractions/property.service';
import { Property } from '../../../models/property/property.model';
import { UpdatePropertyDetails } from '../../../models/property/update-property-details.model';
import { PropertyDetails } from '../../../models/property/property-details.model';
import { UpdatePropertyLocation } from '../../../models/location/update-property-location.model';
import { PropertyLocation } from '../../../models/location/property-location.model';
import { Unit } from '../../../models/unit/unit.model';
import { CreateUnit } from '../../../models/unit/create-unit.model';
import { UpdateUnit } from '../../../models/unit/update-unit.model';
import { UnitStatus } from '../../../models/unit/unit-status.model';
import { MediaFile } from '../../../models/media-file/media-file.model';
import { MediaFileStatus } from '../../../models/media-file/media-file-status.model';
import { MockFiles } from '../mock/data.model';
import { Injectable } from '@angular/core';

@Injectable()
export class PropertyMockService implements PropertyService {
  getPropertyAggregateById(propertyId: number): Observable<Property> {
    return new Observable<Property>((observer) => {
      setTimeout(() => {
        const property = data.properties.find((p) => p.id === propertyId);

        const unitList = data.units
          .filter((unit) => unit.propertyId === propertyId)
          .map(({ id, name, type, size, status }) => ({ id, name, type, size, status }));

        const fileList = data.files
          .filter((img) => img.propertyId === propertyId)
          .map(({ id, name, contentType, processingStatus, thumbnailType, thumbnailProcessingStatus }) => ({
            id,
            name,
            contentType,
            processingStatus,
            thumbnail:
              thumbnailType && thumbnailProcessingStatus
                ? {
                    contentType: thumbnailType,
                    processingStatus: thumbnailProcessingStatus,
                  }
                : undefined,
          }));

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
          mediaFiles: fileList,
          location: property.location ? { ...property.location } : undefined,
          units: unitList,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  updateProperty(propertyId: number, updatePropertyDetails: UpdatePropertyDetails): Observable<PropertyDetails> {
    return new Observable<PropertyDetails>((observer) => {
      setTimeout(() => {
        const id = data.properties.findIndex((p) => p.id === propertyId);

        if (id === -1) {
          observer.error('Property not found');
          return;
        }

        data.properties[id] = { ...data.properties[id], ...updatePropertyDetails };

        observer.next({
          id: propertyId,
          name: data.properties[id].name,
          streetName: data.properties[id].streetName,
          city: data.properties[id].city,
          state: data.properties[id].state,
          zipCode: data.properties[id].zipCode,
          description: data.properties[id].description,
        });

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

        observer.next({
          id: propertyId,
          name: deletedProperty.name,
          streetName: deletedProperty.streetName,
          city: deletedProperty.city,
          state: deletedProperty.state,
          zipCode: deletedProperty.zipCode,
          description: deletedProperty.description,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  updatePropertyLocation(propertyId: number, location: UpdatePropertyLocation): Observable<PropertyLocation> {
    return new Observable<PropertyLocation>((observer) => {
      setTimeout(() => {
        const id = data.properties.findIndex((p) => p.id === propertyId);

        if (id === -1) {
          observer.error('Property not found');
          return;
        }

        data.properties[id].location =
          location.latitude === undefined || location.longitude === undefined
            ? undefined
            : { latitude: location.latitude, longitude: location.longitude };

        const result =
          data.properties[id].location === undefined ? { propertyId } : { propertyId, ...data.properties[id].location };

        observer.next(result);
        observer.complete();
      }, data.apiLatency);
    });
  }

  uploadMediaFile(propertyId: number, file: File): Observable<MediaFile> {
    const formData = new FormData();
    formData.append('file', file);

    return new Observable((observer) => {
      setTimeout(() => {
        const property = data.properties.find((p) => p.id === propertyId);

        if (!property) {
          observer.error('Property not found');
          return;
        }

        const nextId = data.files.reduce((maxId, img) => Math.max(maxId, img.id), 0) + 1;

        const image: MockFiles = {
          id: nextId,
          name: file.name,
          contentType: file.type,
          processingStatus: 'uploaded' as MediaFileStatus,
          propertyId: propertyId,
        };

        data.files.push(image);

        setTimeout(() => {
          image.processingStatus = 'processing';
          image.thumbnailType = file.type;
          image.thumbnailProcessingStatus = 'processing';

          console.log('processing');
        }, data.apiLatency * 2);

        setTimeout(() => {
          image.processingStatus = 'processed';
          image.thumbnailType = file.type;
          image.thumbnailProcessingStatus = 'processed';
        }, data.apiLatency * 6);

        observer.next({
          id: image.id,
          name: image.name,
          contentType: image.contentType,
          processingStatus: image.processingStatus,
          thumbnail:
            image.thumbnailType && image.thumbnailProcessingStatus
              ? {
                  contentType: image.thumbnailType,
                  processingStatus: image.thumbnailProcessingStatus,
                }
              : undefined,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  deleteMediaFile(propertyId: number, mediaId: number): Observable<MediaFile> {
    return new Observable((observer) => {
      setTimeout(() => {
        const id = data.files.findIndex((u) => u.id === mediaId);

        if (id === -1 || data.files[id].propertyId !== propertyId) observer.error('File not found');

        const deletedFile = data.files[id];
        data.files.splice(id, 1);

        observer.next({
          id: deletedFile.id,
          name: deletedFile.name,
          contentType: deletedFile.contentType,
          processingStatus: deletedFile.processingStatus,
          thumbnail:
            deletedFile.thumbnailType && deletedFile.thumbnailProcessingStatus
              ? {
                  contentType: deletedFile.thumbnailType,
                  processingStatus: deletedFile.thumbnailProcessingStatus,
                }
              : undefined,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  createUnit(propertyId: number, unit: CreateUnit): Observable<Unit> {
    return new Observable<Unit>((observer) => {
      setTimeout(() => {
        const property = data.properties.find((p) => p.id === propertyId);

        if (!property) {
          observer.error('Property not found');
          return;
        }

        const nextId = data.units.reduce((maxId, unit) => Math.max(maxId, unit.id), 0) + 1;

        const newData = {
          id: nextId,
          name: unit.name,
          type: unit.type,
          size: unit.size,
          status: 'vacant' as UnitStatus,
          propertyId,
        };

        data.units.push(newData);

        observer.next({
          id: newData.id,
          name: newData.name,
          type: newData.type,
          size: newData.size,
          status: newData.status,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  updateUnit(propertyId: number, unitId: number, unit: UpdateUnit): Observable<Unit> {
    return new Observable<Unit>((observer) => {
      setTimeout(() => {
        const id = data.units.findIndex((u) => u.id === unitId);

        if (id === -1 || data.units[id].propertyId !== propertyId) {
          observer.error('Unit not found');
          return;
        }

        data.units[id] = { ...data.units[id], ...unit };

        observer.next({
          id: data.units[id].id,
          name: data.units[id].name,
          type: data.units[id].type,
          size: data.units[id].size,
          status: data.units[id].status,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  deleteUnit(propertyId: number, unitId: number): Observable<Unit> {
    return new Observable<Unit>((observer) => {
      setTimeout(() => {
        const id = data.units.findIndex((u) => u.id === unitId);

        if (id === -1 || data.units[id].propertyId !== propertyId) {
          observer.error('Unit not found');
          return;
        }

        const deletedUnit = data.units[id];
        data.units.splice(id, 1);

        observer.next({
          id: deletedUnit.id,
          name: deletedUnit.name,
          type: deletedUnit.type,
          size: deletedUnit.size,
          status: deletedUnit.status,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }
}
