import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Blocks, CircleX, Plus, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { CreateUnit, PropertyUnit, UpdateUnit } from '../../../../core/models/unit.model';
import { TableColumnDirective } from '../../../../shared/components/table/table-column.directive';
import { createTypeObject } from '../../../../shared/utils/type-untils';
import { FormComponent } from '../../../../shared/components/form/form.component';
import { EditState } from '../../../../shared/models/edit-state.model';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { FormsModule } from '@angular/forms';
import { UnitService } from '../../../../core/services/unit.service';

@Component({
  selector: 'section[appPropertyUnits]',
  templateUrl: './property-units.component.html',
  styleUrls: ['./property-units.component.css'],
  standalone: true,
  imports: [ButtonComponent, FormComponent, PanelComponent, TableComponent, TableColumnDirective, FormsModule],
})
export class PropertyUnitsComponent implements OnChanges {
  readonly ICONS = { CircleX, Blocks, Plus, Save, SquarePen, Trash2 };
  private tempRowID = -1;
  private readonly TABLE_DATA_TYPE = createTypeObject<EditState<PropertyUnit>>();
  private readonly VALIDATORS: { [K in keyof PropertyUnit]?: (value: any) => boolean } = {
    name: (value: string) => value.length > 0,
    type: (value: string) => value.length > 0,
    size: (value: number) => value > 0,
  };

  unitRows: EditState<PropertyUnit>[] = [];

  @Input({ required: true }) propertyId!: number;
  @Input({ alias: 'appPropertyUnits', required: false }) units?: PropertyUnit[];

  constructor(private unitService: UnitService) {}

  //#region Lifecycle hooks
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['units']) {
      const value: PropertyUnit[] = changes['units'].currentValue ?? [];

      const tempList = this.unitRows.filter((row) => row.isNew);

      const updatedList =
        value.map((unit) => {
          const existingRow = this.unitRows.find((row) => row.data.id === unit.id);

          if (existingRow) {
            return {
              data: existingRow.mode.isView ? unit : existingRow.data,
              previousData: existingRow.mode.isView ? undefined : unit,
              mode: existingRow.mode,
            };
          } else {
            return { data: unit, mode: EditMode.from('view') };
          }
        }) ?? [];

      this.unitRows = [...updatedList, ...tempList];
    }
  }
  //#endregion

  get tableDataType() {
    return this.TABLE_DATA_TYPE;
  }

  private isUnitValid(unit: PropertyUnit): {
    isValid: boolean;
    propertyHasError: { [K in keyof PropertyUnit]?: boolean };
  } {
    let isValid = true;
    const propertyHasError: Partial<Record<keyof PropertyUnit, boolean>> = {};

    for (const key in this.VALIDATORS) {
      const k = key as keyof PropertyUnit;
      propertyHasError[k] = !this.VALIDATORS[k]!(unit[k]);
      isValid = isValid && !propertyHasError[k];
    }

    return { isValid, propertyHasError };
  }

  private isUnitPropertyValid(propertyName: keyof PropertyUnit, value: any): boolean {
    const validator = this.VALIDATORS[propertyName];
    if (!validator || validator(value)) {
      return true;
    } else {
      return false;
    }
  }

  //#region Event handlers
  onAddRowClick() {
    this.unitRows.push({
      data: { id: this.tempRowID--, name: '', type: '', size: 0, status: '' },
      mode: EditMode.from('edit'),
      isNew: true,
    });
  }

  onAddEditClick(row: EditState<PropertyUnit>) {
    if (row.mode.isView) {
      // (Edit clicked) Toggle to edit mode
      row.previousData = structuredClone(row.data);
      row.mode.toggle();
    } else {
      // Save units
      const validation = this.isUnitValid(row.data);
      row.isInvalid = validation.propertyHasError;
      if (validation.isValid) {
        row.isNew ? this.addUnits(row.data) : this.updateUnits(row.data);
      } else {
        const shake: Partial<Record<keyof PropertyUnit, boolean>> = {};
        for (const key in validation.propertyHasError) {
          const k = key as keyof PropertyUnit;
          if (validation.propertyHasError[k]) {
            shake[k] = true;
          }
        }
        row.doShake = shake;
      }
    }
  }

  onRemoveCloseClick(row: EditState<PropertyUnit>) {
    if (row.mode.isView) {
      // (Delete clicked) Remove unit
      this.removeUnits(row.data);
    } else {
      if (row.isNew) {
        // Remove temp row
        this.unitRows = this.unitRows.filter((r) => r.data.id !== row.data.id);
      } else {
        // (Close clicked) Toggle to view mode
        row.data = row.previousData ?? row.data;
        row.previousData = undefined;
        row.mode.toggle();
      }
    }
  }

  onInputChange(rowId: number, propertyName: keyof PropertyUnit) {
    const row = this.unitRows.find((r) => r.data.id === rowId);
    if (row) {
      row.isInvalid = row.isInvalid ?? {};
      row.isInvalid[propertyName] = !this.isUnitPropertyValid(propertyName, row.data[propertyName]);
    }
  }
  //#endregion

  //#region Service Calls
  private addUnits(data: PropertyUnit) {
    // Disable action
    const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);
    if (existingRowId !== -1) {
      this.unitRows[existingRowId].isActionDisabled = true;
    }

    // Save new unit details
    const createUnit: CreateUnit = {
      name: data.name,
      type: data.type,
      size: data.size,
      propertyId: this.propertyId,
    };

    this.unitService.createUnit(createUnit).subscribe({
      next: (unit) => {
        const newRow: EditState<PropertyUnit> = {
          data: {
            id: unit.id,
            name: unit.name,
            type: unit.type,
            size: unit.size,
            status: unit.status,
          },
          mode: EditMode.from('view'),
        };

        //Update newly received unit
        const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);

        if (existingRowId !== -1) {
          this.unitRows[existingRowId] = newRow;
        }
      },
      error: (error) => console.error(error),
    });
  }

  private updateUnits(data: PropertyUnit) {
    // Disable action
    const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);
    if (existingRowId !== -1) {
      this.unitRows[existingRowId].isActionDisabled = true;
    }

    // Save new unit details
    const updateUnit: UpdateUnit = {
      id: data.id,
      name: data.name,
      type: data.type,
      size: data.size,
      propertyId: this.propertyId,
    };

    this.unitService.updateUnit(updateUnit).subscribe({
      next: (unit) => {
        const updatedRow: EditState<PropertyUnit> = {
          data: {
            id: unit.id,
            name: unit.name,
            type: unit.type,
            size: unit.size,
            status: unit.status,
          },
          mode: EditMode.from('view'),
        };

        //Update received unit
        const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);

        if (existingRowId !== -1) {
          this.unitRows[existingRowId] = updatedRow;
        }
      },
      error: (err) => console.error(err),
    });
  }

  private removeUnits(data: PropertyUnit) {
    // Disable action
    const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);
    if (existingRowId !== -1) {
      this.unitRows[existingRowId].isActionDisabled = true;
    }

    this.unitService.deleteUnit(data.id).subscribe({
      next: () => {
        this.unitRows = this.unitRows.filter((r) => r.data.id !== data.id);
      },
      error: (err) => console.error(err),
    });
  }
  //#endregion
}
