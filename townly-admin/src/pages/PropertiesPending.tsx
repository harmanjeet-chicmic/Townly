import React, { useEffect, useState } from 'react';
import api from '../services/api';
import Table from '../components/Table';
import RejectModal from '../components/RejectModal';
import LoadingSpinner from '../components/LoadingSpinner';
import { AdminPropertyListDto } from '../types';

const PropertiesPending: React.FC = () => {
  const [properties, setProperties] = useState<AdminPropertyListDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedPropertyId, setSelectedPropertyId] = useState<string | null>(null);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [showModifyModal, setShowModifyModal] = useState(false);
  const [actionType, setActionType] = useState<'reject' | 'modify'>('reject');

  const columns = [
    { key: 'name', header: 'Property Name' },
    { key: 'location', header: 'Location' },
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

  const fetchPendingProperties = async () => {
    setLoading(true);
    try {
      const response = await api.get('/api/admin/properties/pending');
      console.log('Properties Response:', response.data);
      
      if (Array.isArray(response.data)) {
        setProperties(response.data);
      } else if (response.data && Array.isArray(response.data.data)) {
        setProperties(response.data.data);
      } else if (response.data && typeof response.data === 'object') {
        setProperties([response.data]);
      } else {
        setProperties([]);
      }
    } catch (error) {
      console.error('Failed to fetch properties', error);
      setProperties([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPendingProperties();
  }, []);

  const handleApprove = async (id: string) => {
    try {
      await api.post(`/api/admin/properties/${id}/approve`);
      fetchPendingProperties();
    } catch (error: any) {
      console.error('Failed to approve property', error.response?.data || error);
      alert('Failed to approve: ' + JSON.stringify(error.response?.data));
    }
  };

  const handleReject = async (reason: string) => {
    if (!selectedPropertyId) return;
    try {
      await api.post(`/api/admin/properties/${selectedPropertyId}/reject`, { reason });
      fetchPendingProperties();
      setShowRejectModal(false);
      setSelectedPropertyId(null);
    } catch (error: any) {
      console.error('Failed to reject property', error.response?.data || error);
      alert('Failed to reject: ' + JSON.stringify(error.response?.data));
    }
  };

  const handleModify = async (reason: string) => {
    if (!selectedPropertyId) return;
    try {
      await api.post(`/api/admin/properties/${selectedPropertyId}/modify`, { reason });
      fetchPendingProperties();
      setShowModifyModal(false);
      setSelectedPropertyId(null);
    } catch (error: any) {
      console.error('Failed to send modification request', error.response?.data || error);
      alert('Failed to send modification request: ' + JSON.stringify(error.response?.data));
    }
  };

  const openRejectModal = (id: string) => {
    setSelectedPropertyId(id);
    setActionType('reject');
    setShowRejectModal(true);
  };

  const openModifyModal = (id: string) => {
    setSelectedPropertyId(id);
    setActionType('modify');
    setShowModifyModal(true);
  };

  const handleModalConfirm = (reason: string) => {
    if (actionType === 'reject') {
      handleReject(reason);
    } else {
      handleModify(reason);
    }
  };

  const closeModals = () => {
    setShowRejectModal(false);
    setShowModifyModal(false);
    setSelectedPropertyId(null);
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Pending Properties</h1>
          <p>Review and approve new property listings</p>
        </div>
      </div>

      <Table
        columns={columns}
        data={properties}
        onApprove={handleApprove}
        onReject={openRejectModal}
        onModify={openModifyModal}
        showModifyButton={true}
        showActions={true}
      />

      {/* Reject Modal */}
      <RejectModal
        isOpen={showRejectModal}
        onClose={closeModals}
        onConfirm={handleModalConfirm}
        title="Reject Property"
        message="Please provide a reason for rejecting this property listing:"
      />

      {/* Modify Modal */}
      <RejectModal
        isOpen={showModifyModal}
        onClose={closeModals}
        onConfirm={handleModalConfirm}
        title="Request Modifications"
        message="Please provide details about what needs to be modified in this property listing:"
      />
    </div>
  );
};

export default PropertiesPending;