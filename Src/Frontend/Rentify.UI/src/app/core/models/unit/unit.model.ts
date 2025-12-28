import { UnitStatus } from './unit-status.model';

export interface Unit {
  id: number;
  name: string;
  type: string;
  size: number;
  status: UnitStatus;
}
