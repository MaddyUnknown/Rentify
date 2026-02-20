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
            phone: phoneNumber,
          }));

        observer.next({ totalItems: data.tenants.length, items: tenants, currentPage: page });
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
                name: emergencyContact.name,
                relationship: emergencyContact.relationship,
                phoneNumber: emergencyContact.phoneNumber,
                email: emergencyContact.email,
              }
            : undefined,
          documents: documents.map(({ id, name, contentType, uploadedDate, size, processingStatus }) => ({
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
    contact: UpdateTenantEmergencyContact,
  ): Observable<TenantEmergencyContact> {
    return new Observable<TenantEmergencyContact>((observer) => {
      setTimeout(() => {
        const tenant = data.tenants.find((t) => t.id === tenantId);

        if (!tenant) {
          observer.error('Tenant not found');
          return;
        }

        const existingIndex = data.tenantEmergencyContacts.findIndex((c) => c.tenantId === tenantId);

        if (existingIndex === -1) {
          data.tenantEmergencyContacts.push({ tenantId, ...contact });
        } else {
          data.tenantEmergencyContacts[existingIndex] = { tenantId, ...contact };
        }

        observer.next({ ...contact });
        observer.complete();
      }, data.apiLatency);
    });
  }

  deleteTenantEmergencyContact(tenantId: number): Observable<TenantEmergencyContact> {
    return new Observable<TenantEmergencyContact>((observer) => {
      setTimeout(() => {
        const index = data.tenantEmergencyContacts.findIndex((c) => c.tenantId === tenantId);

        if (index === -1) {
          observer.error('Emergency contact not found');
          return;
        }

        const deleted = data.tenantEmergencyContacts[index];
        data.tenantEmergencyContacts.splice(index, 1);

        observer.next({
          name: deleted.name,
          relationship: deleted.relationship,
          phoneNumber: deleted.phoneNumber,
          email: deleted.email,
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

        observer.next({
          id: document.id,
          name: document.name,
          contentType: document.contentType,
          processingStatus: document.processingStatus,
          size: document.size,
          uploadedDate: document.uploadedDate,
          markedAsCover: false,
          thumbnail:
            document.thumbnailType && document.thumbnailProcessingStatus
              ? {
                  contentType: document.thumbnailType,
                  processingStatus: document.thumbnailProcessingStatus,
                }
              : undefined,
        });
        observer.complete();
      }, data.apiLatency);
    });
  }

  deleteTenantDocument(mediaId: number): Observable<MediaFile> {
    return new Observable((observer) => {
      setTimeout(() => {
        const id = data.files.findIndex((u) => u.id === mediaId);

        const deletedFile = data.files[id];
        data.files.splice(id, 1);

        observer.next({
          id: deletedFile.id,
          name: deletedFile.name,
          contentType: deletedFile.contentType,
          processingStatus: deletedFile.processingStatus,
          markedAsCover: false,
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
}
