import React, { useEffect, useState } from 'react';
import {
  UserCheck,
  Building2,
  RefreshCcw,
  Coins,
  ArrowRight
} from 'lucide-react';
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
    <div className="dashboard-container">
      <div className="page-header">
        <h1>Dashboard</h1>
        <p>Overview of Townly's platform activity</p>
      </div>

      <div className="stats-grid">
        <div className="card stat-card">
          <div className="icon-wrapper">
            <UserCheck size={32} />
          </div>
          <h3>Pending KYC</h3>
          <div className="number">{stats.pendingKyc}</div>
        </div>

        <div className="card stat-card">
          <div className="icon-wrapper">
            <Building2 size={32} />
          </div>
          <h3>Pending Properties</h3>
          <div className="number">{stats.pendingProperties}</div>
        </div>

        <div className="card stat-card">
          <div className="icon-wrapper">
            <RefreshCcw size={32} />
          </div>
          <h3>Property Updates</h3>
          <div className="number">{stats.pendingUpdates}</div>
        </div>

        <div className="card stat-card">
          <div className="icon-wrapper">
            <Coins size={32} />
          </div>
          <h3>Token Requests</h3>
          <div className="number">{stats.pendingTokenRequests}</div>
        </div>
      </div>

      <div className="card">
        <h3 style={{ marginBottom: '24px', fontSize: '18px', fontWeight: '700' }}>Quick Actions</h3>
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px' }}>
          <button className="btn btn-outline" onClick={() => window.location.href = '/kyc'}>
            Review KYC <ArrowRight size={16} />
          </button>
          <button className="btn btn-outline" onClick={() => window.location.href = '/properties'}>
            Review Properties <ArrowRight size={16} />
          </button>
          <button className="btn btn-outline" onClick={() => window.location.href = '/token-requests'}>
            Review Token Requests <ArrowRight size={16} />
          </button>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
