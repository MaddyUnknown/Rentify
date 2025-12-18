import { Routes } from '@angular/router';
import { BillingComponent } from './features/billing/billing.component';
import { ContractsComponent } from './features/contracts/contracts.component';
import { TenantsComponent } from './features/tenants/tenants.component';
import { UnitsComponent } from './features/units/units.component';
import { PropertyMaintenanceComponent } from './features/property/property-maintenance/property-maintenance.component';
import { PropertySearchComponent } from './features/property/property-search/property-search.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'properties',
    pathMatch: 'full',
  },
  {
    path: 'properties',
    component: PropertySearchComponent,
    data: {
      navName: 'properties',
    },
  },
  {
    path: 'property/:id',
    component: PropertyMaintenanceComponent,
    data: {
      navName: 'properties',
    },
  },
  {
    path: 'units',
    component: UnitsComponent,
    data: {
      navName: 'units',
    },
  },
  {
    path: 'contracts',
    component: ContractsComponent,
    data: {
      navName: 'contracts',
    },
  },
  {
    path: 'billing',
    component: BillingComponent,
    data: {
      navName: 'billing',
    },
  },
  {
    path: 'tenants',
    component: TenantsComponent,
    data: {
      navName: 'tenants',
    },
  },
];
