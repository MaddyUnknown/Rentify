import { Observable } from 'rxjs';
import { MediaService } from '../../abstractions/media.service';
import { MediaFile } from '../../../models/media-file/media-file.model';
import { HttpClient, HttpContext } from '@angular/common/http';
import { EnvironmentConfigJsonService } from '../environement-config/environment-config-json.service';
import { Inject, Injectable } from '@angular/core';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../tokens/environement-config.token';
import { ResponseWrapper } from '../../../models/response/response-wrapper.model';
import { unwrapReponse } from '../../../utils/unwrap-response.util';
import { AUTH_HEADER, SUBSCRIPTION_HEADER } from '../../tokens/http-context.token';

@Injectable()
export class MediaApiService implements MediaService {
  constructor(
    private httpClient: HttpClient,
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private environmentConfigService: EnvironmentConfigJsonService,
  ) {}

  getMediaFileStatus(mediaFileIds: number[]): Observable<MediaFile[]> {
    return this.httpClient
      .post<
        ResponseWrapper<MediaFile[]>
      >(this.environmentConfigService.apiBaseURL + `media/polling`, mediaFileIds, { context: new HttpContext().set(AUTH_HEADER, true).set(SUBSCRIPTION_HEADER, true) })
      .pipe(unwrapReponse());
  }
}
