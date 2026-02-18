import React, { useEffect, useState } from 'react';
import api from '../services/api';
import Table from '../components/Table';
import RejectModal from '../components/RejectModal';
import LoadingSpinner from '../components/LoadingSpinner';
import { AdminTokenRequestListDto } from '../types';

const TokenRequestsPending: React.FC = () => {
  const [requests, setRequests] = useState<AdminTokenRequestListDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedRequest, setSelectedRequest] = useState<AdminTokenRequestListDto | null>(null);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [showApproveModal, setShowApproveModal] = useState(false);

  const columns = [
    { key: 'userId', header: 'User ID' },
    { 
      key: 'requestedAmount', 
      header: 'Amount',
      render: (value: number) => `${value} ETH`
    },
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
      header: 'Requested',
      render: (value: string) => new Date(value).toLocaleDateString()
    },
  ];

  const fetchPendingRequests = async () => {
    setLoading(true);
    try {
      const response = await api.get('/api/admin/tokens/requests');
      console.log('Token Requests Response:', response.data);
      
      if (Array.isArray(response.data)) {
        setRequests(response.data);
      } else if (response.data && Array.isArray(response.data.data)) {
        setRequests(response.data.data);
      } else if (response.data && typeof response.data === 'object') {
        setRequests([response.data]);
      } else {
        setRequests([]);
      }
    } catch (error) {
      console.error('Failed to fetch token requests', error);
      setRequests([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPendingRequests();
  }, []);

  const handleApprove = async (id: string) => {
    const request = requests.find(r => r.requestId === id);
    if (!request) return;
    setSelectedRequest(request);
    setShowApproveModal(true);
  };

  const confirmApprove = async () => {
    if (!selectedRequest) return;
    
    // Try different formats based on what your API expects
    const command = {
      approve: true,
      // If API expects requestId in body as well
      requestId: selectedRequest.requestId,
      // If API expects adminId
      // adminId: "8c2b8762-5cfe-4739-8b27-237240992419",
      // Make sure rejectionReason is null for approve
      rejectionReason: null
    };

    try {
      const response = await api.post(
        `/api/admin/tokens/requests/${selectedRequest.requestId}/review`, 
        command
      );
      console.log('Approve response:', response);
      fetchPendingRequests();
      setShowApproveModal(false);
      setSelectedRequest(null);
    } catch (error: any) {
      console.error('Failed to approve request', error.response?.data || error);
      alert('Failed to approve. Error: ' + JSON.stringify(error.response?.data));
    }
  };

  const handleReject = (id: string) => {
    const request = requests.find(r => r.requestId === id);
    if (request) {
      setSelectedRequest(request);
      setShowRejectModal(true);
    }
  };

  const confirmReject = async (reason: string) => {
    if (!selectedRequest) return;
    
    const command = {
      approve: false,
      // If API expects requestId in body as well
      requestId: selectedRequest.requestId,
      // If API expects adminId
      // adminId: "8c2b8762-5cfe-4739-8b27-237240992419",
      rejectionReason: reason
    };

    try {
      const response = await api.post(
        `/api/admin/tokens/requests/${selectedRequest.requestId}/review`, 
        command
      );
      console.log('Reject response:', response);
      fetchPendingRequests();
      setShowRejectModal(false);
      setSelectedRequest(null);
    } catch (error: any) {
      console.error('Failed to reject request', error.response?.data || error);
      alert('Failed to reject. Error: ' + JSON.stringify(error.response?.data));
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Token Requests</h1>
          <p>Review token purchase requests</p>
        </div>
      </div>

      <Table
        columns={columns}
        data={requests}
        onApprove={handleApprove}
        onReject={handleReject}
      />

      {/* Approve Confirmation Modal */}
      {showApproveModal && selectedRequest && (
        <div className="modal-overlay">
          <div className="modal">
            <h3>Approve Token Request</h3>
            <p>Are you sure you want to approve token request for {selectedRequest.requestedAmount} ETH?</p>
            <div className="modal-actions">
              <button 
                onClick={() => {
                  setShowApproveModal(false);
                  setSelectedRequest(null);
                }} 
                className="btn btn-outline"
              >
                Cancel
              </button>
              <button onClick={confirmApprove} className="btn btn-success">
                Approve
              </button>
            </div>
          </div>
        </div>
      )}

      <RejectModal
        isOpen={showRejectModal}
        onClose={() => {
          setShowRejectModal(false);
          setSelectedRequest(null);
        }}
        onConfirm={confirmReject}
        title="Reject Token Request"
        message="Please provide a reason for rejecting this token request:"
      />
    </div>
  );
};

export default TokenRequestsPending;