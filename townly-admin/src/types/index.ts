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

// Add these to your existing types file

// Update AdminPropertyListDto to match API response
export interface AdminPropertyListDto {
  id: string; // Note: API returns 'id' not 'propertyId'
  name: string;
  description: string;
  location: string;
  propertyType: string;
  imageUrl: string;
  status: number; // 1 for pending
  rejectionReason: string | null;
  totalValue: number;
  totalUnits: number;
  availableUnits: number;
  pricePerUnit: number;
  pricePerUnitEth: number;
  annualYieldPercent: number;
  riskScore: number;
  demandScore: number | null;
  rentalIncomeHistory: number;
  documents: PropertyDocument[];
  hasPendingUpdateRequest: boolean;
  canEditFullProperty: boolean;
  canResubmit: boolean;
  canRequestUpdate: boolean;
  canDelete: boolean;
  createdAt?: string; // Add if API returns this
}

export interface PropertyDocument {
  title: string;
  fileName: string;
  documentUrl: string;
}

// Paginated Response Type
export interface PaginatedResponse<T> {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  items: T[];
}