import { useState } from 'react';
import { useAccount, useConnect } from 'wagmi';
import { injected } from 'wagmi/connectors';
import {
  Building2,
  Wallet,
  ShieldCheck,
  TrendingUp,
  LayoutDashboard,
  UserCheck,
  History,
  Bell,
  LogOut,
  Fingerprint
} from 'lucide-react';
import { useAuth } from './contexts/AuthContext';
import { useQuery } from '@tanstack/react-query';
import api from './services/api';
import Marketplace from './components/Marketplace';
import Portfolio from './components/Portfolio';
import KYC from './components/KYC';
import './App.css';

function App() {
  const { isConnected } = useAccount();
  const { connect } = useConnect();
  const { login, logout, isAuthenticated, isAuthenticating } = useAuth();

  const [activeTab, setActiveTab] = useState('Marketplace');

  const { data: transactionsData } = useQuery({
    queryKey: ['my-transactions'],
    queryFn: async () => {
      const res = await api.get('/api/transactions/me');
      return res.data;
    },
    enabled: isAuthenticated
  });

  const transactions = transactionsData?.items || [];

  const navLinks = [
    { name: 'Marketplace', icon: <Building2 size={18} /> },
    { name: 'Portfolio', icon: <LayoutDashboard size={18} /> },
    { name: 'KYC', icon: <UserCheck size={18} /> },
    { name: 'History', icon: <History size={18} /> },
  ];

  if (!isAuthenticated) {
    return (
      <div className="app-layout">
        <nav className="navbar">
          <div className="nav-logo">
            <div className="logo-badge">T</div>
            <span>Townly</span>
          </div>
          <div className="nav-actions">
            {!isConnected ? (
              <button className="wallet-btn" onClick={() => connect({ connector: injected() })}>
                <Wallet size={18} />
                Connect Wallet
              </button>
            ) : (
              <button className="wallet-btn" onClick={() => login()} disabled={isAuthenticating}>
                {isAuthenticating ? 'Authenticating...' : 'Sign to Authenticate'}
              </button>
            )}
          </div>
        </nav>

        <section className="hero">
          <div style={{ background: 'rgba(192, 255, 0, 0.1)', color: 'var(--primary)', padding: '10px 24px', borderRadius: '40px', fontSize: '14px', fontWeight: '800', marginBottom: '32px', border: '1px solid var(--primary-glow)', textTransform: 'uppercase', letterSpacing: '0.1em' }}>
            Institutional Real Estate • On-Chain
          </div>
          <h1 className="text-gradient">Fractionalized <br /> Property Investing.</h1>
          <p>Own premium real estate assets across Dubai, London and New York. Earn monthly yields secured by blockchain technology.</p>
          <div style={{ display: 'flex', gap: '20px' }}>
            <button className="btn-premium neon-glow" onClick={() => isConnected ? login() : connect({ connector: injected() })}>
              Start Investing
            </button>
            <button className="btn-premium" style={{ border: '1px solid var(--card-border)', boxShadow: 'none' }}>
              View Marketplace
            </button>
          </div>
        </section>

        <section className="section-container">
          <div className="property-grid" style={{ gridTemplateColumns: 'repeat(3, 1fr)' }}>
            <div className="card" style={{ padding: '32px' }}>
              <div style={{ color: 'var(--primary)', marginBottom: '16px' }}><ShieldCheck size={32} /></div>
              <h3>Secure Ownership</h3>
              <p style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Property titles are backed by legal SPVs and legal documentation.</p>
            </div>
            <div className="card" style={{ padding: '32px' }}>
              <div style={{ color: 'var(--secondary)', marginBottom: '16px' }}><TrendingUp size={32} /></div>
              <h3>High Yields</h3>
              <p style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Receive your share of rental income directly into your wallet monthly.</p>
            </div>
            <div className="card" style={{ padding: '32px' }}>
              <div style={{ color: 'var(--primary)', marginBottom: '16px' }}><Fingerprint size={32} /></div>
              <h3>Instant Liquidity</h3>
              <p style={{ color: 'var(--text-secondary)', fontSize: '14px' }}>Trade your property tokens anytime on the Townly marketplace.</p>
            </div>
          </div>
        </section>
      </div>
    );
  }

  return (
    <div className="app-layout">
      {/* Authenticated Navbar */}
      <nav className="navbar">
        <div className="nav-logo">
          <div className="logo-badge">T</div>
          <span>Townly</span>
        </div>

        <div className="nav-links">
          {navLinks.map(link => (
            <a
              key={link.name}
              href={`#${link.name.toLowerCase()}`}
              className={`nav-link ${activeTab === link.name ? 'active' : ''}`}
              style={{ display: 'flex', alignItems: 'center', gap: 8 }}
              onClick={() => setActiveTab(link.name)}
            >
              {link.icon}
              {link.name}
            </a>
          ))}
        </div>

        <div className="nav-actions">
          <button className="icon-btn" style={{ position: 'relative', color: '#fff', background: 'transparent', border: 'none', cursor: 'pointer', padding: '10px' }}>
            <Bell size={20} />
            <span style={{ position: 'absolute', top: 4, right: 4, width: 8, height: 8, background: 'var(--primary)', borderRadius: '50%' }}></span>
          </button>
          <div style={{ width: '1px', height: '24px', background: 'var(--card-border)' }}></div>
          <button className="wallet-btn" style={{ background: 'rgba(255,255,255,0.05)', border: '1px solid var(--card-border)', color: '#fff', borderRadius: '12px' }} onClick={() => logout()}>
            <LogOut size={16} /> Logout
          </button>
        </div>
      </nav>

      <main className="section-container" style={{ paddingTop: '120px', minHeight: 'calc(100vh - 100px)' }}>
        {activeTab === 'Marketplace' && <Marketplace />}
        {activeTab === 'Portfolio' && <Portfolio />}
        {activeTab === 'KYC' && <KYC />}
        {activeTab === 'History' && (
          <div className="card" style={{ padding: '0', overflow: 'hidden' }}>
            <div style={{ padding: '32px', borderBottom: '1px solid var(--card-border)', background: 'linear-gradient(to right, rgba(16, 185, 129, 0.05), transparent)' }}>
              <h2 className="text-gradient">Transaction History</h2>
              <p style={{ color: 'var(--text-secondary)', fontSize: '15px' }}>Every transaction is cryptographically verified on the blockchain.</p>
            </div>

            {transactions.length > 0 ? (
              <div className="table-wrapper">
                <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                  <thead>
                    <tr style={{ color: 'var(--text-muted)', fontSize: '12px', textTransform: 'uppercase', textAlign: 'left' }}>
                      <th style={{ padding: '20px 32px' }}>Activity</th>
                      <th style={{ padding: '20px 32px' }}>Type</th>
                      <th style={{ padding: '20px 32px' }}>Amount</th>
                      <th style={{ padding: '20px 32px' }}>Date</th>
                      <th style={{ padding: '20px 32px' }}>Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    {transactions.map((tx: any) => (
                      <tr key={tx.id} style={{ borderTop: '1px solid var(--card-border)' }}>
                        <td style={{ padding: '20px 32px' }}>
                          <div style={{ fontWeight: '700' }}>{tx.propertyName}</div>
                          <div style={{ fontSize: '12px', color: 'var(--text-muted)' }}>{tx.sharesCount} Shares</div>
                        </td>
                        <td style={{ padding: '20px 32px' }}>
                          <span style={{ fontSize: '13px' }}>{tx.transactionType === 0 ? 'Investment' : 'Income'}</span>
                        </td>
                        <td style={{ padding: '20px 32px', fontWeight: '800' }}>
                          ${tx.amountUsd?.toLocaleString()}
                        </td>
                        <td style={{ padding: '20px 32px', color: 'var(--text-secondary)' }}>
                          {new Date(tx.createdAt).toLocaleDateString()}
                        </td>
                        <td style={{ padding: '20px 32px' }}>
                          <span style={{ padding: '6px 14px', borderRadius: '10px', background: 'rgba(192, 255, 0, 0.1)', color: 'var(--primary)', fontSize: '11px', fontWeight: '900', border: '1px solid var(--primary-glow)' }}>SUCCESS</span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <div style={{ padding: '100px', textAlign: 'center' }}>
                <History size={64} style={{ color: 'var(--text-muted)', marginBottom: '24px', opacity: 0.5 }} />
                <h3 style={{ marginBottom: '8px' }}>No Activity Yet</h3>
                <p style={{ color: 'var(--text-secondary)' }}>Your investment history will appear here once you start building your portfolio.</p>
              </div>
            )}
          </div>
        )}
      </main>

      {/* Footer */}
      <footer className="section-container" style={{ borderTop: '1px solid var(--card-border)', marginTop: 'auto', textAlign: 'center', padding: '40px 24px' }}>
        <div className="nav-logo" style={{ justifyContent: 'center', marginBottom: '24px' }}>
          <div className="logo-badge">T</div>
          <span>Townly</span>
        </div>
        <p style={{ color: 'var(--text-muted)', fontSize: '13px' }}>© 2026 Townly Real Estate Tokenization. All rights reserved.</p>
      </footer>
    </div>
  );
}

export default App;
