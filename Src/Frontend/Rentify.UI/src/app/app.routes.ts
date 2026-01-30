import { Routes } from '@angular/router';
import { BillingComponent } from './features/billing/billing.component';
import { ContractsComponent } from './features/contracts/contracts.component';
import { TenantsComponent } from './features/tenants/tenants.component';
import { UnitsComponent } from './features/units/units.component';
import { PropertyMaintenanceComponent } from './features/property/property-maintenance/property-maintenance.component';
import { PropertySearchComponent } from './features/property/property-search/property-search.component';
import { RoutesConstants } from './core/constants/routes.constants';
import { PropertyCreateComponent } from './features/property/property-create/property-create.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: RoutesConstants.Properties,
    pathMatch: 'full',
  },
  {
    path: RoutesConstants.Properties,
    component: PropertySearchComponent,
    data: {
      navName: 'properties',
    },
  },
  {
    path: RoutesConstants.PropertyCreate,
    component: PropertyCreateComponent,
    data: {
      navName: 'properties',
    },
  },
  {
    path: `${RoutesConstants.Property}/:id`,
    component: PropertyMaintenanceComponent,
    data: {
      navName: 'properties',
    },
  },
  {
    path: RoutesConstants.Units,
    component: UnitsComponent,
    data: {
      navName: 'units',
    },
  },
  {
    path: RoutesConstants.Contracts,
    component: ContractsComponent,
    data: {
      navName: 'contracts',
    },
  },
  {
    path: RoutesConstants.Billing,
    component: BillingComponent,
    data: {
      navName: 'billing',
    },
  },
  {
    path: RoutesConstants.Tenants,
    component: TenantsComponent,
    data: {
      navName: 'tenants',
    },
  },
];
