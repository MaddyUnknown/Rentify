export interface EnvironmentConfigService {
  get defaultPropertyLatLon(): { latitude: number; longitude: number };
  get apiBaseURL(): string;
  get thumbnailImagePath(): { propertyMediaProcessing: string; propertyNotFound: string; tenantNotFound: string };
}
