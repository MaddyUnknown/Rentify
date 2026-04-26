import { AfterViewInit, Component, DestroyRef, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { Route, Router, RouterLink } from '@angular/router';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import {
  Blocks,
  CircleX,
  Images,
  InfoIcon,
  LucideAngularModule,
  Map,
  MapPinOff,
  Plus,
  Save,
  SquarePen,
  Star,
  Trash2,
} from 'lucide-angular';
import { ROUTE_SERVICE_TOKEN } from '../../../core/services/tokens/route.token';
import { RouteService } from '../../../core/services/abstractions/route.service';
import { PanelComponent } from '../../../shared/components/panel/panel.component';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { PROPERTY_SERVICE_TOKEN } from '../../../core/services/tokens/property.token';
import { PropertyService } from '../../../core/services/abstractions/property.service';
import { SpinnerLoaderComponent } from '../../../shared/components/spinner-loader/spinner-loader.component';
import { MediaFile } from '../../../core/models/media-file/media-file.model';
import { LocalDestroyRef } from '../../../shared/lifecycles/local-destroy-ref';
import { MediaFileVariantType } from '../../../core/models/media-file/media-file-variant-type.model';
import { BehaviorSubject, debounceTime, merge, Observable, startWith } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { ObservableMap } from '../../../shared/models/observable-map.model';
import { MediaStatusPollingService } from '../../../shared/services/media-status-polling.service';
import { ApiError } from '../../../core/exceptions/api-error';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../../core/services/tokens/environement-config.token';
import { EnvironmentConfigService } from '../../../core/services/abstractions/environment-config.service';
import * as L from 'leaflet';
import { GeoLocationService } from '../../../shared/services/geolocation.service';
import { Location } from '../../../core/models/location/location.model';
import { SkeletonLoaderComponent } from '../../../shared/components/skeleton-loader/skeleton-loader';
import { CreateProperty } from '../../../core/models/property/create-property.model';

type PropertyForm = {
  details: FormGroup<PropertyDetailsForm>;
  location: FormControl<Location | undefined>;
  mediaFileIds: FormArray<FormControl<number>>;
  units: FormArray<FormGroup<PropertyUnitForm>>;
  requestedCoverPicId: FormControl<number | undefined>;
};

type PropertyDetailsForm = {
  name: FormControl<string>;
  streetName: FormControl<string>;
  city: FormControl<string>;
  state: FormControl<string>;
  zipCode: FormControl<string>;
  description: FormControl<string>;
};

type PropertyUnitForm = {
  name: FormControl<string>;
  type: FormControl<string>;
  size: FormControl<number>;
};

type NewMediaFileRow = {
  kind: 'new';
  data: MediaFile;
};

type MediaFileRow = {
  kind: 'existing';
  data: MediaFile;
  imageUrl?: string;
  disableActions: boolean;
  destoryPollingRef: LocalDestroyRef;
  destroyImageRef: LocalDestroyRef;
};

@Component({
  selector: 'app-property-create',
  standalone: true,
  imports: [
    AsyncPipe,
    ButtonComponent,
    LucideAngularModule,
    RouterLink,
    PanelComponent,
    ReactiveFormsModule,
    SpinnerLoaderComponent,
    SkeletonLoaderComponent,
  ],
  templateUrl: './property-create.component.html',
  styleUrl: './property-create.component.css',
})
export class PropertyCreateComponent implements OnInit, AfterViewInit {
  readonly ICONS = { Blocks, CircleX, Images, InfoIcon, Map, MapPinOff, Plus, Save, SquarePen, Star, Trash2 };
  readonly MAP_SELECTOR = 'map';
  readonly LEAFLY_PROVIDER_URL = 'https://{s}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png';
  readonly LEAFLY_PROVIDER_ATTRIBUTION =
    '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Tiles style by <a href="https://www.hotosm.org/" target="_blank">Humanitarian OpenStreetMap Team</a> hosted by <a href="https://openstreetmap.fr/" target="_blank">OpenStreetMap France</a>';

  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  private propertyImageState: {
    images: ObservableMap<number, MediaFileRow>;
    newImages: ObservableMap<number, NewMediaFileRow>;
    newImageTempId: number;
  };

  private propertyLocationState: {
    map?: L.Map;
    locationMarker?: L.Marker;
  };

  propertyImageList$: BehaviorSubject<(NewMediaFileRow | MediaFileRow)[]>;
  requestedCoverPicId?: number;
  mapLoading: boolean;
  propertyForm: FormGroup<PropertyForm>;
  disableActions: boolean;
  propertyMediaProcessingImagePath: string;

  constructor(
    private destroyRef: DestroyRef,
    private fb: FormBuilder,
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private envConfigService: EnvironmentConfigService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    private geolocationService: GeoLocationService,
    private fileStatusPollingService: MediaStatusPollingService,
    private router: Router,
  ) {
    this.propertyForm = this.fb.group<PropertyForm>({
      details: this.fb.group<PropertyDetailsForm>({
        name: this.fb.nonNullable.control('', { validators: [Validators.required] }),
        streetName: this.fb.nonNullable.control('', { validators: [Validators.required] }),
        city: this.fb.nonNullable.control('', { validators: [Validators.required] }),
        state: this.fb.nonNullable.control('', { validators: [Validators.required] }),
        zipCode: this.fb.nonNullable.control('', { validators: [Validators.required] }),
        description: this.fb.nonNullable.control(''),
      }),
      location: this.fb.nonNullable.control<Location | undefined>(undefined),
      mediaFileIds: this.fb.array<FormControl<number>>([]),
      units: this.fb.array<FormGroup<PropertyUnitForm>>([]),
      requestedCoverPicId: this.fb.nonNullable.control<number | undefined>(undefined),
    });

    this.propertyImageState = {
      images: new ObservableMap<number, MediaFileRow>(),
      newImages: new ObservableMap<number, NewMediaFileRow>(),
      newImageTempId: -1,
    };

    this.propertyLocationState = {};
    this.mapLoading = false;

    this.propertyImageList$ = new BehaviorSubject<(NewMediaFileRow | MediaFileRow)[]>([]);

    this.disableActions = false;

    this.propertyMediaProcessingImagePath = envConfigService.thumbnailImagePath.propertyMediaProcessing;
  }

  // #region Lifecycle hooks
  ngOnInit(): void {
    // Code to sync-up list of rows when elements are added/removed
    const subscription = merge(
      this.propertyImageState.images.valueChange,
      this.propertyImageState.newImages.valueChange,
    )
      .pipe(startWith(null), debounceTime(100))
      .subscribe(() => {
        this.syncImageList();
      });

    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  ngAfterViewInit(): void {
    //Hack to avoid expressionChangedAfterItHasBeenCheckedError
    setTimeout(() => (this.mapLoading = true));

    const { latitude, longitude } = this.envConfigService.defaultPropertyLatLon;

    this.propertyLocationState.map = L.map(this.MAP_SELECTOR).setView([latitude, longitude], 18);
    L.tileLayer(this.LEAFLY_PROVIDER_URL, {
      attribution: this.LEAFLY_PROVIDER_ATTRIBUTION,
      maxZoom: 19,
    }).addTo(this.propertyLocationState.map);

    this.geolocationService.getCurrentLocation().subscribe({
      next: (location) => {
        this.panMap(location);
        this.propertyLocationState.map?.on('click', this.onMapClick.bind(this));
        setTimeout(() => (this.mapLoading = false));
      },
      error: (err) => {
        console.error(err);
        setTimeout(() => (this.mapLoading = false));
      },
    });
  }
  // #endregion

  // #region Helper methods
  isInvalid(control: AbstractControl): boolean {
    return control.invalid && (control.dirty || control.touched);
  }

  propertiesRoute() {
    return this.routeService.propeties();
  }
  // #endregion

  // #region Event handlers
  onSaveClick() {
    if (!this.propertyForm.valid) {
      this.propertyForm.markAllAsTouched();
      return;
    }

    this.disableActions = true;

    const property: CreateProperty = this.propertyForm.getRawValue();

    this.propertyService.createProperty(property).subscribe({
      next: (property) => {
        this.disableActions = false;
        this.router.navigate(this.routeService.property(property.id));
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        this.disableActions = false;
      },
    });
  }

  // #endregion

  // #region Helper methods - Property Images
  private syncImageList() {
    const imageList = [...this.propertyImageState.images.values(), ...this.propertyImageState.newImages.values()];
    this.propertyImageList$.next(imageList);
  }

  private createNewImageRow(mediaFile: MediaFile): NewMediaFileRow {
    const data: NewMediaFileRow = { kind: 'new', data: mediaFile };
    return data;
  }

  private createImageRow(mediaFile: MediaFile): MediaFileRow {
    const data: MediaFileRow = {
      kind: 'existing',
      data: mediaFile,
      disableActions: false,
      destoryPollingRef: new LocalDestroyRef(),
      destroyImageRef: new LocalDestroyRef(),
    };

    this.setupImageUrlAndPolling(data);
    return data;
  }

  private deleteImageRow(mediaFileRow: MediaFileRow) {
    mediaFileRow.destoryPollingRef.destroy();
    mediaFileRow.destroyImageRef.destroy();
  }

  private setupImageUrlAndPolling(mediaFileRow: MediaFileRow) {
    if (mediaFileRow.data.processingStatus === 'uploading' || mediaFileRow.data.processingStatus === 'uploaded') {
      this.setupImagePolling(mediaFileRow);
    } else if (mediaFileRow.data.processingStatus === 'processed') {
      this.setupImageUrl(mediaFileRow);
    }
  }

  private setupImageUrl(mediaFileRow: MediaFileRow) {
    if (
      mediaFileRow.data.processingStatus !== 'processed' ||
      mediaFileRow.data.variants?.['thumbnail']?.processingStatus !== 'processed'
    )
      return;

    this.propertyService.getPropertyMediaUrl(mediaFileRow.data.id, 'thumbnail').subscribe({
      next: (img) => {
        mediaFileRow.imageUrl = img.url;
        mediaFileRow.destroyImageRef.onDestroy(() => {
          img.destroyFun();
        });
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }
      },
    });
  }

  private setupImagePolling(mediaFileRow: MediaFileRow) {
    if (mediaFileRow.data.processingStatus !== 'uploading' && mediaFileRow.data.processingStatus !== 'uploaded') return;

    const subscription = this.fileStatusPollingService.getStatusObservable(mediaFileRow.data.id).subscribe({
      next: (data) => {
        if (data.processingStatus === 'deleted' || data.processingStatus === 'failed') {
          const existingRow = this.propertyImageState.images.get(data.id);
          if (existingRow) this.deleteImageRow(existingRow);
          this.propertyImageState.images.delete(data.id);
        } else {
          const existingRow = this.propertyImageState.images.get(data.id);
          if (existingRow) {
            existingRow.destroyImageRef.destroy();
            existingRow.data = data;
            this.setupImageUrl(existingRow);
          }
        }
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        // Only stop polling | TO-DO: Show error to users for reloading the page to see latest upload status
        const existingRow = this.propertyImageState.images.get(mediaFileRow.data.id);
        if (existingRow) existingRow.destoryPollingRef.destroy();
      },
    });

    mediaFileRow.destoryPollingRef.onDestroy(() => subscription.unsubscribe());
  }
  // #endregion

  // #region Event handlers - Property Images
  onFileInputClick() {
    this.fileInput?.nativeElement.click();
  }

  onFileSelected() {
    const files = this.fileInput?.nativeElement.files;
    if (!files || files.length === 0) return;
    for (let i = 0; i < files.length; i++) {
      const file = files.item(i);
      if (!file) continue;
      const newFileId = this.propertyImageState.newImageTempId--;
      const newMediaFile: MediaFile = {
        id: newFileId,
        name: file.name,
        processingStatus: 'uploading',
      };
      const newMediaFileRow: NewMediaFileRow = this.createNewImageRow(newMediaFile);
      this.propertyImageState.newImages.set(newFileId, newMediaFileRow);
      this.propertyService.uploadMediaFile(file).subscribe({
        next: (image) => {
          this.propertyImageState.newImages.delete(newFileId);
          this.propertyImageState.images.set(image.id, this.createImageRow(image));

          //Push to form
          this.propertyForm.controls.mediaFileIds.push(this.fb.nonNullable.control(image.id));
        },
        error: (err) => {
          if (err instanceof ApiError) {
            console.log('API Error', err.Errors);
          } else {
            console.error(err);
          }
          this.propertyImageState.newImages.delete(newFileId);
        },
      });
    }
  }

  onImageRemoveClick(row: MediaFileRow) {
    // Disable action
    const existingRow = this.propertyImageState.images.get(row.data.id);
    if (existingRow) existingRow.disableActions = true;
    this.propertyService.deleteMediaFile(row.data.id).subscribe({
      next: (deletedUnit) => {
        const existingRow = this.propertyImageState.images.get(deletedUnit.id);
        if (existingRow) this.deleteImageRow(existingRow);
        this.propertyImageState.images.delete(deletedUnit.id);

        //Remove from form
        const index = this.propertyForm.controls.mediaFileIds.controls.findIndex((x) => x.value === deletedUnit.id);
        if (index !== -1) this.propertyForm.controls.mediaFileIds.removeAt(index);
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }
        const existingRow = this.propertyImageState.images.get(row.data.id);
        if (existingRow) existingRow.disableActions = false;
      },
    });
  }

  onMarkAsCover(row: MediaFileRow) {
    this.requestedCoverPicId = row.data.id;
    this.propertyForm.controls.requestedCoverPicId.setValue(row.data.id);
  }
  // #endregion

  // #region Helper methods - Property Units
  private createRowForm() {
    return this.fb.group<PropertyUnitForm>({
      name: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      type: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
      size: this.fb.nonNullable.control<number>(0, { validators: [Validators.required, Validators.min(1)] }),
    });
  }
  // #endregion

  // #region Event handlers - Property Units
  onAddRowClick() {
    this.propertyForm.controls.units.push(this.createRowForm());
  }

  onUnitRemoveClick(rowIndex: number) {
    this.propertyForm.controls.units.removeAt(rowIndex);
  }
  // #endregion

  // #region Helper methods - Property Location
  private updateLocationMarker(location: Location | undefined) {
    if (!this.propertyLocationState.map) return;

    this.propertyLocationState.locationMarker?.remove();
    this.propertyLocationState.locationMarker = undefined;

    if (location) {
      const { latitude: markerLat, longitude: markerLng } = location;
      this.propertyLocationState.locationMarker = L.marker([markerLat, markerLng]);
      this.propertyLocationState.locationMarker.addTo(this.propertyLocationState.map);
    }
  }

  private panMap(location: Location) {
    this.propertyLocationState.map?.panTo([location.latitude, location.longitude], { animate: true });
  }

  private deleteLocation() {
    const currentState = undefined;
    this.propertyForm.controls.location.setValue(currentState);
    this.updateLocationMarker(currentState);
  }
  // #endregion

  // #region Event handlers - Property Location
  onMapClick(e: L.LeafletMouseEvent) {
    if (!this.propertyLocationState.map) return;

    const { lat: latitude, lng: longitude } = e.latlng;
    const currentState = { latitude, longitude };
    this.propertyForm.controls.location.setValue(currentState);
    this.updateLocationMarker(currentState);
  }

  onClearClick() {
    this.deleteLocation();
  }
  // #endregion
}
