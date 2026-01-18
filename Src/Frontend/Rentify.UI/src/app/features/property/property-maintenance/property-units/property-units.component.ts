import { Component, DestroyRef, Inject, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { Blocks, CircleX, Plus, Save, SquarePen, Trash2 } from 'lucide-angular';
import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { patchMapWithList } from '../../../../shared/utils/patch-util';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { Unit } from '../../../../core/models/unit/unit.model';
import { PropertyService } from '../../../../core/services/abstractions/property.service';
import { PROPERTY_SERVICE_TOKEN } from '../../../../core/services/tokens/property.token';
import { ApiError } from '../../../../core/exceptions/api-error';
import { LocalDestroyRef } from '../../../../shared/lifecycles/local-destroy-ref';
import { UnitStatus } from '../../../../core/models/unit/unit-status.model';
import { BehaviorSubject, debounceTime, merge, startWith } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { ObservableMap } from '../../../../shared/models/observable-map.model';

type UnitRowForm = {
  id: FormControl<number>;
  name: FormControl<string>;
  type: FormControl<string>;
  size: FormControl<number>;
  status: FormControl<UnitStatus>;
};

type NewUnitRow = {
  kind: 'new';
  disableActions: boolean;
  form: FormGroup<UnitRowForm>;
};

type UnitRow = {
  kind: 'existing';
  mode: EditMode;
  disableActions: boolean;
  form: FormGroup<UnitRowForm>;
  originalState: Unit;
  destroyRef: LocalDestroyRef;
};

@Component({
  selector: 'section[appPropertyUnits]',
  templateUrl: './property-units.component.html',
  styleUrls: ['./property-units.component.css'],
  standalone: true,
  imports: [ButtonComponent, ReactiveFormsModule, PanelComponent, FormsModule, SkeletonLoaderComponent, AsyncPipe],
})
export class PropertyUnitsComponent implements OnChanges, OnInit, OnDestroy {
  readonly ICONS = { CircleX, Blocks, Plus, Save, SquarePen, Trash2 };
  private tempRowID = -1;

  private unitRows: ObservableMap<number, UnitRow>;
  private newRows: ObservableMap<number, NewUnitRow>;

  unitRowList$: BehaviorSubject<(UnitRow | NewUnitRow)[]>;

  @Input({ required: true }) propertyId!: number;
  @Input({ alias: 'appPropertyUnits', required: false }) units?: Unit[];
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private destroyRef: DestroyRef,
    private fb: FormBuilder,
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
  ) {
    this.unitRows = new ObservableMap<number, UnitRow>();
    this.newRows = new ObservableMap<number, NewUnitRow>();
    this.unitRowList$ = new BehaviorSubject<(UnitRow | NewUnitRow)[]>([]);
  }

  //#region Lifecycle hooks
  ngOnInit(): void {
    // Code to sync-up list of rows when elements are added/removed
    const subscribe = merge(this.unitRows.valueChange, this.newRows.valueChange)
      .pipe(startWith(null), debounceTime(100))
      .subscribe(() => this.syncUnitsList());

    this.destroyRef.onDestroy(() => subscribe.unsubscribe());
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['units']) {
      const newUnits: Unit[] = changes['units'].currentValue ?? [];

      patchMapWithList(
        this.unitRows,
        newUnits,
        (unit) => unit.id,
        this.createUnitRow.bind(this),
        this.modifyUnitRow.bind(this),
        this.deleteUnitRow.bind(this),
      );
    }
  }

  ngOnDestroy(): void {
    this.unitRows.forEach((row) => row.destroyRef.destroy());
  }
  //#endregion

  // #region Helper methods
  isInvalid(control: AbstractControl): boolean {
    return control.invalid && (control.dirty || control.touched);
  }

  private createRowForm() {
    return this.fb.group<UnitRowForm>({
      id: this.fb.nonNullable.control<number>(0),
      name: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      type: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      size: this.fb.nonNullable.control<number>(0, { validators: [Validators.required, Validators.min(1)] }),
      status: this.fb.nonNullable.control<UnitStatus>('vacant'),
    });
  }

  private createNewUnitRow(id: number): NewUnitRow {
    const form = this.createRowForm();
    form.controls.id.setValue(id);
    return { kind: 'new', disableActions: false, form };
  }

  private createUnitRow(unit: Unit): UnitRow {
    const newValue: UnitRow = {
      kind: 'existing',
      mode: EditMode.from('view'),
      disableActions: false,
      form: this.createRowForm(),
      originalState: unit,
      destroyRef: LocalDestroyRef.create(),
    };

    newValue.form.reset(unit);
    const subscription = newValue.mode.changes.subscribe((mode) =>
      mode === 'view' ? newValue.form.disable() : newValue.form.enable(),
    );
    newValue.destroyRef.onDestroy(() => subscription.unsubscribe());

    return newValue;
  }

  private modifyUnitRow(existingRow: UnitRow, unit: Unit): UnitRow {
    existingRow.originalState = unit;
    if (existingRow.mode.isView) existingRow.form.reset(unit);
    return existingRow;
  }

  private deleteUnitRow(existingRow: UnitRow): void {
    existingRow.destroyRef.destroy();
  }

  private syncUnitsList() {
    const newRows = [...this.unitRows.values(), ...this.newRows.values()];
    this.unitRowList$.next(newRows);
  }
  // #endregion

  //#region Event handlers
  onAddRowClick() {
    const id = this.tempRowID--;
    this.newRows.set(id, this.createNewUnitRow(id));
  }

  onAddEditClick(row: UnitRow | NewUnitRow) {
    if (row.kind === 'existing' && row.mode.isView) {
      // (Edit clicked) Toggle to edit mode
      row.mode.toggle();
    } else if (!row.form.valid) {
      row.form.markAllAsTouched();
    } else {
      // (Save click ) Save property details
      row.kind === 'new' ? this.addUnits(row.form.getRawValue()) : this.updateUnits(row.form.getRawValue());
    }
  }

  onRemoveCloseClick(row: UnitRow | NewUnitRow) {
    if (row.kind === 'existing' && row.mode.isView) {
      // (Delete clicked) Remove unit
      this.removeUnits(row.form.getRawValue());
    } else {
      if (row.kind === 'new') {
        // Remove temp row
        this.newRows.delete(row.form.controls.id.value);
      } else {
        // (Close clicked) Toggle to view mode
        row.form.reset(row.originalState ?? {});
        row.mode.toggle();
      }
    }
  }
  //#endregion

  //#region Service Calls
  private addUnits(data: Unit) {
    // Disable action
    const item = this.newRows.get(data.id);
    if (item) item.disableActions = true;

    this.propertyService.createUnit(this.propertyId, data).subscribe({
      next: (unit) => {
        this.newRows.delete(data.id);
        this.unitRows.set(unit.id, this.createUnitRow(unit));
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        const item = this.newRows.get(data.id);
        if (item) item.disableActions = false;
      },
    });
  }

  private updateUnits(data: Unit) {
    const item = this.unitRows.get(data.id);
    if (item) item.disableActions = true;

    this.propertyService.updateUnit(this.propertyId, data.id, data).subscribe({
      next: (unit) => {
        const item = this.unitRows.get(unit.id);
        if (item) {
          this.modifyUnitRow(item, unit);
          item.mode.state = 'view';
          item.disableActions = false;
        }
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        const item = this.unitRows.get(data.id);
        if (item) item.disableActions = false;
      },
    });
  }

  private removeUnits(data: Unit) {
    const item = this.unitRows.get(data.id);
    if (item) item.disableActions = true;

    this.propertyService.deleteUnit(this.propertyId, data.id).subscribe({
      next: (deletedUnit) => {
        const item = this.unitRows.get(deletedUnit.id);
        if (item) this.deleteUnitRow(item);

        this.unitRows.delete(deletedUnit.id);
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        const item = this.unitRows.get(data.id);
        if (item) item.disableActions = false;
      },
    });
  }
  //#endregion
}
