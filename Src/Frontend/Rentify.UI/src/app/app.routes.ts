import { Routes } from '@angular/router';
import { PropertiesComponent } from './components/pages/properties/properties.component';
import { UnitsComponent } from './components/pages/units/units.component';
import { ContractsComponent } from './components/pages/contracts/contracts.component';
import { BillingComponent } from './components/pages/billing/billing.component';
import { TenantsComponent } from './components/pages/tenants/tenants.component';

export const routes: Routes = [
    { 
        path: '',
        redirectTo: 'properties',
        pathMatch: 'full' 
    },
    {
        path: 'properties',
        component: PropertiesComponent,
        data: {
            navName: 'properties'
        }
    },
    {
        path: 'units',
        component: UnitsComponent,
        data: {
            navName: 'units'
        }
    },
    {
        path: 'contracts',
        component: ContractsComponent,
        data: {
            navName: 'contracts'
        }
    },
    {
        path: 'billing',
        component: BillingComponent,
        data: {
            navName: 'billing'
        }
    },
    {
        path: 'tenants',
        component: TenantsComponent,
        data: {
            navName: 'tenants'
        }
    }
];
