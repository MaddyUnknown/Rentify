export interface EnvironmentConfigService {
  get defaultPropertyLatLon(): { latitude: number; longitude: number };
  get apiBaseURL(): string;
}
