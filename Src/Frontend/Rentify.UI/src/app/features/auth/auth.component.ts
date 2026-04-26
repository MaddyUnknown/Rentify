import { Component, Inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonComponent } from '../../shared/components/button/button.component';
import {
  FileText,
  House,
  KeyRound,
  LogIn,
  LucideAngularModule,
  ShieldCheck,
  User,
  UserPlus,
  UsersRound,
} from 'lucide-angular';
import { RoutesConstants } from '../../core/constants/routes.constants';
import { UserService } from '../../core/services/abstractions/user.service';
import { USER_SERVICE_TOKEN } from '../../core/services/tokens/user.token';
import { ApiError } from '../../core/exceptions/api-error';
import { ROUTE_SERVICE_TOKEN } from '../../core/services/tokens/route.token';
import { RouteService } from '../../core/services/abstractions/route.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [ReactiveFormsModule, ButtonComponent, LucideAngularModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css',
})
export class AuthComponent {
  readonly ICONS = { FileText, House, KeyRound, LogIn, ShieldCheck, User, UserPlus, UsersRound };

  mode: 'login' | 'register' = 'login';
  isSubmitting = false;
  loginForm;
  registerForm;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    @Inject(USER_SERVICE_TOKEN) private userService: UserService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
  ) {
    this.loginForm = this.fb.group({
      email: this.fb.nonNullable.control('', [Validators.required, Validators.email]),
      password: this.fb.nonNullable.control('', [Validators.required, Validators.minLength(8)]),
      rememberMe: this.fb.nonNullable.control(false),
    });

    this.registerForm = this.fb.group({
      name: this.fb.nonNullable.control('', [Validators.required]),
      email: this.fb.nonNullable.control('', [Validators.required, Validators.email]),
      phoneNumber: this.fb.nonNullable.control('', [Validators.required]),
      password: this.fb.nonNullable.control('', [Validators.required, Validators.minLength(8)]),
      confirmPassword: this.fb.nonNullable.control('', [Validators.required]),
    });
  }

  setMode(mode: 'login' | 'register'): void {
    this.mode = mode;
  }

  isInvalid(control: AbstractControl | null): boolean {
    if (!control) return false;
    return control.invalid && (control.dirty || control.touched);
  }

  onLoginSubmit(): void {
    if (!this.loginForm.valid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const { email, password, rememberMe } = this.loginForm.getRawValue();

    this.userService.login(email, password, rememberMe).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(this.routeService.propeties());
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        this.isSubmitting = false;
      },
    });
  }

  onRegisterSubmit(): void {
    if (!this.registerForm.valid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    if (this.registerForm.controls.password.value !== this.registerForm.controls.confirmPassword.value) {
      this.registerForm.controls.confirmPassword.markAsTouched();
      this.registerForm.controls.confirmPassword.setErrors({ mismatch: true });
      return;
    }

    this.isSubmitting = true;

    this.userService.registerUser(this.registerForm.getRawValue()).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.registerForm.reset();
        console.log('User registered successfully'); //TO-DO: Add new changes
      },
      error: (err) => {
        if (err instanceof ApiError) {
          console.log('API Error', err.Errors);
        } else {
          console.error(err);
        }

        this.isSubmitting = false;
      },
    });
  }
}
