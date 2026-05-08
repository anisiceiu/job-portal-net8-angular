import { Component, computed, inject, signal } from '@angular/core';
import { Header } from "../../layout/header/header";
import { Footer } from "../../layout/footer/footer";
import { JobService } from '../../core/services/job.service';
import { Job } from '../../core/services/job.service';
import { FormBuilder, ReactiveFormsModule, Validators, ɵInternalFormsSharedModule } from '@angular/forms';
import { environment } from '../../../environments/environment';
import { finalize } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ToastService } from '../../core/services/toast.service';
import { RouterLink } from "@angular/router";

interface Category {
  categoryId: number;
  categoryName: string;
}

@Component({
  selector: 'app-jobs',
  imports: [Header, Footer, ɵInternalFormsSharedModule, ReactiveFormsModule, RouterLink],
  templateUrl: './jobs.html',
  styleUrl: './jobs.css',
})
export class Jobs {
  private readonly apiUrl = environment.baseUrl;
  private readonly fb = new FormBuilder();
  jobService = inject(JobService);
  jobs = signal<Job[]>([]);
  loading = signal(false);
  currentPage = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);

  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));

  pages = computed(() => Array.from({ length: this.totalPages() }, (_, index) => index + 1));

  showingFrom = computed(() => {
    if (this.totalCount() === 0) {
      return 0;
    }

    return (this.currentPage() - 1) * this.pageSize() + 1;
  });

  showingTo = computed(() => Math.min(this.currentPage() * this.pageSize(), this.totalCount()));

  categories: Category[] = [];
  isLoadingCategories = false;
  serverError = '';

  jobSearchForm = this.fb.nonNullable.group({
    categoryId: [null],
    workMode: [null],
    jobTypes: this.fb.group({
      fullTime: false,
      partTime: false,
      contract: false,
      remote: false
    })
  });

  constructor( private readonly http: HttpClient,private readonly toastService: ToastService) {
    this.loadJobs();
    this.loadCategories();
  }

  loadJobs(page: number = this.currentPage()) {
    this.loading.set(true);

    // API call
    this.jobService.getJobsPaginated(page, this.pageSize()).subscribe({
      next: (res) => {
        this.jobs.set(res.items);
        this.currentPage.set(res.page);
        this.pageSize.set(res.pageSize);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.jobs.set([]);
        this.totalCount.set(0);
        this.loading.set(false);
      }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages() || page === this.currentPage()) {
      return;
    }

    this.loadJobs(page);
  }

  previousPage(): void {
    this.goToPage(this.currentPage() - 1);
  }

  nextPage(): void {
    this.goToPage(this.currentPage() + 1);
  }



  search(): void {
    const formValue = this.jobSearchForm.getRawValue();
    const jobTypesGroup = this.jobSearchForm.value.jobTypes;

    const selectedJobTypes: string[] = [];

    if (jobTypesGroup?.fullTime) {
      selectedJobTypes.push('Full-time');
    }

    if (jobTypesGroup?.partTime) {
      selectedJobTypes.push('Part-time');
    }

    if (jobTypesGroup?.contract) {
      selectedJobTypes.push('Contract');
    }

    if (jobTypesGroup?.remote) {
      selectedJobTypes.push('Remote');
    }

    this.jobService.searchJobs(
      selectedJobTypes.join(),
      formValue.categoryId,
      formValue.workMode,
      null
    ).subscribe(res => {

      this.jobs.set(res.items);
      this.totalCount.set(res.totalCount);

    })
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
