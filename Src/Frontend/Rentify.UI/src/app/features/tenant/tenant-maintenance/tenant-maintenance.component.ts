import { Component, OnInit } from '@angular/core';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { ArrowLeft } from 'lucide-angular';
import { TenantDetailsComponent } from './tenant-details/tenant-details.component';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TenantEmergencyContactComponent } from './tenant-emergency-contact/tenant-emergency-contact.component';
import { TenantDocumentsComponent } from './tenant-documents/tenant-documents.component';
import { ROUTE_SERVICE_TOKEN } from '../../../core/services/tokens/route.token';
import { RouteService } from '../../../core/services/abstractions/route.service';
import { Inject } from '@angular/core';
import { TENANT_SERVICE_TOKEN } from '../../../core/services/tokens/tenant.token';
import { TenantService } from '../../../core/services/abstractions/tenant.service';
import { Tenant } from '../../../core/models/tenant/tenant.model';
import { ApiError } from '../../../core/exceptions/api-error';

@Component({
  selector: 'app-tenant-maintenance',
  templateUrl: './tenant-maintenance.component.html',
  styleUrl: './tenant-maintenance.component.css',
  standalone: true,
  imports: [
    ButtonComponent,
    TenantDetailsComponent,
    TenantEmergencyContactComponent,
    TenantDocumentsComponent,
    RouterLink,
  ],
})
export class TenantMaintenanceComponent implements OnInit {
  readonly ICONS = { ArrowLeft };
  readonly TENANT_ID_PARAM = 'id';

  tenantAggregate?: Tenant;
  tenantLoading = false;

  constructor(
    private route: ActivatedRoute,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
    @Inject(TENANT_SERVICE_TOKEN) private tenantService: TenantService,
  ) {}

  get tenantId() {
    const idParam = this.route.snapshot.paramMap.get(this.TENANT_ID_PARAM);
    if (!idParam) return 0;

    const tenantId = parseFloat(idParam);
    if (Number.isNaN(tenantId) || !Number.isInteger(tenantId)) return 0;

    return tenantId;
  }

  tenantsRoute() {
    return this.routeService.tenants();
  }

  ngOnInit(): void {
    this.tenantLoading = true;

    this.tenantService.getTenantAggregateById(this.tenantId).subscribe({
      next: (tenant) => {
        this.tenantAggregate = tenant;
        this.tenantLoading = false;
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }
      },
    });
  }
}
