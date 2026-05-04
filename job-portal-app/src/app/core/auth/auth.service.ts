import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, AuthUser, JwtPayload, LoginRequest, RegisterRequest } from './auth.models';

const TOKEN_KEY = 'job_portal_access_token';
const REFRESH_TOKEN_KEY = 'job_portal_refresh_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.authApiUrl;
  private readonly tokenSignal = signal<string | null>(this.getStoredToken());

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

    const role = Array.isArray(payload.role) ? payload.role[0] : payload.role;

    return {
      id: payload.sub,
      email: payload.email ?? '',
      fullName: payload.name ?? payload.unique_name,
      role,
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
    this.tokenSignal.set(null);
  }

  getAccessToken(): string | null {
    const token = this.tokenSignal();
    return token && !this.isTokenExpired(token) ? token : null;
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

  private decodeToken(token: string): JwtPayload | null {
    try {
      const payload = token.split('.')[1];
      const normalizedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
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
}
