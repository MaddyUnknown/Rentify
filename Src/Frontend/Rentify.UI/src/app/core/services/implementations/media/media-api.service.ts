import { Observable } from 'rxjs';
import { data } from '../mock/data';
import { MediaService } from '../../abstractions/media.service';
import { MediaFile } from '../../../models/media-file/media-file.model';
import { HttpClient } from '@angular/common/http';
import { EnvironmentConfigJsonService } from '../environement-config/environment-config-json.service';
import { Inject, Injectable } from '@angular/core';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../tokens/environement-config.token';
import { ResponseWrapper } from '../../../models/response/response-wrapper.model';
import { processResponse } from '../../../utils/process-response.util';

@Injectable()
export class MediaApiService implements MediaService {
  constructor(
    private httpClient: HttpClient,
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private environmentConfigService: EnvironmentConfigJsonService,
  ) {}

  getMediaFileStatus(mediaFileIds: number[]): Observable<MediaFile[]> {
    return this.httpClient
      .post<ResponseWrapper<MediaFile[]>>(this.environmentConfigService.apiBaseURL + `media/polling`, mediaFileIds)
      .pipe(processResponse());
  }
}
