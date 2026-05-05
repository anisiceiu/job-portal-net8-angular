import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { Header } from '../../layout/header/header';
import { Footer } from '../../layout/footer/footer';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/services/toast.service';

interface Company {
  companyId: number;
  companyName: string;
}

interface Category {
  categoryId: number;
  categoryName: string;
}

interface CreateJobRequest {
  companyId: number;
  postedByUserId: number;
  categoryId?: number | null;
  title: string;
  jobType: string;
  workMode?: string | null;
  location?: string | null;
  salaryMin?: number | null;
  salaryMax?: number | null;
  description: string;
  responsibilities?: string | null;
  requirements?: string | null;
  benefits?: string | null;
  deadline?: string | null;
  status: string;
}

@Component({
  selector: 'app-post-job',
  imports: [Header, Footer, ReactiveFormsModule],
  templateUrl: './post-job.html',
  styleUrl: './post-job.css',
})
export class PostJob {
  private readonly fb = new FormBuilder();
  private readonly apiUrl = environment.baseUrl;

  companies: Company[] = [];
  categories: Category[] = [];
  isLoadingCompanies = false;
  isLoadingCategories = false;
  isSubmitting = false;
  serverError = '';

  jobForm = this.fb.nonNullable.group({
    companyId: [0, [Validators.required, Validators.min(1)]],
    categoryId: [0, [Validators.required, Validators.min(1)]],
    title: ['', [Validators.required, Validators.maxLength(180)]],
    jobType: ['Full-time', [Validators.required, Validators.maxLength(50)]],
    workMode: ['Remote', [Validators.maxLength(50)]],
    location: ['', [Validators.maxLength(150)]],
    salaryMin: [0, [Validators.min(0)]],
    salaryMax: [0, [Validators.min(0)]],
    description: ['', [Validators.required]],
    responsibilities: [''],
    requirements: [''],
    benefits: [''],
    deadline: [''],
    status: ['Open', [Validators.required, Validators.maxLength(30)]],
  });

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly toastService: ToastService,
  ) {
    this.loadCompanies();
    this.loadCategories();
  }

  get title() {
    return this.jobForm.controls.title;
  }

  get jobType() {
    return this.jobForm.controls.jobType;
  }

  get companyId() {
    return this.jobForm.controls.companyId;
  }

  get categoryId() {
    return this.jobForm.controls.categoryId;
  }

  get workMode() {
    return this.jobForm.controls.workMode;
  }

  get location() {
    return this.jobForm.controls.location;
  }

  get salaryMin() {
    return this.jobForm.controls.salaryMin;
  }

  get salaryMax() {
    return this.jobForm.controls.salaryMax;
  }

  get description() {
    return this.jobForm.controls.description;
  }

  onSubmit(): void {
    this.serverError = '';

    if (this.jobForm.invalid) {
      this.jobForm.markAllAsTouched();
      return;
    }

    const postedByUserId = Number(this.authService.currentUser()?.id);
    if (!postedByUserId) {
      this.serverError = 'Could not identify the signed-in user. Please log in again.';
      this.toastService.error(this.serverError);
      return;
    }

    const formValue = this.jobForm.getRawValue();
    const request: CreateJobRequest = {
      companyId: formValue.companyId,
      categoryId:formValue.categoryId,
      postedByUserId,
      title: formValue.title,
      jobType: formValue.jobType,
      workMode: this.emptyToNull(formValue.workMode),
      location: this.emptyToNull(formValue.location),
      salaryMin: this.zeroToNull(formValue.salaryMin),
      salaryMax: this.zeroToNull(formValue.salaryMax),
      description: formValue.description,
      responsibilities: this.emptyToNull(formValue.responsibilities),
      requirements: this.emptyToNull(formValue.requirements),
      benefits: this.emptyToNull(formValue.benefits),
      deadline: this.emptyToNull(formValue.deadline),
      status: formValue.status,
    };
    this.isSubmitting = true;

    this.http
      .post(`${this.apiUrl}Jobs`, request)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => {
          this.toastService.success('Job published successfully');
          this.router.navigateByUrl('/jobs');
        },
        error: (error) => {
          this.serverError = error?.error?.message ?? 'Could not publish this job. Please try again.';
          this.toastService.error(this.serverError);
        },
      });
  }

  private emptyToNull(value: string): string | null {
    const trimmedValue = value.trim();
    return trimmedValue ? trimmedValue : null;
  }

  private zeroToNull(value: number): number | null {
    return value > 0 ? value : null;
  }

  private loadCompanies(): void {
    this.isLoadingCompanies = true;

    this.http
      .get<Company[]>(`${this.apiUrl}Companies`)
      .pipe(finalize(() => (this.isLoadingCompanies = false)))
      .subscribe({
        next: (companies) => {
          this.companies = companies;
        },
        error: () => {
          this.serverError = 'Could not load companies. Please try again later.';
          this.toastService.error(this.serverError);
        },
      });
  }

  private loadCategories(): void {
    this.isLoadingCategories = true;

    this.http
      .get<Category[]>(`${this.apiUrl}Categories`)
      .pipe(finalize(() => (this.isLoadingCategories = false)))
      .subscribe({
        next: (categories) => {
          this.categories = categories;
        },
        error: () => {
          this.serverError = 'Could not load categories. Please try again later.';
          this.toastService.error(this.serverError);
        },
      });
  }
}
