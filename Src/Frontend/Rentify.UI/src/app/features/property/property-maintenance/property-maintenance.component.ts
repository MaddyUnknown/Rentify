import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PropertyService } from '../../../core/services/property.service';

import { Property } from '../../../core/models/property.model';
import { PropertyDetailsComponent } from '../shared/property-details/property-details.component';
import { PropertyMediaComponent } from '../shared/property-media/property-media.component';
import { PropertyUnitsComponent } from '../shared/property-units/property-units.component';
import { PropertyLocationComponent } from '../shared/property-location/property-location.component';

@Component({
  selector: 'app-property',
  standalone: true,
  imports: [PropertyDetailsComponent, PropertyMediaComponent, PropertyUnitsComponent, PropertyLocationComponent],
  templateUrl: './property-maintenance.component.html',
  styleUrl: './property-maintenance.component.css',
})
export class PropertyMaintenanceComponent implements OnInit {
  readonly PROPERTY_ID_PARAM = 'id';

  propertyDetailsAgg: Property = {};
  propertyLoading: boolean = true;

  constructor(
    private propertyService: PropertyService,
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
    this.propertyService.getPropertyById(this.propertyId).subscribe((propertyDetailsAgg) => {
      this.propertyDetailsAgg = propertyDetailsAgg ?? {};
      this.propertyLoading = false;
    });
  }
}
