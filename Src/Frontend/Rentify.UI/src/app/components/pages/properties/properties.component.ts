import { Component, OnInit } from '@angular/core';
import { PropertySummary } from '../../../models/property.model';

@Component({
  selector: 'app-properties',
  standalone: true,
  imports: [],
  templateUrl: './properties.component.html',
  styleUrl: './properties.component.css',
})
export class PropertiesComponent implements OnInit {
  public properties: PropertySummary[] = [];

  ngOnInit(): void {
    this.properties = [
      {
        propertyId: 1,
        name: 'Marlin Duplix',
        address: '16/1 Kabitirtha Sarani',
        numberOfUnits: 2,
        numberOfUtility: 1,
      },
      {
        propertyId: 2,
        name: 'Kamalnagar Nivas',
        address: '16 Kamlanager',
        numberOfUnits: 2,
        numberOfUtility: 1,
      },
      {
        propertyId: 3,
        name: 'Kamalnagar Nivas',
        address: '16 Kamlanager',
        numberOfUnits: 2,
        numberOfUtility: 1,
      },
      {
        propertyId: 4,
        name: 'Kamalnagar Nivas',
        address: '16 Kamlanager',
        numberOfUnits: 2,
        numberOfUtility: 1,
      },
      {
        propertyId: 5,
        name: 'Kamalnagar Nivas',
        address: '16 Kamlanager',
        numberOfUnits: 2,
        numberOfUtility: 1,
      },
    ];
  }
}
