import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Property } from '../models/property.model';

@Injectable({
  providedIn: 'root',
})
export class PropertyService {
  // private apiUrl = 'https://localhost:7000/api/properties';

  constructor(private http: HttpClient) {}

  // getAllProperties(): Observable<Property[]> {
  //   return this.http.get<Property[]>(this.apiUrl);
  // }

  getPropertyDetailsById(propertyId: number): Observable<Property> {
    return new Observable<Property>((observer) => {
      observer.next({
        id: propertyId,
        generalDetails: {
          name: `Thakurpukur Nivas ${propertyId}`,
          streetName: 'Bakrahat Road',
          city: 'Kolkata',
          state: 'West Bengal',
          zipCode: '700023',
          description:
            '3 Floors property with 6 rentable units and multiple utility. Just 2 km away from Joka Metro status which available connectivity.',
        },
        imageMetadataList: [
          {
            fileName: 'photo1.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1560185127-6ed189bf02f4',
          },
          {
            fileName: 'photo2.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1572120360610-d971b9d7767c',
          },
          {
            fileName: 'photo3.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1507089947368-19c1da9775ae',
          },
          {
            fileName: 'photo4.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1568605114967-8130f3a36994',
          },
          {
            fileName: 'photo5.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1600573472591-ee6c8e695237',
          },
          {
            fileName: 'photo6.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1570129477492-45c003edd2be',
          },
          {
            fileName: 'photo7.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1599423300746-b62533397364',
          },
          {
            fileName: 'photo8.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1599423300746-b62533397364',
          },
          {
            fileName: 'photo9.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1600585154084-4e5fe7c39103',
          },
          {
            fileName: 'photo10.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1600607687939-ce8a6c25118c',
          },
          {
            fileName: 'photo11.png',
            thumbnailUrl: '/img/thumbnails/thumbnail-image.png',
            imageUrl: 'https://images.unsplash.com/photo-1554995207-c18c203602cb',
          },
        ],
        location: {
          latitude: 22.451619072283112,
          longitude: 88.2892952762177,
        },
      });
      observer.complete();
    });
  }
}
