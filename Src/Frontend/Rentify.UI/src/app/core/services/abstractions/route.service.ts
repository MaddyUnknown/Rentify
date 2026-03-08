export interface RouteService {
  propeties(): any[];
  property(id: number): any[];
  propertyCreate(): any[];
  tenants(): any[];
  tenant(id: number): any[];
  tenantCreate(): any[];
}
