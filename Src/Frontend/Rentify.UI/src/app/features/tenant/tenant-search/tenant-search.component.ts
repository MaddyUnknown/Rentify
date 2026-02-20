import { Component, Inject, OnInit } from '@angular/core';
import { LucideAngularModule, Plus, SearchX, SquarePen } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { SkeletonLoaderComponent } from '../../../shared/components/skeleton-loader/skeleton-loader';
import { RouterLink } from '@angular/router';
import { ROUTE_SERVICE_TOKEN } from '../../../core/services/tokens/route.token';
import { RouteService } from '../../../core/services/abstractions/route.service';
import { TenantSummary } from '../../../core/models/tenant/tenant-summary.model';

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

  public totalItems = 0;
  public currentPage = 1;
  public loading = false;
  public disableActions = false;

  public tenants: TenantSummary[] = [
    { id: 1, name: 'Jordan Rivers', email: 'jordan@rentify.io', phone: '+1 415 555 9204', status: 'active' },
    { id: 2, name: 'Maya Chen', email: 'maya@rentify.io', phone: '+1 206 555 1188', status: 'active' },
    { id: 3, name: 'Rafael Ortiz', email: 'rafael@rentify.io', phone: '+1 305 555 4412', status: 'inactive' },
  ];

  constructor(@Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService) {}

  ngOnInit(): void {
    this.totalItems = this.tenants.length;
  }

  onPageChange(page: number) {
    this.currentPage = page;
  }

  tenantRoute(id: number) {
    return this.routeService.tenant(id);
  }

  tenantCreateRoute() {
    return this.routeService.tenantCreate();
  }
}
