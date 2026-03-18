import React, { useEffect, useState } from 'react';
import api from '../services/api';
import Table from '../components/Table';
import RejectModal from '../components/RejectModal';
import LoadingSpinner from '../components/LoadingSpinner';
import { AdminPropertyListDto, PaginatedResponse, RejectPropertyRequest } from '../types';

const PropertiesPending: React.FC = () => {
  const [properties, setProperties] = useState<AdminPropertyListDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedProperty, setSelectedProperty] = useState<AdminPropertyListDto | null>(null);
  const [selectedPropertyId, setSelectedPropertyId] = useState<string | null>(null);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [showModifyModal, setShowModifyModal] = useState(false);
  const [showDetailsModal, setShowDetailsModal] = useState(false);
  const [actionType, setActionType] = useState<'reject' | 'modify'>('reject');
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 1
  });

  // Transform API data to match what your table expects
  const transformPropertiesForTable = (items: AdminPropertyListDto[]) => {
    return items.map(item => ({
      ...item,
      // Map id to propertyId for the table if it expects propertyId
      propertyId: item.id,
      // Format status for display
      statusText: getStatusText(item.status),
      // Format date if available
      createdAt: item.createdAt ? new Date(item.createdAt).toLocaleDateString() : 'N/A',
      // Format currency
      formattedValue: new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
      }).format(item.totalValue),
      // Format price per unit
      formattedPricePerUnit: new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      }).format(item.pricePerUnit),
      // Format ETH price
      formattedPricePerUnitEth: `${item.pricePerUnitEth.toFixed(6)} ETH`
    }));
  };

  const getStatusText = (status: number): string => {
    const statusMap: Record<number, string> = {
      0: 'Draft',
      1: 'Pending',
      2: 'Approved',
      3: 'Rejected',
      4: 'Modification Requested'
    };
    return statusMap[status] || 'Unknown';
  };

  // Table columns - showing only key information
  const columns = [
    {
      key: 'imageUrl',
      header: 'Image',
      render: (value: string) => (
        <img
          src={value}
          alt="Property"
          className="table-thumbnail"
        />
      )
    },
    { 
      key: 'name', 
      header: 'Property Name',
      render: (value: string) => <span className="td-bold">{value}</span>
    },
    { key: 'location', header: 'Location' },
    { key: 'propertyType', header: 'Type' },
    {
      key: 'formattedValue',
      header: 'Total Value',
      render: (value: string) => <span className="td-number">{value}</span>
    },
    {
      key: 'statusText',
      header: 'Status',
      render: (value: string) => {
        const statusClass = value.toLowerCase().replace(' ', '-');
        return (
          <span className={`badge ${statusClass}`}>{value}</span>
        );
      }
    },
    {
      key: 'createdAt',
      header: 'Submitted',
      render: (value: string) => <span style={{ whiteSpace: 'nowrap' }}>{value}</span>
    }
  ];

  const fetchPendingProperties = async (page = 1) => {
    setLoading(true);
    try {
      const response = await api.get<PaginatedResponse<AdminPropertyListDto>>(
        `/api/admin/properties/pending?page=${page}&pageSize=${pagination.pageSize}`
      );

      console.log('Properties Response:', response.data);

      if (response.data && response.data.items) {
        const transformedData = transformPropertiesForTable(response.data.items);
        setProperties(transformedData);
        setPagination({
          page: response.data.page,
          pageSize: response.data.pageSize,
          totalCount: response.data.totalCount,
          totalPages: response.data.totalPages
        });
      } else {
        setProperties([]);
      }
    } catch (error: any) {
      console.error('Failed to fetch properties', error);
      if (error.response?.status === 401) {
        // Handle unauthorized - your interceptor should handle this
        console.error('Unauthorized access');
      }
      setProperties([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPendingProperties();
  }, []);

  const handleRowClick = (property: AdminPropertyListDto) => {
    setSelectedProperty(property);
    setShowDetailsModal(true);
  };

  const handleApprove = async (id: string) => {
    try {
      await api.post(`/api/admin/properties/${id}/approve`);
      // Refresh the current page
      fetchPendingProperties(pagination.page);
    } catch (error: any) {
      console.error('Failed to approve property', error);
      const errorMessage = error.response?.data?.message || 'Failed to approve property';
      alert(errorMessage);
    }
  };

  const handleReject = async (reason: string) => {
    if (!selectedPropertyId) return;
    try {
      const rejectRequest: RejectPropertyRequest = { reason };
      await api.post(`/api/admin/properties/${selectedPropertyId}/reject`, rejectRequest);
      fetchPendingProperties(pagination.page);
      setShowRejectModal(false);
      setSelectedPropertyId(null);
    } catch (error: any) {
      console.error('Failed to reject property', error);
      const errorMessage = error.response?.data?.message || 'Failed to reject property';
      alert(errorMessage);
    }
  };

  const handleModify = async (reason: string) => {
    if (!selectedPropertyId) return;
    try {
      const modifyRequest: RejectPropertyRequest = { reason };
      await api.post(`/api/admin/properties/${selectedPropertyId}/modify`, modifyRequest);
      fetchPendingProperties(pagination.page);
      setShowModifyModal(false);
      setSelectedPropertyId(null);
    } catch (error: any) {
      console.error('Failed to send modification request', error);
      const errorMessage = error.response?.data?.message || 'Failed to send modification request';
      alert(errorMessage);
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
    setShowDetailsModal(false);
    setSelectedProperty(null);
    setSelectedPropertyId(null);
  };

  const handlePageChange = (newPage: number) => {
    fetchPendingProperties(newPage);
  };

  if (loading && properties.length === 0) return <LoadingSpinner />;

  return (
    <div className="properties-pending">
      <div className="page-header">
        <div>
          <h1>Pending Properties</h1>
          <p>Review and approve new property listings</p>
        </div>
        <div className="page-header-stats">
          <span>Total: {pagination.totalCount}</span>
          <span>Page: {pagination.page} of {pagination.totalPages}</span>
        </div>
      </div>

      {properties.length === 0 ? (
        <div className="empty-state">
          <p>No pending properties to review</p>
        </div>
      ) : (
        <>
          <div className="table-clickable">
            <Table
              columns={columns}
              data={properties}
              onApprove={handleApprove}
              onReject={openRejectModal}
              onModify={openModifyModal}
              showActions={true}
              showModifyButton={true}
              onRowClick={handleRowClick}
            />
          </div>

          {/* Pagination Controls */}
          {pagination.totalPages > 1 && (
            <div className="pagination">
              <button
                onClick={() => handlePageChange(pagination.page - 1)}
                disabled={pagination.page === 1}
                className="btn btn-secondary"
              >
                Previous
              </button>
              <span className="pagination-info">
                Page {pagination.page} of {pagination.totalPages}
              </span>
              <button
                onClick={() => handlePageChange(pagination.page + 1)}
                disabled={pagination.page === pagination.totalPages}
                className="btn btn-secondary"
              >
                Next
              </button>
            </div>
          )}
        </>
      )}

      {/* Property Details Modal */}
      {showDetailsModal && selectedProperty && (
        <div className="modal-overlay" onClick={closeModals}>
          <div className="modal modal-large" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2>Property Details</h2>
              <button onClick={closeModals} className="btn-close">&times;</button>
            </div>
            <div className="modal-body">
              <div className="property-details">
                {/* Main Image */}
                <div className="property-image-container">
                  <img
                    src={selectedProperty.imageUrl}
                    alt={selectedProperty.name}
                    className="property-main-image"
                  />
                </div>

                {/* Basic Information */}
                <div className="details-section">
                  <h3>Basic Information</h3>
                  <div className="details-grid">
                    <div className="detail-item">
                      <label>Property Name:</label>
                      <span>{selectedProperty.name}</span>
                    </div>
                    <div className="detail-item">
                      <label>Location:</label>
                      <span>{selectedProperty.location}</span>
                    </div>
                    <div className="detail-item">
                      <label>Property Type:</label>
                      <span>{selectedProperty.propertyType}</span>
                    </div>
                    <div className="detail-item">
                      <label>Status:</label>
                      <span className={`badge ${getStatusText(selectedProperty.status).toLowerCase()}`}>
                        {getStatusText(selectedProperty.status)}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Description */}
                <div className="details-section">
                  <h3>Description</h3>
                  <p className="property-description">{selectedProperty.description}</p>
                </div>

                {/* Financial Details */}
                <div className="details-section">
                  <h3>Financial Details</h3>
                  <div className="details-grid">
                    <div className="detail-item">
                      <label>Total Value:</label>
                      <span className="highlight">
                        {new Intl.NumberFormat('en-US', {
                          style: 'currency',
                          currency: 'USD',
                          minimumFractionDigits: 0,
                          maximumFractionDigits: 0
                        }).format(selectedProperty.totalValue)}
                      </span>
                    </div>
                    <div className="detail-item">
                      <label>Total Units:</label>
                      <span>{selectedProperty.totalUnits.toLocaleString()}</span>
                    </div>
                    <div className="detail-item">
                      <label>Available Units:</label>
                      <span>{selectedProperty.availableUnits.toLocaleString()}</span>
                    </div>
                    <div className="detail-item">
                      <label>Price Per Unit (USD):</label>
                      <span>
                        {new Intl.NumberFormat('en-US', {
                          style: 'currency',
                          currency: 'USD',
                          minimumFractionDigits: 2,
                          maximumFractionDigits: 2
                        }).format(selectedProperty.pricePerUnit)}
                      </span>
                    </div>
                    <div className="detail-item">
                      <label>Price Per Unit (ETH):</label>
                      <span>{selectedProperty.pricePerUnitEth.toFixed(6)} ETH</span>
                    </div>
                    <div className="detail-item">
                      <label>Annual Yield:</label>
                      <span className="highlight">{selectedProperty.annualYieldPercent}%</span>
                    </div>
                    <div className="detail-item">
                      <label>Risk Score:</label>
                      <span>
                        <span className={`risk-score risk-${selectedProperty.riskScore}`}>
                          {selectedProperty.riskScore}/10
                        </span>
                      </span>
                    </div>
                    <div className="detail-item">
                      <label>Demand Score:</label>
                      <span>{selectedProperty.demandScore || 'N/A'}</span>
                    </div>
                    <div className="detail-item">
                      <label>Rental Income History:</label>
                      <span>
                        {new Intl.NumberFormat('en-US', {
                          style: 'currency',
                          currency: 'USD',
                          minimumFractionDigits: 0,
                          maximumFractionDigits: 0
                        }).format(selectedProperty.rentalIncomeHistory)}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Documents */}
                {selectedProperty.documents && selectedProperty.documents.length > 0 && (
                  <div className="details-section">
                    <h3>Documents</h3>
                    <div className="documents-list">
                      {selectedProperty.documents.map((doc, index) => (
                        <a
                          key={index}
                          href={doc.documentUrl}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="document-link"
                        >
                          <span className="document-icon">📄</span>
                          <span className="document-title">{doc.title}</span>
                          <span className="document-file">{doc.fileName}</span>
                        </a>
                      ))}
                    </div>
                  </div>
                )}

                {/* Permissions */}
                <div className="details-section">
                  <h3>Permissions</h3>
                  <div className="permissions-grid">
                    <div className="permission-item">
                      <span className={`permission-badge ${selectedProperty.canEditFullProperty ? 'yes' : 'no'}`}>
                        {selectedProperty.canEditFullProperty ? '✓' : '✗'} Edit Full Property
                      </span>
                    </div>
                    <div className="permission-item">
                      <span className={`permission-badge ${selectedProperty.canResubmit ? 'yes' : 'no'}`}>
                        {selectedProperty.canResubmit ? '✓' : '✗'} Can Resubmit
                      </span>
                    </div>
                    <div className="permission-item">
                      <span className={`permission-badge ${selectedProperty.canRequestUpdate ? 'yes' : 'no'}`}>
                        {selectedProperty.canRequestUpdate ? '✓' : '✗'} Can Request Update
                      </span>
                    </div>
                    <div className="permission-item">
                      <span className={`permission-badge ${selectedProperty.canDelete ? 'yes' : 'no'}`}>
                        {selectedProperty.canDelete ? '✓' : '✗'} Can Delete
                      </span>
                    </div>
                    <div className="permission-item">
                      <span className={`permission-badge ${selectedProperty.hasPendingUpdateRequest ? 'pending' : 'no'}`}>
                        {selectedProperty.hasPendingUpdateRequest ? '⏳' : '✗'} Pending Update Request
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="modal-footer">
              <button onClick={closeModals} className="btn btn-secondary">Close</button>
            </div>
          </div>
        </div>
      )}

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