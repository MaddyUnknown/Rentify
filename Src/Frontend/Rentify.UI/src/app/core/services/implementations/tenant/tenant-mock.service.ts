import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TenantService } from '../../abstractions/tenant.service';
import { PaginatedList } from '../../../models/response/paginated-list.model';
import { TenantSummary } from '../../../models/tenant/tenant-summary.model';
import { Tenant } from '../../../models/tenant/tenant.model';
import { TenantDetails } from '../../../models/tenant/tenant-details.model';
import { TenantEmergencyContact } from '../../../models/tenant/tenant-emergency-contact.model';
import { UpdateTenantDetails } from '../../../models/tenant/update-tenant-details.model';
import { UpdateTenantEmergencyContact } from '../../../models/tenant/update-tenant-emergency-contact.model';
import { data } from '../mock/data';
import { MediaFile } from '../../../models/media-file/media-file.model';
import { MockFiles } from '../mock/data.model';
import { MediaFileStatus } from '../../../models/media-file/media-file-status.model';
import { TenantStatus } from '../../../models/tenant/tenant-status.model';
import { CreateTenant } from '../../../models/tenant/create-tenant.model';
import { MediaFileVariant } from '../../../models/media-file/media-file-variant.model';
import { MediaFileVariantType } from '../../../models/media-file/media-file-variant-type.model';

@Injectable()
export class TenantMockService implements TenantService {
  getPaginatedTenants(page: number, pageSize: number, _asOfDate: Date): Observable<PaginatedList<TenantSummary>> {
    return new Observable<PaginatedList<TenantSummary>>((observer) => {
      setTimeout(() => {
        const tenants = data.tenants
          .slice((page - 1) * pageSize, page * pageSize)
          .map(({ id, name, email, phoneNumber }) => ({
            id,
            name,
            email,
            phoneNumber: phoneNumber,
            status: 'active' as TenantStatus,
          }));

        observer.next({ totalItems: data.tenants.length, items: tenants, currentPage: page });
        observer.complete();
      }, data.apiLatency);
    });
  }

