import { Component, OnInit } from '@angular/core';
import { PropertyService } from '../../services/property.service';
import { Property, CreateProperty } from '../../models/property.model';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-property-form',
  templateUrl: './property-form.component.html',
  styleUrls: ['./property-form.component.css'],
  standalone: true,
  imports: [FormsModule],
})
export class PropertyFormComponent implements OnInit {
  property: Property | CreateProperty = { name: '', address: '', description: '' };
  isEditMode: boolean = false;

  // constructor(
  //   private propertyService: PropertyService,
  //   private route: ActivatedRoute,
  //   private router: Router
  // ) {}

  ngOnInit(): void {
    // const id = this.route.snapshot.paramMap.get('id');
    // if (id) {
    //   this.isEditMode = true;
    //   this.propertyService.getPropertyById(+id).subscribe(data => {
    //     this.property = data;
    //   });
    // }
  }

  saveProperty(): void {
    // if (this.isEditMode && (this.property as Property).propertyId) {
    //   this.propertyService.updateProperty((this.property as Property).propertyId, this.property).subscribe(() => {
    //     this.router.navigate(['/properties']);
    //   });
    // } else {
    //   this.propertyService.createProperty(this.property as CreateProperty).subscribe(() => {
    //     this.router.navigate(['/properties']);
    //   });
    // }
  }
}

