import { Component, Inject, OnInit } from '@angular/core';
import { LucideAngularModule, Plus, SearchX, SquarePen } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { SkeletonLoaderComponent } from '../../../shared/components/skeleton-loader/skeleton-loader';
import { RouterLink } from '@angular/router';
import { ROUTE_SERVICE_TOKEN } from '../../../core/services/tokens/route.token';
import { RouteService } from '../../../core/services/abstractions/route.service';
import { TenantSummary } from '../../../core/models/tenant/tenant-summary.model';
import { TENANT_SERVICE_TOKEN } from '../../../core/services/tokens/tenant.token';
import { TenantService } from '../../../core/services/abstractions/tenant.service';
import { PaginatedList } from '../../../core/models/response/paginated-list.model';
import { ApiError } from '../../../core/exceptions/api-error';
import { MediaFileVariantType } from '../../../core/models/media-file/media-file-variant-type.model';
import { EnvironmentConfigService } from '../../../core/services/abstractions/environment-config.service';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../../core/services/tokens/environement-config.token';

@Component({
  selector: 'app-tenant-search',
  standalone: true,
  imports: [LucideAngularModule, ButtonComponent, PaginationComponent, SkeletonLoaderComponent, RouterLink],
  templateUrl: './tenant-search.component.html',
  styleUrl: './tenant-search.component.css',
})
export class TenantSearchComponent implements OnInit {
  readonly ICONS = { Plus, SquarePen, SearchX };

  readonly ITEMS_PER_PAGE = 12;
  readonly DATE_SNAPSHOT: Date;

  public totalItems = 1;
  public currentPage = 1;
  public tenants?: PaginatedList<TenantSummary>;
  public loading = false;
  public disableActions = false;
  tenantNotFoundImagePath: string;

  constructor(
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private envConfigService: EnvironmentConfigService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
    @Inject(TENANT_SERVICE_TOKEN) private tenantService: TenantService,
  ) {
    this.DATE_SNAPSHOT = new Date();
    this.tenantNotFoundImagePath = this.envConfigService.thumbnailImagePath.tenantNotFound;
  }

  ngOnInit(): void {
    this.syncUpTenants(1, { enableLoading: true });
  }

  onPageChange(page: number) {
    this.syncUpTenants(page, { enableActionDisable: true });
  }

  tenantRoute(id: number) {
    return this.routeService.tenant(id);
  }

  tenantCreateRoute() {
    return this.routeService.tenantCreate();
  }

  generatePropertyProfilePicUrl(property: TenantSummary): string {
    return property.profilePic?.variants?.['profile_pic']?.processingStatus === 'processed'
      ? this.tenantService.generateTenantMediaUrl(property.profilePic.id, 'profile_pic')
      : this.tenantNotFoundImagePath;
  }

  private syncUpTenants(currentPage: number, { enableLoading = false, enableActionDisable = false } = {}) {
    if (enableLoading) this.loading = true;
    if (enableActionDisable) this.disableActions = true;

    this.tenantService.getPaginatedTenants(currentPage, this.ITEMS_PER_PAGE, this.DATE_SNAPSHOT).subscribe({
      next: (tenants) => {
        this.tenants = tenants;
        this.totalItems = this.tenants.totalItems;
        this.currentPage = this.tenants.currentPage;

        if (enableLoading) this.loading = false;
        if (enableActionDisable) this.disableActions = false;
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.log(err);
        }

        if (enableLoading) this.loading = false;
        if (enableActionDisable) this.disableActions = false;
      },
    });
  }
}
