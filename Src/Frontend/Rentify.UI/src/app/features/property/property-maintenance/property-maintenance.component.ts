import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PropertyService } from '../../../core/services/abstractions/property.service';

import { PropertyDetailsComponent } from './property-details/property-details.component';
import { PropertyMediaComponent } from './property-media/property-media.component';
import { PropertyUnitsComponent } from './property-units/property-units.component';
import { PropertyLocationComponent } from './property-location/property-location.component';
import { PROPERTY_SERVICE_TOKEN } from '../../../core/services/tokens/property.token';
import { Property } from '../../../core/models/property/property.model';
import { ApiError } from '../../../core/exceptions/api-error';
import { ArrowLeft, LucideAngularModule } from 'lucide-angular';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { RouteService } from '../../../core/services/abstractions/route.service';
import { ROUTE_SERVICE_TOKEN } from '../../../core/services/tokens/route.token';

@Component({
  selector: 'app-property-maintenance',
  standalone: true,
  imports: [
    PropertyDetailsComponent,
    PropertyMediaComponent,
    PropertyUnitsComponent,
    PropertyLocationComponent,
    LucideAngularModule,
    ButtonComponent,
    RouterLink,
  ],
  templateUrl: './property-maintenance.component.html',
  styleUrl: './property-maintenance.component.css',
})
export class PropertyMaintenanceComponent implements OnInit {
  readonly ICONS = { ArrowLeft };
  readonly PROPERTY_ID_PARAM = 'id';

  propertyDetailsAgg?: Property;
  propertyLoading: boolean = true;

  constructor(
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
    private route: ActivatedRoute,
  ) {}

  get propertyId() {
    const idParam = this.route.snapshot.paramMap.get(this.PROPERTY_ID_PARAM);
    if (!idParam) return 0;

    const propertyId = parseFloat(idParam);
    if (Number.isNaN(propertyId) || !Number.isInteger(propertyId)) return 0;

    return propertyId;
  }

  propertiesRoute() {
    return this.routeService.propeties();
  }

  ngOnInit(): void {
    this.propertyService.getPropertyAggregateById(this.propertyId).subscribe({
      next: (propertyDetailsAgg) => {
        this.propertyDetailsAgg = propertyDetailsAgg;
        this.propertyLoading = false;
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
