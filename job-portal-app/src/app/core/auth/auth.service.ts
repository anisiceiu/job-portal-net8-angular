import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, AuthUser, JwtPayload, LoginRequest, RegisterRequest } from './auth.models';

const TOKEN_KEY = 'job_portal_access_token';
const REFRESH_TOKEN_KEY = 'job_portal_refresh_token';
const CANDIDATE_PROFILE_ID_KEY = 'job_portal_candidate_profile_id';
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
const EMAIL_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress';
const CANDIDATE_PROFILE_ID_CLAIM = 'candidateProfileId';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.authApiUrl;
  private readonly tokenSignal = signal<string | null>(this.getStoredToken());
  private readonly candidateProfileIdSignal = signal<number>(this.getStoredCandidateProfileId());

  readonly token = this.tokenSignal.asReadonly();
  readonly isAuthenticated = computed(() => {
    const token = this.tokenSignal();
    return !!token && !this.isTokenExpired(token);
  });

  readonly currentUser = computed<AuthUser | null>(() => {
    const token = this.tokenSignal();

    if (!token || this.isTokenExpired(token)) {
      return null;
    }

    const payload = this.decodeToken(token);
    if (!payload) {
      return null;
    }

    const role = this.getFirstClaimValue(payload.role ?? payload[ROLE_CLAIM]);
    const candidateProfileId =
      this.candidateProfileIdSignal() ||
      Number(this.getClaimValue(payload[CANDIDATE_PROFILE_ID_CLAIM])) ||
      0;

    return {
      id: this.getClaimValue(payload.sub),
      email: this.getClaimValue(payload.email ?? payload[EMAIL_CLAIM]) ?? '',
      fullName: this.getClaimValue(payload['fullName'] ?? payload.name ?? payload.unique_name ?? payload[NAME_CLAIM]),
      role,
      candidateProfileId,
    };
  });

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, request)
      .pipe(tap((response) => this.storeSession(response)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/register`, request)
      .pipe(tap((response) => this.storeSession(response)));
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(CANDIDATE_PROFILE_ID_KEY);
    this.tokenSignal.set(null);
    this.candidateProfileIdSignal.set(0);
  }

  getAccessToken(): string | null {
    const token = this.tokenSignal();
    return token && !this.isTokenExpired(token) ? token : null;
  }

  setCandidateProfileId(candidateProfileId: number): void {
    const profileId = Number(candidateProfileId) || 0;

    if (profileId > 0) {
      localStorage.setItem(CANDIDATE_PROFILE_ID_KEY, profileId.toString());
    } else {
      localStorage.removeItem(CANDIDATE_PROFILE_ID_KEY);
    }

    this.candidateProfileIdSignal.set(profileId);
  }

  isTokenExpired(token: string): boolean {
    const payload = this.decodeToken(token);
    if (!payload?.exp) {
      return false;
    }

    return payload.exp * 1000 <= Date.now();
  }

  private storeSession(response: AuthResponse): void {
    const token = response.token ?? response.accessToken;
    this.setCandidateProfileId(response.candidateProfileId ?? 0);
    if (!token) {
      throw new Error('Authentication response did not include a JWT token.');
    }

    localStorage.setItem(TOKEN_KEY, token);

    if (response.refreshToken) {
      localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
    }

    this.tokenSignal.set(token);
  }

  private getStoredToken(): string | null {
    const token = localStorage.getItem(TOKEN_KEY);
    return token && !this.isTokenExpired(token) ? token : null;
  }

  private getStoredCandidateProfileId(): number {
    return Number(localStorage.getItem(CANDIDATE_PROFILE_ID_KEY)) || 0;
  }

  private decodeToken(token: string): JwtPayload | null {
    try {
      const payload = token.split('.')[1];
      if (!payload) {
        return null;
      }

      const normalizedPayload = this.padBase64(payload.replace(/-/g, '+').replace(/_/g, '/'));
      const decoded = atob(normalizedPayload);
      const json = decodeURIComponent(
        decoded
          .split('')
          .map((char) => `%${`00${char.charCodeAt(0).toString(16)}`.slice(-2)}`)
          .join(''),
      );

      return JSON.parse(json) as JwtPayload;
    } catch {
      return null;
    }
  }

  private getFirstClaimValue(value: unknown): string | undefined {
    if (Array.isArray(value)) {
      return this.getClaimValue(value[0]);
    }

    return this.getClaimValue(value);
  }

  private getClaimValue(value: unknown): string | undefined {
    return typeof value === 'string' && value ? value : undefined;
  }

  private padBase64(value: string): string {
    const paddingLength = (4 - (value.length % 4)) % 4;
    return `${value}${'='.repeat(paddingLength)}`;
  }
}
