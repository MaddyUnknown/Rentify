import { Component, OnInit } from '@angular/core';
import { UnitService } from '../../services/unit.service';
import { Unit, CreateUnit } from '../../models/unit.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-unit-form',
  templateUrl: './unit-form.component.html',
  styleUrls: ['./unit-form.component.css']
})
export class UnitFormComponent implements OnInit {
  unit: Unit | CreateUnit = { name: '', description: '', propertyId: 0 };
  isEditMode: boolean = false;

  constructor(
    private unitService: UnitService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.unitService.getUnitById(+id).subscribe(data => {
        this.unit = data;
      });
    }
  }

  saveUnit(): void {
    if (this.isEditMode && (this.unit as Unit).unitId) {
      this.unitService.updateUnit((this.unit as Unit).unitId, this.unit).subscribe(() => {
        this.router.navigate(['/units']);
      });
    } else {
      this.unitService.createUnit(this.unit as CreateUnit).subscribe(() => {
        this.router.navigate(['/units']);
      });
    }
  }
}

