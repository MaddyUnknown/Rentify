import { AfterViewInit, Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Map } from 'lucide-angular';
import * as L from 'leaflet';

import { PanelComponent } from '../../../../shared/components/panel/panel.component';
import { GeoLocationService } from '../../../../core/services/geolocation.service';
import { Location } from '../../../../core/models/location.model';
import { EnvironmentConfigService } from '../../../../core/services/environment-config.service';
import { SkeletonLoaderComponent } from '../../../../shared/components/skeleton-loader/skeleton-loader';

@Component({
  selector: 'section[appPropertyLocation]',
  templateUrl: './property-location.component.html',
  styleUrls: ['./property-location.component.css'],
  imports: [PanelComponent, SkeletonLoaderComponent],
  standalone: true,
})
export class PropertyLocationComponent implements OnChanges, AfterViewInit {
  readonly ICONS = { Map };
  readonly MAP_SELECTOR = 'map';
  readonly LEAFLY_PROVIDER_URL = 'https://{s}.tile.openstreetmap.fr/hot/{z}/{x}/{y}.png';
  readonly LEAFLY_PROVIDER_ATTRIBUTION =
    '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Tiles style by <a href="https://www.hotosm.org/" target="_blank">Humanitarian OpenStreetMap Team</a> hosted by <a href="https://openstreetmap.fr/" target="_blank">OpenStreetMap France</a>';

  private map?: L.Map;

  @Input({ alias: 'appPropertyLocation', required: false }) location?: Location;
  @Input({ required: false }) loading: boolean = false;

  constructor(
    private geolocationService: GeoLocationService,
    private envConfigService: EnvironmentConfigService,
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.map) {
      return;
    }

    if (changes['location']) {
      this.addLocationMarker(changes['location'].currentValue);
      this.panMap(changes['location'].currentValue);
    }
  }

  ngAfterViewInit(): void {
    const { latitude, longitude } = this.envConfigService.defaultPropertyLatLon;

    this.map = L.map(this.MAP_SELECTOR).setView([latitude, longitude], 18);

    L.tileLayer(this.LEAFLY_PROVIDER_URL, {
      attribution: this.LEAFLY_PROVIDER_ATTRIBUTION,
      maxZoom: 19,
    }).addTo(this.map);

    if (this.location) {
      this.addLocationMarker(this.location);
      this.panMap(this.location);
    } else {
      this.geolocationService.getCurrentLocation().subscribe({
        next: (location) => {
          this.panMap(location);
        },
        error: (err) => console.error(err),
      });
    }
  }

  private addLocationMarker(location: Location) {
    if (!this.map) return;

    const { latitude: marketLat, longitude: marketLng } = location;
    L.marker([marketLat, marketLng]).addTo(this.map);
  }

  private panMap(location: Location) {
    this.map?.panTo([location.latitude, location.longitude]);
  }
}
