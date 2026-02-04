import { AfterViewInit, Component, Inject, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CircleX, Map, MapPin, MapPinOff, Save, SquarePen } from 'lucide-angular';
import * as L from 'leaflet';

import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { GeoLocationService } from '../../../../shared/services/geolocation.service';
import { EnvironmentConfigService } from '../../../../core/services/abstractions/environment-config.service';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { PropertyService } from '../../../../core/services/abstractions/property.service';
import { PROPERTY_SERVICE_TOKEN } from '../../../../core/services/tokens/property.token';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../../../core/services/tokens/environement-config.token';
import { Location } from '../../../../core/models/location/location.model';
import { UpdatePropertyLocation } from '../../../../core/models/location/update-property-location.model';
import { ApiError } from '../../../../core/exceptions/api-error';

@Component({
  selector: 'section[appPropertyLocation]',
  templateUrl: './property-location.component.html',
  styleUrls: ['./property-location.component.css'],
  imports: [PanelComponent, SkeletonLoaderComponent, ButtonComponent],
  standalone: true,
})
export class PropertyLocationComponent implements OnChanges, AfterViewInit {
  readonly ICONS = { CircleX, Map, MapPin, MapPinOff, Save, SquarePen };
  readonly MAP_SELECTOR = 'map';
  readonly LEAFLY_PROVIDER_URL = 'https://{s}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png';
  readonly LEAFLY_PROVIDER_ATTRIBUTION =
    '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Tiles style by <a href="https://www.hotosm.org/" target="_blank">Humanitarian OpenStreetMap Team</a> hosted by <a href="https://openstreetmap.fr/" target="_blank">OpenStreetMap France</a>';

  private map?: L.Map;
  private locationMarker?: L.Marker;

  mode = EditMode.from('view');
  disableActions = false;
  currentState?: Location;
  private originalState?: Location;

  @Input({ required: true }) propertyId!: number;
  @Input({ alias: 'appPropertyLocation', required: false }) location?: Location;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    @Inject(PROPERTY_SERVICE_TOKEN) private propertyService: PropertyService,
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private envConfigService: EnvironmentConfigService,
    private geolocationService: GeoLocationService,
  ) {}

  //#region Lifecycle hooks
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['location']) {
      const value: Location | undefined = changes['location'].currentValue ?? undefined;

      if (this.mode.isView) this.currentState = value;
      this.originalState = value;

      if (value) {
        this.updateLocationMarker(value);
        this.panMap(value);
      }
    }
  }

  ngAfterViewInit(): void {
    const { latitude, longitude } = this.envConfigService.defaultPropertyLatLon;

    this.map = L.map(this.MAP_SELECTOR).setView([latitude, longitude], 18);
    L.tileLayer(this.LEAFLY_PROVIDER_URL, {
      attribution: this.LEAFLY_PROVIDER_ATTRIBUTION,
      maxZoom: 19,
    }).addTo(this.map);

    this.map.on('click', this.onMapClick.bind(this));

    if (this.currentState) {
      this.updateLocationMarker(this.currentState);
      this.panMap(this.currentState);
    } else {
      this.geolocationService.getCurrentLocation().subscribe({
        next: (location) => {
          this.panMap(location);
        },
        error: (err) => console.error(err),
      });
    }
  }
  //#endregion

  //#region Event handlers
  onLocateClick() {
    if (!this.currentState) return;
    this.panMap(this.currentState);
  }

  onCloseClearClick() {
    if (this.mode.isEdit) {
      this.currentState = this.originalState;
      this.mode.toggle();
      this.updateLocationMarker(this.currentState);
    } else {
      this.deleteLocation();
    }
  }

  onEditSaveClick() {
    if (this.mode.isView) {
      //Edit mode on
      this.mode.toggle();
    } else {
      //Update property
      if (this.currentState) {
        this.updateLocation(this.currentState);
      }
    }
  }

  onMapClick(e: L.LeafletMouseEvent) {
    if (!this.map || this.mode.isView) return;

    const { lat: latitude, lng: longitude } = e.latlng;
    this.currentState = { latitude, longitude };
    this.updateLocationMarker(this.currentState);
  }
  //#endregion

  //#region Service Call
  private updateLocation(data: Location) {
    // Disable action
    this.disableActions = true;

    this.propertyService.updatePropertyLocation(this.propertyId, data).subscribe({
      next: (location) => {
        const propertyLocation =
          location.latitude && location.longitude
            ? { latitude: location.latitude, longitude: location.longitude }
            : undefined;
        this.originalState = propertyLocation;
        this.currentState = propertyLocation;
        this.mode.state = 'view';
        this.disableActions = false;

        this.updateLocationMarker(this.currentState);
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

  private deleteLocation() {
    // Disable action
    this.disableActions = true;

    this.propertyService.updatePropertyLocation(this.propertyId, {}).subscribe({
      next: (location) => {
        const propertyLocation =
          location.latitude && location.longitude
            ? { latitude: location.latitude, longitude: location.longitude }
            : undefined;
        this.originalState = propertyLocation;
        this.currentState = propertyLocation;
        this.mode.state = 'view';
        this.disableActions = false;

        this.updateLocationMarker(this.currentState);
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
  //#endregion

  private updateLocationMarker(location: Location | undefined) {
    if (!this.map) return;

    this.locationMarker?.remove();
    this.locationMarker = undefined;

    if (location) {
      const { latitude: markerLat, longitude: markerLng } = location;
      this.locationMarker = L.marker([markerLat, markerLng]);
      this.locationMarker.addTo(this.map);
    }
  }

  private panMap(location: Location) {
    this.map?.panTo([location.latitude, location.longitude], { animate: true });
  }
}
