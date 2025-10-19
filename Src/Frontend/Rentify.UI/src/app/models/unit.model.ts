export interface Unit {
  unitId: number;
  name: string;
  description?: string;
  propertyId: number;
}

export interface CreateUnit {
  name: string;
  description?: string;
  propertyId: number;
}

export interface UpdateUnit {
  name: string;
  description?: string;
  propertyId: number;
}
