import React, { useEffect, useState } from 'react';
import api from '../services/api';
import LoadingSpinner from '../components/LoadingSpinner';

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState({
    pendingKyc: 0,
    pendingProperties: 0,
    pendingUpdates: 0,
    pendingTokenRequests: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [kyc, properties, updates, tokens] = await Promise.all([
          api.get('/api/admin/kyc/pending'),
          api.get('/api/admin/properties/pending'),
          api.get('/api/admin/properties/update-requests/pending'),
          api.get('/api/admin/tokens/requests'),
        ]);

        setStats({
          pendingKyc: kyc.data.length,
          pendingProperties: properties.data.length,
          pendingUpdates: updates.data.length,
          pendingTokenRequests: tokens.data.length,
        });
      } catch (error) {
        console.error('Failed to fetch stats', error);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1>Dashboard</h1>
          <p>Welcome to your admin dashboard</p>
        </div>
      </div>

      <div className="stats-grid">
        <div className="stat-card">
          <h3>Pending KYC</h3>
          <div className="number">{stats.pendingKyc}</div>
        </div>
        <div className="stat-card">
          <h3>Pending Properties</h3>
          <div className="number">{stats.pendingProperties}</div>
        </div>
        <div className="stat-card">
          <h3>Property Updates</h3>
          <div className="number">{stats.pendingUpdates}</div>
        </div>
        <div className="stat-card">
          <h3>Token Requests</h3>
          <div className="number">{stats.pendingTokenRequests}</div>
        </div>
      </div>

      <div className="card">
        <h3 style={{ marginBottom: '16px' }}>Quick Actions</h3>
        <div style={{ display: 'flex', gap: '12px' }}>
          <button className="btn btn-primary" onClick={() => window.location.href = '/kyc'}>
            Review KYC
          </button>
          <button className="btn btn-primary" onClick={() => window.location.href = '/properties'}>
            Review Properties
          </button>
          <button className="btn btn-primary" onClick={() => window.location.href = '/token-requests'}>
            Review Token Requests
          </button>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;