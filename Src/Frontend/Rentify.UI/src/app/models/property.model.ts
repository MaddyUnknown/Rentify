export interface Property {
  propertyId: number;
  name: string;
  address: string;
  description?: string;
}

export interface CreateProperty {
  name: string;
  address: string;
  description?: string;
}

export interface UpdateProperty {
  name: string;
  address: string;
  description?: string;
}

