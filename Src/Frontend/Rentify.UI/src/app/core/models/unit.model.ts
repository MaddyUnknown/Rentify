export interface Unit {
  id: number;
  name: string;
  type: string;
  size: number;
  status: string;
  propertyId: number;
}

export interface PropertyUnit {
  id: number;
  name: string;
  type: string;
  size: number;
  status: string;
}

export interface CreateUnit {
  name: string;
  type: string;
  size: number;
  propertyId: number;
}

export interface UpdateUnit {
  id: number;
  name: string;
  type: string;
  size: number;
  propertyId: number;
}
