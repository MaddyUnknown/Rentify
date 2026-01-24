import { Route } from '@angular/router';
import { RouteService } from '../../abstractions/route.service';
import { RoutesConstants } from '../../../constants/routes.constants';

export class RouteImplementationService implements RouteService {
  propeties(): any[] {
    return [`/${RoutesConstants.Properties}`];
  }
  property(id: number): any[] {
    return [`/${RoutesConstants.Property}`, id];
  }
}
