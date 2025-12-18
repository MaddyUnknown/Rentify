import { AfterViewInit, Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CircleX, Map, MapPin, MapPinOff, Save, SquarePen } from 'lucide-angular';
import * as L from 'leaflet';

import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { GeoLocationService } from '../../../../shared/services/geolocation.service';
import { Location, UpdateLocation } from '../../../../core/models/location.model';
import { EnvironmentConfigService } from '../../../../core/services/environment-config.service';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { UIEditState } from '../../../../shared/models/edit-ui-state.model';
import { EditMode } from '../../../../shared/models/edit-mode.model';
import { PropertyService } from '../../../../core/services/property.service';

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

  locationDetails: UIEditState<Location | undefined> = { data: undefined, mode: EditMode.from('view') };

  @Input({ required: true }) propertyId!: number;
  @Input({ alias: 'appPropertyLocation', required: false }) location?: Location;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private geolocationService: GeoLocationService,
    private propertyService: PropertyService,
    private envConfigService: EnvironmentConfigService,
  ) {}

  //#region Lifecycle hooks
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['location']) {
      const value: Location | undefined = changes['location'].currentValue ?? undefined;

      this.locationDetails = {
        data: this.locationDetails.mode.isView ? value : this.locationDetails.data,
        mode: this.locationDetails.mode,
        previousData: this.locationDetails.mode.isView ? undefined : value,
      };

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

    if (this.locationDetails.data) {
      this.updateLocationMarker(this.locationDetails.data);
      this.panMap(this.locationDetails.data);
      this.map.on('click', this.onMapClick.bind(this));
    } else {
      this.geolocationService.getCurrentLocation().subscribe({
        next: (location) => {
          this.panMap(location);
          this.map?.on('click', this.onMapClick.bind(this));
        },
        error: (err) => console.error(err),
      });
    }
  }
  //#endregion

  //#region Event handlers
  onLocateClick() {
    if (!this.locationDetails.data) return;
    this.panMap(this.locationDetails.data);
  }

  onCloseClearClick() {
    if (this.locationDetails.mode.isEdit) {
      this.locationDetails.data = this.locationDetails.previousData;
      this.locationDetails.previousData = undefined;
      this.locationDetails.mode.toggle();
      this.updateLocationMarker(this.locationDetails.data);
    } else {
      this.deleteLocation(this.propertyId);
    }
  }

  onEditSaveClick() {
    if (this.locationDetails.mode.isView) {
      //Edit mode on
      this.locationDetails.previousData = this.locationDetails.data;
      this.locationDetails.mode.toggle();
    } else {
      //Update property
      if (this.locationDetails.data) {
        this.updateLocation(this.locationDetails.data);
      }
    }
  }

  onMapClick(e: L.LeafletMouseEvent) {
    if (!this.map || this.locationDetails.mode.isView) return;

    const { lat: latitude, lng: longitude } = e.latlng;
    this.locationDetails.data = { latitude, longitude };
    this.updateLocationMarker(this.locationDetails.data);
  }
  //#endregion

  //#region Service Call
  private updateLocation(data: Location) {
    // Disable action
    this.locationDetails.isActionDisabled = true;

    // Save new unit details
    const updateLocation: UpdateLocation = {
      propertyId: this.propertyId,
      latitude: data.latitude,
      longitude: data.longitude,
    };

    this.propertyService.updatePropertyLocation(updateLocation).subscribe({
      next: (location) => {
        const updatedData: UIEditState<Location | undefined> = {
          data: { ...location },
          mode: EditMode.from('view'),
        };

        //Update received property
        this.locationDetails = updatedData;
        this.updateLocationMarker(location);
      },
      error: (err) => console.error(err),
    });
  }

  private deleteLocation(propertyId: number) {
    this.propertyService.deletePropertyLocation(propertyId).subscribe({
      next: (location) => {
        const updatedData: UIEditState<Location | undefined> = {
          data: undefined,
          mode: EditMode.from('view'),
        };

        //Update received property
        this.locationDetails = updatedData;
        this.updateLocationMarker(undefined);
      },
      error: (err) => console.error(err),
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
