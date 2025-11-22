import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PropertyService } from '../../../services/property.service';

import { Property } from '../../../models/property.model';
import { PropertyDetailsComponent } from './property-details/property-details.component';
import { PropertyMediaComponent } from './property-media/property-media.component';
import { PropertyUnitsComponent } from './property-units/property-units.component';
import { PropertyLocationComponent } from './property-location/property-location.component';

@Component({
  selector: 'app-property',
  standalone: true,
  imports: [PropertyDetailsComponent, PropertyMediaComponent, PropertyUnitsComponent, PropertyLocationComponent],
  templateUrl: './property.component.html',
  styleUrl: './property.component.css',
})
export class PropertyComponent implements OnInit {
  readonly PROPERTY_ID_PARAM = 'id';

  propertyDetailsAgg!: Property;

  constructor(
    private propertyService: PropertyService,
    private route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get(this.PROPERTY_ID_PARAM);
    if (!idParam) return;

    const propertyId = parseFloat(idParam);
    if (Number.isNaN(propertyId) || !Number.isInteger(propertyId)) return;

    this.propertyService.getPropertyDetailsById(propertyId).subscribe((propertyDetailsAgg) => {
      this.propertyDetailsAgg = propertyDetailsAgg ?? {};
    });
  }
}
