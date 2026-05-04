import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private readonly fb = new FormBuilder();

  isSubmitting = false;
  serverError = '';

  registerForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    role: ['Candidate', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    acceptTerms: [false, [Validators.requiredTrue]],
  });

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly toastService: ToastService,
  ) {}

  get fullName() {
    return this.registerForm.controls.fullName;
  }

  get role() {
    return this.registerForm.controls.role;
  }

  get email() {
    return this.registerForm.controls.email;
  }

  get password() {
    return this.registerForm.controls.password;
  }

  get acceptTerms() {
    return this.registerForm.controls.acceptTerms;
  }

  onSubmit(): void {
    this.serverError = '';

    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const { fullName, role, email, password } = this.registerForm.getRawValue();
    this.isSubmitting = true;

    this.authService
      .register({ fullName, role: role as 'Candidate' | 'Employer' | 'Admin', email, password })
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => {
          this.toastService.success('Account created successfully');
          this.router.navigateByUrl('/dashboard');
        },
        error: (error) => {
          this.serverError = error?.error?.message ?? 'Registration failed. Please try again.';
          this.toastService.error(this.serverError);
        },
      });
  }
}
