import React, { useEffect, useState } from 'react';
import api from '../services/api';
import Table from '../components/Table';
import RejectModal from '../components/RejectModal';
import LoadingSpinner from '../components/LoadingSpinner';
import { AdminKycListDto } from '../types';

const KycPending: React.FC = () => {
  const [kycList, setKycList] = useState<AdminKycListDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedKycId, setSelectedKycId] = useState<string | null>(null);
  const [showRejectModal, setShowRejectModal] = useState(false);

  const columns = [
    { key: 'fullName', header: 'Full Name' },
    { key: 'userId', header: 'User ID' },
    { 
      key: 'status', 
      header: 'Status',
      render: (value: any) => {
        const statusStr = String(value).toLowerCase();
        return (
          <span className={`badge ${statusStr}`}>{statusStr}</span>
        );
      }
    },
    { 
      key: 'createdAt', 
      header: 'Submitted',
      render: (value: string) => new Date(value).toLocaleDateString()
    },
  ];

  const fetchPendingKyc = async () => {
    setLoading(true);
    try {
      const response = await api.get('/api/admin/kyc/pending');
      console.log('KYC Response:', response.data);
      
      if (Array.isArray(response.data)) {
        setKycList(response.data);
      } else if (response.data && Array.isArray(response.data.data)) {
        setKycList(response.data.data);
      } else if (response.data && typeof response.data === 'object') {
        setKycList([response.data]);
      } else {
        setKycList([]);
      }
    } catch (error) {
      console.error('Failed to fetch KYC', error);
      setKycList([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPendingKyc();
  }, []);

  const handleApprove = async (id: string) => {
    try {
      await api.post(`/api/admin/kyc/${id}/approve`);
      fetchPendingKyc();
    } catch (error: any) {
      console.error('Failed to approve KYC', error.response?.data || error);
      alert('Failed to approve: ' + JSON.stringify(error.response?.data));
    }
  };

  const handleReject = async (reason: string) => {
    if (!selectedKycId) return;
    try {
      await api.post(`/api/admin/kyc/${selectedKycId}/reject`, { reason });
      fetchPendingKyc();
      setShowRejectModal(false);
      setSelectedKycId(null);
    } catch (error: any) {
      console.error('Failed to reject KYC', error.response?.data || error);
      alert('Failed to reject: ' + JSON.stringify(error.response?.data));
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>KYC Reviews</h1>
          <p>Review and verify user KYC submissions</p>
        </div>
      </div>

      <Table
        columns={columns}
        data={kycList}
        onApprove={handleApprove}
        onReject={(id) => {
          setSelectedKycId(id);
          setShowRejectModal(true);
        }}
      />

      <RejectModal
        isOpen={showRejectModal}
        onClose={() => {
          setShowRejectModal(false);
          setSelectedKycId(null);
        }}
        onConfirm={handleReject}
        title="Reject KYC"
        message="Please provide a reason for rejecting this KYC submission:"
      />
    </div>
  );
};

export default KycPending;