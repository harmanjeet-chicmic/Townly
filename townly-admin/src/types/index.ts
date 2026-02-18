export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  expiresAt: string;
}

export interface AdminKycListDto {
  kycId: string;
  userId: string;
  fullName: string;
  status: string;
  createdAt: string;
}

export interface AdminPropertyListDto {
  propertyId: string;
  name: string;
  location: string;
  status: string;
  createdAt: string;
}

export interface PendingPropertyUpdateDto {
  updateRequestId: string;
  propertyId: string;
  requestedByUserId: string;
  name: string;
  location: string;
  propertyType: string;
  imageUrl?: string;
  requestedAt: string;
}

export interface AdminTokenRequestListDto {
  requestId: string;
  userId: string;
  requestedAmount: number;
  status: string;
  createdAt: string;
}

export interface RejectKycRequest {
  reason: string;
}

export interface RejectPropertyRequest {
  reason: string;
}

export interface ReviewTokenRequestCommand {
  requestId: string;
  adminId: string;
  approve: boolean;
  rejectionReason?: string;
}