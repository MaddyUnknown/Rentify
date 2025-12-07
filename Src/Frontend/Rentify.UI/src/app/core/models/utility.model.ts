export interface Utility {
  utilityId: number;
  utilityType: string;
  providerName: string;
  utilityExternalId: string;
  billingCycle: string;
  billingCycleStartDate: Date;
  propertyId: number;
}

export interface CreateUtility {
  utilityType: string;
  providerName: string;
  utilityExternalId: string;
  billingCycle: string;
  billingCycleStartDate: Date;
  propertyId: number;
}

export interface UpdateUtility {
  utilityType: string;
  providerName: string;
  utilityExternalId: string;
  billingCycle: string;
  billingCycleStartDate: Date;
  propertyId: number;
}
