import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PropertyService } from '../../../core/services/abstractions/property.service';

import { PropertyDetailsComponent } from '../shared/property-details/property-details.component';
import { PropertyMediaComponent } from '../shared/property-media/property-media.component';
import { PropertyUnitsComponent } from '../shared/property-units/property-units.component';
import { PropertyLocationComponent } from '../shared/property-location/property-location.component';
import { PROPERTY_SERVICE_TOKEN } from '../../../core/services/tokens/property.token';
import { Property } from '../../../core/models/property/property.model';
import { ApiError } from '../../../core/exceptions/api-error';

@Component({
  selector: 'app-property',
  standalone: true,
  imports: [PropertyDetailsComponent, PropertyMediaComponent, PropertyUnitsComponent, PropertyLocationComponent],
  templateUrl: './property-maintenance.component.html',
  styleUrl: './property-maintenance.component.css',
})
export class PropertyMaintenanceComponent implements OnInit {
  readonly PROPERTY_ID_PARAM = 'id';

  propertyDetailsAgg?: Property;
  propertyLoading: boolean = true;

  constructor(
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    private route: ActivatedRoute,
  ) {}

  get propertyId() {
    const idParam = this.route.snapshot.paramMap.get(this.PROPERTY_ID_PARAM);
    if (!idParam) return 0;

    const propertyId = parseFloat(idParam);
    if (Number.isNaN(propertyId) || !Number.isInteger(propertyId)) return 0;

    return propertyId;
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