  createTenant(tenant: CreateTenant): Observable<Tenant> {
    return new Observable<Tenant>((observer) => {
      setTimeout(() => {
        const nextTenantId = data.tenants.reduce((maxId, item) => Math.max(maxId, item.id), 0) + 1;
        const nextTenantContactId =
          data.tenantEmergencyContacts.reduce((maxId, item) => Math.max(maxId, item.id), 0) + 1;

        data.tenants.push({
          id: nextTenantId,
          name: tenant.details.name,
          email: tenant.details.email,
          phoneNumber: tenant.details.phoneNumber,
          dob: tenant.details.dob,
          employment: tenant.details.employment,
          streetName: tenant.details.streetName,
          city: tenant.details.city,
          state: tenant.details.state,
          zipCode: tenant.details.zipCode,
          note: tenant.details.note,
          profilePicFileId: undefined,
        });

        const emergencyContact = {
          id: nextTenantContactId,
          tenantId: nextTenantId,
          name: tenant.emergencyContact.name,
          relationship: tenant.emergencyContact.relationship,
          phoneNumber: tenant.emergencyContact.phoneNumber,
          email: tenant.emergencyContact.email,
        };

        data.tenantEmergencyContacts.push(emergencyContact);

        const selectedDocumentIds = new Set(tenant.documents.map((document) => document.id));
        const documents = data.files
          .filter((file) => selectedDocumentIds.has(file.id))
          .map((file) => {
            file.tenantId = nextTenantId;

            return {
              id: file.id,
              name: file.name,
              contentType: file.contentType,
              uploadedDate: file.uploadedDate,
              size: file.size,
              processingStatus: file.processingStatus,
            };
          });

        const profilePic = data.files.find((file) => file.id === tenant.profilePicId);
        if (profilePic) {
          profilePic.tenantId = nextTenantId;
          profilePic.markedAsProfilePic = true;
        }

        observer.next({
          id: nextTenantId,
          details: { ...tenant.details },
          emergencyContact: { ...emergencyContact },
          mediaFiles: documents,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  getTenantAggregateById(tenantId: number): Observable<Tenant> {
    return new Observable<Tenant>((observer) => {
      setTimeout(() => {
        const tenant = data.tenants.find((t) => t.id === tenantId);

        if (!tenant) {
          observer.error('Tenant not found');
          return;
        }

        const emergencyContact = data.tenantEmergencyContacts.find((c) => c.tenantId === tenantId);
        const documents = data.files.filter((doc) => doc.tenantId === tenantId);

        observer.next({
          id: tenantId,
          details: {
            name: tenant.name,
            email: tenant.email,
            phoneNumber: tenant.phoneNumber,
            dob: tenant.dob,
            employment: tenant.employment,
            streetName: tenant.streetName,
            city: tenant.city,
            state: tenant.state,
            zipCode: tenant.zipCode,
            note: tenant.note,
          },
          emergencyContact: emergencyContact
            ? {
                id: emergencyContact.id,
                name: emergencyContact.name,
                relationship: emergencyContact.relationship,
                phoneNumber: emergencyContact.phoneNumber,
                email: emergencyContact.email,
              }
            : undefined,
          mediaFiles: documents.map(({ id, name, contentType, uploadedDate, size, processingStatus }) => ({
            id,
            name,
            contentType,
            uploadedDate,
            size,
            processingStatus,
          })),
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  updateTenantDetails(tenantId: number, details: UpdateTenantDetails): Observable<TenantDetails> {
    return new Observable<TenantDetails>((observer) => {
      setTimeout(() => {
        const id = data.tenants.findIndex((t) => t.id === tenantId);

        if (id === -1) {
          observer.error('Tenant not found');
          return;
        }

        data.tenants[id] = {
          ...data.tenants[id],
          name: details.name,
          email: details.email,
          phoneNumber: details.phoneNumber,
          dob: details.dob,
          employment: details.employment,
          streetName: details.streetName,
          city: details.city,
          state: details.state,
          zipCode: details.zipCode,
          note: details.note,
        };

        observer.next({ id: tenantId, ...details });
        observer.complete();
      }, data.apiLatency);
    });
  }

  deleteTenant(tenantId: number): Observable<TenantDetails> {
    return new Observable<TenantDetails>((observer) => {
      setTimeout(() => {
        const id = data.tenants.findIndex((t) => t.id === tenantId);

        if (id === -1) {
          observer.error('Tenant not found');
          return;
        }

        const deletedTenant = data.tenants[id];
        data.tenants.splice(id, 1);

        data.tenantEmergencyContacts = data.tenantEmergencyContacts.filter((c) => c.tenantId !== tenantId);
        data.files = data.files.filter((doc) => doc.tenantId !== tenantId);

        observer.next({
          id: tenantId,
          name: deletedTenant.name,
          email: deletedTenant.email,
          phoneNumber: deletedTenant.phoneNumber,
          dob: deletedTenant.dob,
          employment: deletedTenant.employment,
          streetName: deletedTenant.streetName,
          city: deletedTenant.city,
          state: deletedTenant.state,
          zipCode: deletedTenant.zipCode,
          note: deletedTenant.note,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  updateTenantEmergencyContact(
    tenantId: number,
    tenantEmergencyContactId: number,
    contact: UpdateTenantEmergencyContact,
  ): Observable<TenantEmergencyContact> {
    return new Observable<TenantEmergencyContact>((observer) => {
      setTimeout(() => {
        const tenant = data.tenants.find((t) => t.id === tenantId);

        if (!tenant) {
          observer.error('Tenant emergency contact not found');
          return;
        }

        const existingIndex = data.tenantEmergencyContacts.findIndex((c) => c.tenantId === tenantId);

        if (existingIndex === -1) {
          data.tenantEmergencyContacts.push({ tenantId, id: tenantEmergencyContactId, ...contact });
        } else {
          if (data.tenantEmergencyContacts[existingIndex].id !== tenantEmergencyContactId) {
            observer.error('Tenant emergency contact not found');
          }
          data.tenantEmergencyContacts[existingIndex] = { tenantId, id: tenantEmergencyContactId, ...contact };
        }

        observer.next({ id: tenantEmergencyContactId, ...contact });
        observer.complete();
      }, data.apiLatency);
    });
  }

  updateTenantProfilePic(file: File): Observable<MediaFile> {
    return new Observable<MediaFile>((observer) => {
      setTimeout(() => {
        const nextId = data.files.reduce((maxId, img) => Math.max(maxId, img.id), 0) + 1;

        const profilePicture: MockFiles = {
          id: nextId,
          name: file.name,
          contentType: file.type,
          processingStatus: 'uploaded' as MediaFileStatus,
          size: file.size,
          uploadedDate: new Date(),
        };

        data.files.push(profilePicture);

        setTimeout(() => {
          profilePicture.processingStatus = 'processed';
          profilePicture.thumbnailType = file.type;
          profilePicture.thumbnailProcessingStatus = 'processed';
        }, data.apiLatency * 6);

        observer.next({
          id: profilePicture.id,
          name: profilePicture.name,
          contentType: profilePicture.contentType,
          processingStatus: profilePicture.processingStatus,
          length: profilePicture.size,
          uploadedDate: profilePicture.uploadedDate,
          markedAsCover: false,
          variants: {},
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  uploadTenantDocument(file: File, tenantId?: number): Observable<MediaFile> {
    const formData = new FormData();
    formData.append('file', file);

    return new Observable((observer) => {
      setTimeout(() => {
        if (tenantId) {
          const property = data.properties.find((p) => p.id === tenantId);

          if (!property) {
            observer.error('Tenant not found');
            return;
          }
        }

        const nextId = data.files.reduce((maxId, img) => Math.max(maxId, img.id), 0) + 1;

        const document: MockFiles = {
          id: nextId,
          name: file.name,
          contentType: file.type,
          processingStatus: 'uploaded' as MediaFileStatus,
          tenantId: tenantId,
          size: file.size,
          uploadedDate: new Date(),
        };

        data.files.push(document);

        setTimeout(() => {
          document.processingStatus = 'processed';
          document.thumbnailType = file.type;
          document.thumbnailProcessingStatus = 'processed';
        }, data.apiLatency * 6);

        let variants: Partial<Record<MediaFileVariantType, MediaFileVariant>> = {};
        if (document.thumbnailType && document.thumbnailProcessingStatus) {
          variants['thumbnail'] = {
            contentType: document.thumbnailType,
            processingStatus: document.thumbnailProcessingStatus,
          };
        }

        observer.next({
          id: document.id,
          name: document.name,
          contentType: document.contentType,
          processingStatus: document.processingStatus,
          length: document.size,
          uploadedDate: document.uploadedDate,
          markedAsCover: false,
          variants: variants,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  deleteTenantMedia(mediaId: number): Observable<MediaFile> {
    return new Observable((observer) => {
      setTimeout(() => {
        const id = data.files.findIndex((u) => u.id === mediaId);

        const deletedFile = data.files[id];
        data.files.splice(id, 1);

        let variants: Partial<Record<MediaFileVariantType, MediaFileVariant>> = {};
        if (deletedFile.thumbnailType && deletedFile.thumbnailProcessingStatus) {
          variants['thumbnail'] = {
            contentType: deletedFile.thumbnailType,
            processingStatus: deletedFile.thumbnailProcessingStatus,
          };
        }

        observer.next({
          id: deletedFile.id,
          name: deletedFile.name,
          contentType: deletedFile.contentType,
          processingStatus: deletedFile.processingStatus,
          markedAsCover: false,
          variants: variants,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  generateTenantMediaUrl(mediaId: number, variant: MediaFileVariantType | undefined): string {
    return './img/thumbnails/thumbnail-image.png';
  }
}
