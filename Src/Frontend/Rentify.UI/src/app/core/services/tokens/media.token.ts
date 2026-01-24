import { InjectionToken } from '@angular/core';
import { MediaService } from '../abstractions/media.service';

export const MEDIA_SERVICE_TOKEN = new InjectionToken<MediaService>('MEDIA_SERVICE_TOKEN');
