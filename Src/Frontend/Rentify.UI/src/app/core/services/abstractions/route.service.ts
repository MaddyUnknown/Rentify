export interface RouteService {
  auth(): any[];
  propeties(): any[];
  property(id: number): any[];
  propertyCreate(): any[];
  tenants(): any[];
  tenant(id: number): any[];
  tenantCreate(): any[];
}
