import React, { useEffect, useState } from "react";
import api from "../services/api";
import Table from "../components/Table";
import RejectModal from "../components/RejectModal";
import LoadingSpinner from "../components/LoadingSpinner";
import { PendingPropertyUpdateDto } from "../types";

const PropertyUpdatesPending: React.FC = () => {
  const [updates, setUpdates] = useState<PendingPropertyUpdateDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedUpdateId, setSelectedUpdateId] = useState<string | null>(null);
  const [showRejectModal, setShowRejectModal] = useState(false);

  const columns = [
    { key: "name", header: "Property Name" },
    { key: "location", header: "Location" },
    { key: "propertyType", header: "Type" },
    {
      key: "requestedAt",
      header: "Requested",
      render: (value: string) => new Date(value).toLocaleDateString(),
    },
    {
      key: "imageUrl",
      header: "Image",
      render: (value: string) => (value ? "📸 Has Image" : "🚫 No Image"),
    },
  ];

  const fetchPendingUpdates = async () => {
    setLoading(true);
    try {
      const response = await api.get(
        "/api/admin/properties/update-requests/pending",
      );
      console.log("🔥 FULL RESPONSE:", response);
      console.log("🔥 RESPONSE DATA:", response.data);

      // Log each item's ID field
      if (Array.isArray(response.data)) {
        console.log("📋 ALL ITEMS:", response.data);
        response.data.forEach((item, index) => {
          console.log(`Item ${index}:`, {
            fullItem: item,
            updateRequestId: item.updateRequestId,
            id: item.id,
            propertyId: item.propertyId,
            type: typeof item.updateRequestId,
          });
        });
        setUpdates(response.data);
      } else if (response.data && typeof response.data === "object") {
        console.log("📋 SINGLE ITEM:", response.data);
        setUpdates([response.data]);
      } else {
        setUpdates([]);
      }
    } catch (error) {
      console.error("Failed to fetch property updates", error);
      setUpdates([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPendingUpdates();
  }, []);

  const handleApprove = async (id: string) => {
    try {
      // Using updateRequestId
      await api.post(`/api/admin/properties/update-requests/${id}/approve`);
      fetchPendingUpdates();
    } catch (error: any) {
      console.error(
        "Failed to approve property update",
        error.response?.data || error,
      );
      alert(
        "Failed to approve update: " + JSON.stringify(error.response?.data),
      );
    }
  };

  const handleReject = async (reason: string) => {
    if (!selectedUpdateId) return;

    console.log("🚀 REJECTING with ID:", selectedUpdateId);
    console.log(
      "🚀 This should match one of these:",
      updates.map((u) => u.updateRequestId),
    );

    try {
      const response = await api.post(
        `/api/admin/properties/update-requests/${selectedUpdateId}/reject`,
        { reason },
      );
      console.log("✅ Success:", response);
      fetchPendingUpdates();
      setShowRejectModal(false);
      setSelectedUpdateId(null);
    } catch (error: any) {
      console.error("❌ Error:", error.response?.data || error);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Property Update Requests</h1>
          <p>Review pending property update requests</p>
        </div>
      </div>

      <Table
        columns={columns}
        data={updates}
        onApprove={handleApprove}
        onReject={(id) => {
          console.log("🔴 REJECT CLICKED - ID from table:", id);
          console.log("🔴 ID type:", typeof id);
          console.log("🔴 ID length:", id?.length);
          console.log("🔴 Full updates array:", updates);
          console.log("🔴 Looking for item with ID:", id);

          // Find the item with this ID
          const foundItem = updates.find((u) => u.updateRequestId === id);
          console.log("🔴 Found item:", foundItem);

          setSelectedUpdateId(id);
          setShowRejectModal(true);
        }}
      />

      <RejectModal
        isOpen={showRejectModal}
        onClose={() => {
          setShowRejectModal(false);
          setSelectedUpdateId(null);
        }}
        onConfirm={handleReject}
        title="Reject Property Update"
        message="Please provide a reason for rejecting this property update request:"
      />
    </div>
  );
};

export default PropertyUpdatesPending;
