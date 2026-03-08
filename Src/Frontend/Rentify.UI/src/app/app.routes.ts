import { Routes } from '@angular/router';
import { BillingComponent } from './features/billing/billing.component';
import { ContractsComponent } from './features/contracts/contracts.component';
import { TenantSearchComponent } from './features/tenant/tenant-search/tenant-search.component';
import { PropertyMaintenanceComponent } from './features/property/property-maintenance/property-maintenance.component';
import { PropertySearchComponent } from './features/property/property-search/property-search.component';
import { RoutesConstants } from './core/constants/routes.constants';
import { PropertyCreateComponent } from './features/property/property-create/property-create.component';
import { TenantMaintenanceComponent } from './features/tenant/tenant-maintenance/tenant-maintenance.component';
import { TenantCreateComponent } from './features/tenant/tenant-create/tenant-create.component';

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
    path: RoutesConstants.Tenants,
    component: TenantSearchComponent,
    data: {
      navName: 'tenants',
    },
  },
  {
    path: RoutesConstants.TenantCreate,
    component: TenantCreateComponent,
    data: {
      navName: 'tenants',
    },
  },
  {
    path: `${RoutesConstants.Tenant}/:id`,
    component: TenantMaintenanceComponent,
    data: {
      navName: 'tenants',
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
];
