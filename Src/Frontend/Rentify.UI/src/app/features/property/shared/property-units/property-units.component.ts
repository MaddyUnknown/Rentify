import { Component, Inject, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Blocks, CircleX, Plus, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { TableComponent } from '../../../../shared/components/table/table.component';
import { TableColumnDirective } from '../../../../shared/components/table/table-column.directive';
import { createTypeObject } from '../../../../shared/utils/type-untils';
import { FormComponent } from '../../../../shared/components/form/form.component';
import { UIEditState } from '../../../../shared/models/edit-ui-state.model';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { FormsModule } from '@angular/forms';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { Unit } from '../../../../core/models/unit/unit.model';
import { PropertyService } from '../../../../core/services/abstractions/property.service';
import { PROPERTY_SERVICE_TOKEN } from '../../../../core/services/tokens/property.token';
import { CreateUnit } from '../../../../core/models/unit/create-unit.model';
import { UpdateUnit } from '../../../../core/models/unit/update-unit.model';
import { ApiError } from '../../../../core/exceptions/api-error';

@Component({
  selector: 'section[appPropertyUnits]',
  templateUrl: './property-units.component.html',
  styleUrls: ['./property-units.component.css'],
  standalone: true,
  imports: [
    ButtonComponent,
    FormComponent,
    PanelComponent,
    TableComponent,
    TableColumnDirective,
    FormsModule,
    SkeletonLoaderComponent,
  ],
})
export class PropertyUnitsComponent implements OnChanges {
  readonly ICONS = { CircleX, Blocks, Plus, Save, SquarePen, Trash2 };
  private tempRowID = -1;
  private readonly TABLE_DATA_TYPE = createTypeObject<UIEditState<Unit>>();
  private readonly VALIDATORS: { [K in keyof Unit]?: (value: any) => boolean } = {
    name: (value: string) => value.length > 0,
    type: (value: string) => value.length > 0,
    size: (value: number) => value > 0,
  };

  unitRows: UIEditState<Unit>[] = [];

  @Input({ required: true }) propertyId!: number;
  @Input({ alias: 'appPropertyUnits', required: false }) units?: Unit[];
  @Input({ required: false }) loading: boolean = false;

  constructor(@Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService) {}

  //#region Lifecycle hooks
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['units']) {
      const value: Unit[] = changes['units'].currentValue ?? [];

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

  //#region Validators
  private isUnitValid(unit: Unit): {
    isValid: boolean;
    propertyHasError: { [K in keyof Unit]?: boolean };
  } {
    let isValid = true;
    const propertyHasError: Partial<Record<keyof Unit, boolean>> = {};

    for (const key in this.VALIDATORS) {
      const k = key as keyof Unit;
      propertyHasError[k] = !this.VALIDATORS[k]!(unit[k]);
      isValid = isValid && !propertyHasError[k];
    }

    return { isValid, propertyHasError };
  }

  private isUnitPropertyValid(propertyName: keyof Unit, value: any): boolean {
    const validator = this.VALIDATORS[propertyName];
    if (!validator || validator(value)) {
      return true;
    } else {
      return false;
    }
  }
  //#endregion

  //#region Event handlers
  onAddRowClick() {
    this.unitRows.push({
      data: { id: this.tempRowID--, name: '', type: '', size: 0, status: 'vacant' },
      mode: EditMode.from('edit'),
      isNew: true,
    });
  }

  onAddEditClick(row: UIEditState<Unit>) {
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
        const shake: Partial<Record<keyof Unit, boolean>> = {};
        for (const key in validation.propertyHasError) {
          const k = key as keyof Unit;
          if (validation.propertyHasError[k]) {
            shake[k] = true;
          }
        }
        row.doShake = shake;
      }
    }
  }

  onRemoveCloseClick(row: UIEditState<Unit>) {
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
        row.isInvalid = {};
        row.mode.toggle();
      }
    }
  }

  onInputChange(rowId: number, propertyName: keyof Unit) {
    const row = this.unitRows.find((r) => r.data.id === rowId);
    if (row) {
      row.isInvalid = row.isInvalid ?? {};
      row.isInvalid[propertyName] = !this.isUnitPropertyValid(propertyName, row.data[propertyName]);
    }
  }
  //#endregion

  //#region Service Calls
  private addUnits(data: Unit) {
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
    };

    this.propertyService.createUnit(this.propertyId, createUnit).subscribe({
      next: (unit) => {
        const newRow: UIEditState<Unit> = {
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
      error: (err) => {
        const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);
        if (existingRowId !== -1) {
          this.unitRows[existingRowId].isActionDisabled = false;
        }

        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }
      },
    });
  }

  private updateUnits(data: Unit) {
    // Disable action
    const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);
    if (existingRowId !== -1) {
      this.unitRows[existingRowId].isActionDisabled = true;
    }

    // Save new unit details
    const updateUnit: UpdateUnit = {
      name: data.name,
      type: data.type,
      size: data.size,
    };

    this.propertyService.updateUnit(this.propertyId, data.id, updateUnit).subscribe({
      next: (unit) => {
        const updatedRow: UIEditState<Unit> = {
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
      error: (err) => {
        const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);
        if (existingRowId !== -1) {
          this.unitRows[existingRowId].isActionDisabled = false;
        }

        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }
      },
    });
  }

  private removeUnits(data: Unit) {
    // Disable action
    const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);
    if (existingRowId !== -1) {
      this.unitRows[existingRowId].isActionDisabled = true;
    }

    this.propertyService.deleteUnit(this.propertyId, data.id).subscribe({
      next: (deletedUnit) => {
        this.unitRows = this.unitRows.filter((r) => r.data.id !== deletedUnit.id);
      },
      error: (err) => {
        const existingRowId = this.unitRows.findIndex((r) => r.data.id === data.id);
        if (existingRowId !== -1) {
          this.unitRows[existingRowId].isActionDisabled = false;
        }

        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }
      },
    });
  }
  //#endregion
}
