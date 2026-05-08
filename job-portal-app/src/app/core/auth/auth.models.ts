export type UserRole = 'Candidate' | 'Employer' | 'Admin';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  role: UserRole;
}

export interface AuthResponse {
  token?: string;
  refreshToken?: string;
  accessToken?: string;
  fullName?: string;
  email: string;
  role?: UserRole | string;
  candidateProfileId?: number;
}

export interface AuthUser {
  id?: string;
  fullName?: string;
  email: string;
  role?: UserRole | string;
  candidateProfileId: number;
}

export interface JwtPayload {
  sub?: string;
  email?: string;
  unique_name?: string;
  name?: string;
  role?: string | string[];
  exp?: number;
  [claim: string]: unknown;
}
