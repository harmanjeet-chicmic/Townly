import React from 'react';
import { useQuery } from '@tanstack/react-query';
import api from '../services/api';
import { TrendingUp, Wallet, ArrowUpRight, Building2 } from 'lucide-react';

const Portfolio: React.FC = () => {
    const { data: portfolioData, isLoading: portfolioLoading } = useQuery({
        queryKey: ['portfolio-properties'],
        queryFn: async () => {
            const res = await api.get('/api/portfolio/me/properties');
            return res.data;
        }
    });

    const { data: overviewData } = useQuery({
        queryKey: ['portfolio-overview'],
        queryFn: async () => {
            const res = await api.get('/api/portfolio/me/overview');
            return res.data;
        }
    });

    const overview = overviewData || {};

    const stats = [
        { label: 'Total Invested', value: overview.totalInvestedUsd ? `$${overview.totalInvestedUsd?.toLocaleString()}` : '$0', icon: <Wallet size={20} />, color: 'var(--primary)' },
        { label: 'Active Properties', value: overview.propertyCount || '0', icon: <Building2 size={20} />, color: 'var(--secondary)' },
        { label: 'Monthly Yield (Est)', value: overview.estimatedMonthlyIncome ? `$${overview.estimatedMonthlyIncome?.toLocaleString()}` : '$0', icon: <TrendingUp size={20} />, color: 'var(--primary)' },
    ];

    const myProperties = portfolioData || [];

    return (
        <div className="portfolio-container">
            <div className="section-title">
                <div>
                    <h2>Your Portfolio</h2>
                    <p style={{ color: 'var(--text-secondary)' }}>Track your real estate assets and earnings in real-time.</p>
                </div>
            </div>

            <div className="stats-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '24px', marginBottom: '40px' }}>
                {stats.map((stat, i) => (
                    <div key={i} className="card" style={{ padding: '24px', display: 'flex', alignItems: 'center', gap: '20px' }}>
                        <div style={{ padding: '12px', background: 'rgba(255,255,255,0.03)', color: stat.color, borderRadius: '12px', border: '1px solid var(--card-border)' }}>
                            {stat.icon}
                        </div>
                        <div>
                            <span style={{ fontSize: '13px', color: 'var(--text-muted)', display: 'block', marginBottom: '4px' }}>{stat.label}</span>
                            <strong style={{ fontSize: '24px', fontWeight: '800' }}>{stat.value}</strong>
                        </div>
                    </div>
                ))}
            </div>

            <div className="card" style={{ padding: '0' }}>
                <div style={{ padding: '24px', borderBottom: '1px solid var(--card-border)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <h3 style={{ fontSize: '18px' }}>Active Investments</h3>
                    <button style={{ background: 'transparent', border: 'none', color: 'var(--primary)', fontSize: '14px', fontWeight: '600', cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 4 }}>
                        View Detailed Reports <ArrowUpRight size={16} />
                    </button>
                </div>

                <div className="table-wrapper" style={{ overflowX: 'auto' }}>
                    {portfolioLoading ? (
                        <div style={{ padding: '40px', textAlign: 'center' }}>Loading investments...</div>
                    ) : myProperties.length > 0 ? (
                        <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
                            <thead>
                                <tr style={{ color: 'var(--text-muted)', fontSize: '12px', textTransform: 'uppercase', letterSpacing: '0.05em' }}>
                                    <th style={{ padding: '16px 24px' }}>Property</th>
                                    <th style={{ padding: '16px 24px' }}>Investment</th>
                                    <th style={{ padding: '16px 24px' }}>ROI</th>
                                    <th style={{ padding: '16px 24px' }}>Monthly Payout</th>
                                    <th style={{ padding: '16px 24px' }}>Status</th>
                                </tr>
                            </thead>
                            <tbody>
                                {myProperties.map((prop: any) => (
                                    <tr key={prop.propertyId || prop.id} style={{ borderTop: '1px solid var(--card-border)' }}>
                                        <td style={{ padding: '24px' }}>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                                                <div style={{ width: '48px', height: '48px', background: `url(${prop.imageUrl}) center/cover`, borderRadius: '12px', border: '1px solid var(--card-border)' }}></div>
                                                <div>
                                                    <div style={{ fontWeight: '700', fontSize: '15px' }}>{prop.propertyName || prop.name}</div>
                                                    <div style={{ fontSize: '12px', color: 'var(--text-muted)' }}>{prop.location}</div>
                                                </div>
                                            </div>
                                        </td>
                                        <td style={{ padding: '24px', fontWeight: '700' }}>{prop.totalSharesOwned} Tokens</td>
                                        <td style={{ padding: '24px', color: 'var(--primary)', fontWeight: '700' }}>{prop.annualYieldPercent}%</td>
                                        <td style={{ padding: '24px', fontWeight: '700' }}>${prop.totalInvestedUsd?.toLocaleString()}</td>
                                        <td style={{ padding: '24px' }}>
                                            <span style={{ padding: '6px 12px', borderRadius: '8px', background: 'rgba(192, 255, 0, 0.1)', color: 'var(--primary)', fontSize: '11px', fontWeight: '800', border: '1px solid var(--primary-glow)' }}>OWNED</span>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    ) : (
                        <div style={{ padding: '80px', textAlign: 'center' }}>
                            <Building2 size={48} style={{ color: 'var(--text-muted)', marginBottom: '16px' }} />
                            <p style={{ color: 'var(--text-secondary)' }}>You don't have any active investments yet.</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Portfolio;
