import { Component, ElementRef, OnInit, ViewChild, signal } from '@angular/core';
import { Header } from "../../layout/header/header";
import { Footer } from "../../layout/footer/footer";
import { ActivatedRoute } from '@angular/router';
import { JobService } from '../../core/services/job.service';
import { Job } from '../../core/services/job.service';
import { CommonModule, DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { finalize } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ToastService } from '../../core/services/toast.service';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-job-details',
  imports: [Header, Footer, DatePipe, CommonModule, ReactiveFormsModule],
  templateUrl: './job-details.html',
  styleUrl: './job-details.css',
})
export class JobDetails implements OnInit {
  @ViewChild('applySection') private applySection?: ElementRef<HTMLElement>;
  @ViewChild('resumeInput') private resumeInput?: ElementRef<HTMLInputElement>;

  private readonly fb = new FormBuilder();
  private readonly apiUrl = environment.baseUrl;

  job = signal<Job | null>(null);
  loading = signal<boolean>(true);
  submitting = signal<boolean>(false);
  resumeFile = signal<File | null>(null);
  error: string | null = null;
  applicationError = '';
  applicationSuccess = '';

  applicationForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(150)]],
    portfolioUrl: ['', [Validators.maxLength(255)]],
    coverLetter: ['', [Validators.maxLength(4000)]],
    candidateProfileId:[0]
  });

  constructor(
    private route: ActivatedRoute,
    private jobService: JobService,
    private http: HttpClient,
    private toastService: ToastService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.prefillApplicationForm();

    const jobId = this.route.snapshot.paramMap.get('id');
    if (jobId) {
      this.jobService.getJobById(+jobId).subscribe({
        next: (job) => {
          this.job.set(job);
          this.loading.set(false);
        },
        error: (err) => {
          this.error = 'Failed to load job details';
          this.loading.set(false);
          console.error('Error fetching job:', err);
        }
      });
    } else {
      this.error = 'Job ID not provided';
      this.loading.set(false);
    }
  }

  get fullName() {
    return this.applicationForm.controls.fullName;
  }

  get email() {
    return this.applicationForm.controls.email;
  }

  get portfolioUrl() {
    return this.applicationForm.controls.portfolioUrl;
  }

  get coverLetter() {
    return this.applicationForm.controls.coverLetter;
  }

  scrollToApply(): void {
    this.applySection?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }

  onResumeSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    this.resumeFile.set(file);
  }

  submitApplication(): void {
    this.applicationError = '';
    this.applicationSuccess = '';

    if (this.applicationForm.invalid) {
      this.applicationForm.markAllAsTouched();
      return;
    }

    const currentJob = this.job();
    if (!currentJob) {
      this.applicationError = 'Job details are not available yet.';
      this.toastService.error(this.applicationError);
      return;
    }

    const formValue = this.applicationForm.getRawValue();
    const request = new FormData();
    request.append('jobId', currentJob.jobId.toString());
    request.append('fullName', formValue.fullName.trim());
    request.append('email', formValue.email.trim());
    request.append('candidateProfileId', formValue.candidateProfileId.toString());

    const portfolioUrl = this.emptyToNull(formValue.portfolioUrl);
    const coverLetter = this.emptyToNull(formValue.coverLetter);

    if (portfolioUrl) {
      request.append('portfolioUrl', portfolioUrl);
    }

    if (coverLetter) {
      request.append('coverLetter', coverLetter);
    }

    const resumeFile = this.resumeFile();
    if (resumeFile) {
      request.append('resumeFile', resumeFile, resumeFile.name);
    }

    this.submitting.set(true);

    this.http
      .post(`${this.apiUrl}jobs/Applications`, request)
      .pipe(finalize(() => this.submitting.set(false)))
      .subscribe({
        next: () => {
          this.applicationSuccess = 'Application submitted successfully.';
          this.toastService.success(this.applicationSuccess);
          this.applicationForm.reset({
            fullName: formValue.fullName,
            email: formValue.email,
            portfolioUrl: '',
            coverLetter: '',
          });
          this.resumeFile.set(null);
          if (this.resumeInput) {
            this.resumeInput.nativeElement.value = '';
          }
        },
        error: (error) => {
          this.applicationError =
            error?.error?.message ?? 'Could not submit your application. Please try again.';
          this.toastService.error(this.applicationError);
        },
      });
  }

  private prefillApplicationForm(): void {
    const currentUser = this.authService.currentUser();

    this.applicationForm.patchValue({
      fullName: currentUser?.fullName ?? '',
      email: currentUser?.email ?? '',
      candidateProfileId: currentUser?.candidateProfileId ?? 0,
    });
  }

  private emptyToNull(value: string): string | null {
    const trimmedValue = value.trim();
    return trimmedValue ? trimmedValue : null;
  }
}
