import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
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

  loginForm;
  registerForm;

  constructor(private fb: FormBuilder) {
    this.loginForm = this.fb.group({
      email: this.fb.nonNullable.control('', [Validators.required, Validators.email]),
      password: this.fb.nonNullable.control('', [Validators.required, Validators.minLength(8)]),
      rememberMe: this.fb.nonNullable.control(false),
    });

    this.registerForm = this.fb.group({
      fullName: this.fb.nonNullable.control('', [Validators.required]),
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

    // Placeholder: replace with auth service call
    console.log('Login payload', this.loginForm.getRawValue());
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

    // Placeholder: replace with auth service call
    console.log('Register payload', this.registerForm.getRawValue());
  }
}
