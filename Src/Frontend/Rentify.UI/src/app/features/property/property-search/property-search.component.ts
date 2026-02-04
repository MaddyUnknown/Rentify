import { Component, Inject, OnInit } from '@angular/core';
import { PropertySummary } from '../../../core/models/property/property-summary.model';
import { LucideAngularModule, Plus, SearchX, SquarePen } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { PROPERTY_SERVICE_TOKEN } from '../../../core/services/tokens/property.token';
import { PropertyService } from '../../../core/services/abstractions/property.service';
import { ApiError } from '../../../core/exceptions/api-error';
import { PaginatedList } from '../../../core/models/response/paginated-list.model';
import { SkeletonLoaderComponent } from '../../../shared/components/skeleton-loader/skeleton-loader';
import { RouterLink } from '@angular/router';
import { ROUTE_SERVICE_TOKEN } from '../../../core/services/tokens/route.token';
import { RouteService } from '../../../core/services/abstractions/route.service';
import { MediaFileVariantType } from '../../../core/models/media-file/media-file-variant-type.model';

@Component({
  selector: 'app-property-search',
  standalone: true,
  imports: [LucideAngularModule, ButtonComponent, PaginationComponent, SkeletonLoaderComponent, RouterLink],
  templateUrl: './property-search.component.html',
  styleUrl: './property-search.component.css',
})
export class PropertySearchComponent implements OnInit {
  readonly ICONS = { Plus, SquarePen, SearchX };
  readonly DEFAULT_COVER_IMAGE = 'https://images.pexels.com/photos/1732414/pexels-photo-1732414.jpeg';

  readonly ITEMS_PER_PAGE = 12;
  readonly DATE_SNAPSHOT: Date;

  public totalItems = 1;
  public currentPage = 1;
  public properties?: PaginatedList<PropertySummary>;
  public loading = false;
  public disableActions = false;

  constructor(
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
  ) {
    this.DATE_SNAPSHOT = new Date();
  }

  ngOnInit(): void {
    this.syncUpProperties(1, { enableLoading: true });
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

  generatePropertyCoverUrl(property: PropertySummary, variant: MediaFileVariantType): string {
    return property.coverImage?.cover?.processingStatus === 'processed'
      ? this.propertyService.generatePropertyMediaUrl(property.coverImage.id, variant)
      : this.DEFAULT_COVER_IMAGE;
  }

  private syncUpProperties(currentPage: number, { enableLoading = false, enableActionDisable = false } = {}) {
    if (enableLoading) this.loading = true;
    if (enableActionDisable) this.disableActions = true;

    this.propertyService.getPaginatedProperties(currentPage, this.ITEMS_PER_PAGE, this.DATE_SNAPSHOT).subscribe({
      next: (properties) => {
        this.properties = properties;
        this.totalItems = this.properties.totalItems;
        this.currentPage = this.properties.currentPage;

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
