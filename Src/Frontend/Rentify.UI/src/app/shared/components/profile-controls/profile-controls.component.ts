import { Component, EventEmitter, Inject, Input, Output } from '@angular/core';
import { LucideAngularModule, ChevronDown, LogOut, PencilLine } from 'lucide-angular';
import { UserProfile } from '../../../core/models/user/user-profile.model';
import { ButtonComponent } from '../button/button.component';
import { USER_SERVICE_TOKEN } from '../../../core/services/tokens/user.token';
import { UserService } from '../../../core/services/abstractions/user.service';

@Component({
  selector: 'app-profile-controls',
  standalone: true,
  imports: [LucideAngularModule, ButtonComponent],
  templateUrl: './profile-controls.component.html',
  styleUrl: './profile-controls.component.css',
})
export class ProfileControlsComponent {
  readonly ICONS = { ChevronDown, LogOut, PencilLine };
  readonly panelId = `app-profile-controls-panel-${Math.random().toString(36).slice(2, 11)}`;

  @Input({ required: false }) expanded = false;

  constructor(@Inject(USER_SERVICE_TOKEN) public userService: UserService) {}

  get userInitial(): string {
    const firstName = this.userService?.userData?.name?.trim().split(/\s+/)[0];
    return firstName?.charAt(0).toUpperCase() || 'U';
  }

  onToggle(): void {
    this.expanded = !this.expanded;
  }

  onEdit(): void {}

  onLogout(): void {
    this.userService.logout().subscribe();
  }
}
