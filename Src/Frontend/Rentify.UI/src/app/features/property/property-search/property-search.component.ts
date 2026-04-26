import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { PropertySummary } from '../../../core/models/property/property-summary.model';
import { LucideAngularModule, Plus, SearchX, SquarePen } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { PROPERTY_SERVICE_TOKEN } from '../../../core/services/tokens/property.token';
import { PropertyService } from '../../../core/services/abstractions/property.service';
import { ApiError } from '../../../core/exceptions/api-error';
import { SkeletonLoaderComponent } from '../../../shared/components/skeleton-loader/skeleton-loader';
import { RouterLink } from '@angular/router';
import { ROUTE_SERVICE_TOKEN } from '../../../core/services/tokens/route.token';
import { RouteService } from '../../../core/services/abstractions/route.service';
import { EnvironmentConfigService } from '../../../core/services/abstractions/environment-config.service';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../../core/services/tokens/environement-config.token';
import { LocalDestroyRef } from '../../../shared/lifecycles/local-destroy-ref';

@Component({
  selector: 'app-property-search',
  standalone: true,
  imports: [LucideAngularModule, ButtonComponent, PaginationComponent, SkeletonLoaderComponent, RouterLink],
  templateUrl: './property-search.component.html',
  styleUrl: './property-search.component.css',
})
export class PropertySearchComponent implements OnInit, OnDestroy {
  readonly ICONS = { Plus, SquarePen, SearchX };

  readonly ITEMS_PER_PAGE = 12;
  readonly DATE_SNAPSHOT: Date;

  public totalItems = 1;
  public currentPage = 1;
  public properties?: (PropertySummary & { propertyImageUrl?: string; onDestroy?: LocalDestroyRef })[];
  public loading = false;
  public disableActions = false;
  propertyNotFoundImagePath: string;

  constructor(
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private envConfigService: EnvironmentConfigService,
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
  ) {
    this.DATE_SNAPSHOT = new Date();
    this.propertyNotFoundImagePath = this.envConfigService.thumbnailImagePath.propertyNotFound;
  }

  ngOnInit(): void {
    this.syncUpProperties(1, { enableLoading: true });
  }

  ngOnDestroy(): void {
    for (const property of this.properties ?? []) {
      property.onDestroy?.destroy();
    }

    this.properties = undefined;
  }

  onPageChange(page: number) {
    this.syncUpProperties(page, { enableActionDisable: true });
  }

  propertyRoute(id: number) {
    return this.routeService.property(id);
  }

  propertyCreateRoute() {
    return this.routeService.propertyCreate();
  }

  private syncUpProperties(currentPage: number, { enableLoading = false, enableActionDisable = false } = {}) {
    if (enableLoading) this.loading = true;
    if (enableActionDisable) this.disableActions = true;

    // Clean previous error
    for (const property of this.properties ?? []) {
      property.onDestroy?.destroy();
    }

    this.propertyService.getPaginatedProperties(currentPage, this.ITEMS_PER_PAGE, this.DATE_SNAPSHOT).subscribe({
      next: (properties) => {
        this.properties = properties.items;

        for (const property of this.properties) {
          if (property.coverPic?.variants?.['cover_pic']?.processingStatus === 'processed') {
            property.onDestroy = new LocalDestroyRef();

            this.propertyService.getPropertyMediaUrl(property.coverPic.id, 'cover_pic').subscribe({
              next: (img) => {
                property.propertyImageUrl = img.url;
                property.onDestroy?.onDestroy(() => {
                  img.destroyFun();
                });
              },
              error: (err) => {
                if (err instanceof ApiError) {
                  console.log('API Error', err.Errors);
                } else {
                  console.error(err);
                }
              },
            });
          } else {
            property.propertyImageUrl = this.propertyNotFoundImagePath;
          }
        }

        this.totalItems = properties.totalItems;
        this.currentPage = properties.currentPage;

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
