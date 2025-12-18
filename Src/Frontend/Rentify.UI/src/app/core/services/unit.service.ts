import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Unit, CreateUnit, UpdateUnit } from '../models/unit.model';
import { data } from './mock-data/data';

@Injectable({
  providedIn: 'root',
})
export class UnitService {
  constructor() {}

  createUnit(unit: CreateUnit): Observable<Unit> {
    return new Observable<Unit>((observer) => {
      setTimeout(() => {
        const property = data.properties.find((p) => p.id === unit.propertyId);

        if (!property) {
          observer.error('Property not found');
          return;
        }

        const nextId = data.units.reduce((maxId, unit) => Math.max(maxId, unit.id), 0) + 1;

        const newData = {
          id: nextId,
          name: unit.name,
          type: unit.type,
          size: unit.size,
          status: 'vacant',
          propertyId: unit.propertyId,
        };

        data.units.push(newData);

        observer.next(newData);
        observer.complete();
      }, data.apiLatency);
    });
  }

  updateUnit(unit: UpdateUnit): Observable<Unit> {
    return new Observable<Unit>((observer) => {
      setTimeout(() => {
        const id = data.units.findIndex((u) => u.id === unit.id);

        if (id === -1) {
          observer.error('Unit not found');
          return;
        }

        data.units[id] = { ...data.units[id], ...unit };

        observer.next(data.units[id]);
        observer.complete();
      }, data.apiLatency);
    });
  }

  deleteUnit(unitId: number): Observable<Unit> {
    return new Observable<Unit>((observer) => {
      setTimeout(() => {
        const id = data.units.findIndex((u) => u.id === unitId);

        if (id === -1) {
          observer.error('Unit not found');
          return;
        }

        const deletedUnit = data.units[id];
        data.units.splice(id, 1);

        observer.next(deletedUnit);
        observer.complete();
      }, data.apiLatency);
    });
  }
}
