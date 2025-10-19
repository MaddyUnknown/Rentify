import { Component, OnInit } from '@angular/core';
import { PropertyService } from '../../services/property.service';
import { Property } from '../../models/property.model';

@Component({
  selector: 'app-property-list',
  templateUrl: './property-list.component.html',
  styleUrls: ['./property-list.component.css']
})
export class PropertyListComponent implements OnInit {
  properties: Property[] = [];

  constructor(private propertyService: PropertyService) { }

  ngOnInit(): void {
    this.propertyService.getAllProperties().subscribe(data => {
      this.properties = data;
    });
  }

  editProperty(property: Property): void {
    // Implement edit logic
  }

  deleteProperty(id: number): void {
    // Implement delete logic
  }

  addProperty(): void {
    // Implement add logic
  }
}

