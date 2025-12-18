import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { data } from './mock-data/data';
import { FileStatus, ImageFile } from '../models/file.model';

@Injectable({
  providedIn: 'root',
})
export class FileUploadService {
  uploadFile(propertyId: number, file: File): Observable<ImageFile> {
    const formData = new FormData();
    formData.append('file', file);

    return new Observable((observer) => {
      setTimeout(() => {
        const property = data.properties.find((p) => p.id === propertyId);

        if (!property) {
          observer.error('Property not found');
          return;
        }

        const nextId = data.images.reduce((maxId, img) => Math.max(maxId, img.id), 0) + 1;

        const image = {
          id: nextId,
          name: file.name,
          thumbnailUrl: '',
          imageUrl: '',
          processingStatus: 'processing' as FileStatus,
          propertyId: propertyId,
        };

        data.images.push(image);

        setTimeout(() => {
          image.processingStatus = 'processed';
          image.thumbnailUrl = '/img/thumbnails/thumbnail-image.png';
          image.imageUrl = 'https://images.unsplash.com/photo-1560185127-6ed189bf02f4';
        }, data.apiLatency * 3);

        observer.next({ ...image });
        observer.complete();
      }, data.apiLatency);
    });
  }

  getFilesStatus(fileIdList: number[]): Observable<ImageFile[]> {
    return new Observable((observer) => {
      setTimeout(() => {
        const files = data.images
          .filter((img) => fileIdList.includes(img.id))
          .map(({ id, name, thumbnailUrl, imageUrl, processingStatus }) => ({
            id,
            name,
            thumbnailUrl,
            imageUrl,
            processingStatus,
          }));

        //TO-DO: Error if the id doesnt match.

        observer.next(files);
        observer.complete();
      });
    });
  }

  deleteFile(fileId: number): Observable<ImageFile> {
    return new Observable((observer) => {
      setTimeout(() => {
        const id = data.images.findIndex((u) => u.id === fileId);

        if (id === -1) observer.error('File not found');

        const deletedFile = data.images[id];
        data.images.splice(id, 1);

        observer.next(deletedFile);
        observer.complete();
      }, data.apiLatency);
    });
  }
}
