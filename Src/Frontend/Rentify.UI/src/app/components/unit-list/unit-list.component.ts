import { Component, OnInit } from '@angular/core';
import { UnitService } from '../../services/unit.service';
import { Unit } from '../../models/unit.model';

@Component({
  selector: 'app-unit-list',
  templateUrl: './unit-list.component.html',
  styleUrls: ['./unit-list.component.css']
})
export class UnitListComponent implements OnInit {
  units: Unit[] = [];

  constructor(private unitService: UnitService) { }

  ngOnInit(): void {
    this.unitService.getAllUnits().subscribe(data => {
      this.units = data;
    });
  }

  editUnit(unit: Unit): void {
    // Implement edit logic
  }

  deleteUnit(id: number): void {
    // Implement delete logic
  }

  addUnit(): void {
    // Implement add logic
  }
}
