import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { Header } from '../../layout/header/header';
import { Footer } from '../../layout/footer/footer';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/auth/auth.service';
import { ToastService } from '../../core/services/toast.service';

interface CandidateUser {
  fullName?: string;
  email?: string;
}

interface CandidateProfile {
  candidateProfileId: number;
  userId: number;
  headline?: string | null;
  summary?: string | null;
  experienceYears?: number | null;
  currentSalary?: number | null;
  expectedSalary?: number | null;
  location?: string | null;
  portfolioUrl?: string | null;
  linkedInUrl?: string | null;
  gitHubUrl?: string | null;
  resumeUrl?: string | null;
  user?: CandidateUser | null;
}

interface CreateCandidateProfileRequest {
  userId: number;
  headline?: string | null;
  summary?: string | null;
  experienceYears?: number | null;
  currentSalary?: number | null;
  expectedSalary?: number | null;
  location?: string | null;
  portfolioUrl?: string | null;
  linkedInUrl?: string | null;
  gitHubUrl?: string | null;
  resumeUrl?: string | null;
}

type UpdateCandidateProfileRequest = Omit<CreateCandidateProfileRequest, 'userId'>;

@Component({
  selector: 'app-candidate-profile',
  imports: [Header, Footer, ReactiveFormsModule],
  templateUrl: './candidate-profile.html',
  styleUrl: './candidate-profile.css',
})
export class CandidateProfileComponent implements OnInit {
  private readonly fb = new FormBuilder();
  private readonly apiUrl = `${environment.baseUrl}CandidateProfiles`;

  profileId: number | null = null;
  isLoading = false;
  isSubmitting = false;
  serverError = '';

  profileForm = this.fb.nonNullable.group({
    headline: ['', [Validators.maxLength(200)]],
    summary: [''],
    experienceYears: [0, [Validators.min(0), Validators.max(99.9)]],
    currentSalary: [0, [Validators.min(0)]],
    expectedSalary: [0, [Validators.min(0)]],
    location: ['', [Validators.maxLength(150)]],
    portfolioUrl: ['', [Validators.maxLength(255)]],
    linkedInUrl: ['', [Validators.maxLength(255)]],
    gitHubUrl: ['', [Validators.maxLength(255)]],
    resumeUrl: ['', [Validators.maxLength(500)]],
  });

  constructor(
    private readonly http: HttpClient,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly authService: AuthService,
    private readonly toastService: ToastService,
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (id) {
      this.profileId = id;
      this.loadProfile(id);
    }
  }

  get isEditMode(): boolean {
    return this.profileId !== null;
  }

  get headline() {
    return this.profileForm.controls.headline;
  }

  get experienceYears() {
    return this.profileForm.controls.experienceYears;
  }

  get currentSalary() {
    return this.profileForm.controls.currentSalary;
  }

  get expectedSalary() {
    return this.profileForm.controls.expectedSalary;
  }

  get location() {
    return this.profileForm.controls.location;
  }

  get portfolioUrl() {
    return this.profileForm.controls.portfolioUrl;
  }

  get linkedInUrl() {
    return this.profileForm.controls.linkedInUrl;
  }

  get gitHubUrl() {
    return this.profileForm.controls.gitHubUrl;
  }

  get resumeUrl() {
    return this.profileForm.controls.resumeUrl;
  }

  onSubmit(): void {
    this.serverError = '';

    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const request = this.buildRequest();
    const currentUserId = this.profileId ? null : this.getCurrentUserId();

    if (!this.profileId && !currentUserId) {
      this.serverError = 'Could not identify the signed-in user. Please log in again.';
      this.toastService.error(this.serverError);
      return;
    }

    if (this.profileId) {
      this.updateProfile(this.profileId, request);
      return;
    }

    this.createProfile({
      userId: currentUserId || 0,
      ...request,
    });
  }

  private createProfile(request: CreateCandidateProfileRequest): void {
    this.isSubmitting = true;

    this.http
      .post<CandidateProfile>(this.apiUrl, request)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: (profile) => {
          this.authService.setCandidateProfileId(profile.candidateProfileId);
          this.toastService.success('Candidate profile created successfully');
          this.router.navigateByUrl('/candidates');
        },
        error: (error) => this.handleSaveError(error),
      });
  }

  private updateProfile(profileId: number, request: UpdateCandidateProfileRequest): void {
    this.isSubmitting = true;

    this.http
      .put<void>(`${this.apiUrl}/${profileId}`, request)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => {
          this.toastService.success('Candidate profile updated successfully');
          this.router.navigateByUrl('/candidates');
        },
        error: (error) => this.handleSaveError(error),
      });
  }

  private loadProfile(id: number): void {
    this.isLoading = true;

    this.http
      .get<CandidateProfile>(`${this.apiUrl}/${id}`)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (profile) => {
          this.profileForm.patchValue({
            headline: profile.headline ?? '',
            summary: profile.summary ?? '',
            experienceYears: profile.experienceYears ?? 0,
            currentSalary: profile.currentSalary ?? 0,
            expectedSalary: profile.expectedSalary ?? 0,
            location: profile.location ?? '',
            portfolioUrl: profile.portfolioUrl ?? '',
            linkedInUrl: profile.linkedInUrl ?? '',
            gitHubUrl: profile.gitHubUrl ?? '',
            resumeUrl: profile.resumeUrl ?? '',
          });
        },
        error: () => {
          this.serverError = 'Could not load this candidate profile.';
          this.toastService.error(this.serverError);
        },
      });
  }

  private buildRequest(): UpdateCandidateProfileRequest {
    const value = this.profileForm.getRawValue();

    return {
      headline: this.emptyToNull(value.headline),
      summary: this.emptyToNull(value.summary),
      experienceYears: this.zeroToNull(value.experienceYears),
      currentSalary: this.zeroToNull(value.currentSalary),
      expectedSalary: this.zeroToNull(value.expectedSalary),
      location: this.emptyToNull(value.location),
      portfolioUrl: this.emptyToNull(value.portfolioUrl),
      linkedInUrl: this.emptyToNull(value.linkedInUrl),
      gitHubUrl: this.emptyToNull(value.gitHubUrl),
      resumeUrl: this.emptyToNull(value.resumeUrl),
    };
  }

  private getCurrentUserId(): number | null {
    const userId = Number(this.authService.currentUser()?.id);
    return userId || null;
  }

  private handleSaveError(error: any): void {
    this.serverError = error?.error?.message || 'Could not save candidate profile. Please try again.';
    this.toastService.error(this.serverError);
  }

  private emptyToNull(value: string): string | null {
    const trimmedValue = value.trim();
    return trimmedValue ? trimmedValue : null;
  }

  private zeroToNull(value: number): number | null {
    return value > 0 ? value : null;
  }
}
