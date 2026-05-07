import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Company {
  companyId: number;
  companyName: string;
  logoUrl?: string;
}

export interface Job {
  jobId: number;
  companyId: number;
  postedByUserId: number;
  categoryId: number;

  title: string;
  jobType: string;
  workMode: string;
  location: string;

  salaryMin?: number;
  salaryMax?: number;

  description: string;
  responsibilities?: string;
  requirements?: string;
  benefits?: string;

  deadline?: string;
  status?: string;

  createdAt?: string;
  updatedAt?: string;

  company?: Company;
}

export interface CreateJobDto {
  companyId: number;
  postedByUserId: number;
  categoryId: number;

  title: string;
  jobType: string;
  workMode: string;
  location: string;

  salaryMin?: number;
  salaryMax?: number;

  description: string;
  responsibilities?: string;
  requirements?: string;
  benefits?: string;

  deadline?: string;
  status?: string;
}

export interface UpdateJobDto {
  companyId: number;
  postedByUserId: number;
  categoryId: number;

  title: string;
  jobType: string;
  workMode: string;
  location: string;

  salaryMin?: number;
  salaryMax?: number;

  description: string;
  responsibilities?: string;
  requirements?: string;
  benefits?: string;

  deadline?: string;
  status?: string;
}


export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class JobService {

  private http = inject(HttpClient);

  // change this to your API url
  //private apiUrl = 'https://localhost:7168/api/jobs';
  private readonly apiUrl = environment.baseUrl+'jobs';

  /**
   * Get all jobs
   */
  getJobs(): Observable<Job[]> {
    return this.http.get<Job[]>(this.apiUrl);
  }

  /**
   * Get single job by id
   */
  getJobById(id: number): Observable<Job> {
    return this.http.get<Job>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get paginated jobs
   */
  getJobsPaginated(
    page: number = 1,
    pageSize: number = 10
  ): Observable<PaginatedResult<Job>> {

    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    return this.http.get<PaginatedResult<Job>>(
      `${this.apiUrl}/paginated`,
      { params }
    );
  }

  /**
   * Create new job
   */
  createJob(data: CreateJobDto): Observable<Job> {
    return this.http.post<Job>(this.apiUrl, data);
  }

  /**
   * Update existing job
   */
  updateJob(id: number, data: UpdateJobDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, data);
  }

  /**
   * Delete job
   */
  deleteJob(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Search jobs
   */
  searchJobs(
  type?: string | null,
  categoryId?: number | null,
  workmode?: string | null,
  experience?: string | null
): Observable<PaginatedResult<Job>> {

  let params = new HttpParams();

  if (type) {
    params = params.set('type', type);
  }

  if (categoryId !== undefined && categoryId !== null) {
    params = params.set('categoryId', categoryId);
  }

  if (workmode) {
    params = params.set('workmode', workmode);
  }

  if (experience) {
    params = params.set('experience', experience);
  }

  return this.http.get<PaginatedResult<Job>>(
    `${this.apiUrl}/search`,
    { params }
  );
}
}