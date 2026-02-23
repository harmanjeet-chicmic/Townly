import React, { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import api from '../services/api';
import PropertyCard from './PropertyCard';
import { Search, Building2 } from 'lucide-react';

const Marketplace: React.FC = () => {
    const [search, setSearch] = useState('');
    const [type, setType] = useState('');

    const { data, isLoading } = useQuery({
        queryKey: ['properties', search, type],
        queryFn: async () => {
            const res = await api.get('/api/properties/marketplace', {
                params: {
                    page: 1,
                    pageSize: 12,
                    search,
                    propertyType: type
                }
            });
            return res.data;
        }
    });

    const { data: featuredData } = useQuery({
        queryKey: ['featured-properties'],
        queryFn: async () => {
            const res = await api.get('/api/properties/featured');
            return res.data;
        }
    });

    const properties = data?.items || [];
    const featuredProperties = featuredData || [];

    return (
        <div className="marketplace-container">
            {featuredProperties.length > 0 && (
                <section style={{ marginBottom: '60px' }}>
                    <div className="section-title">
                        <div>
                            <h2 className="text-gradient">Featured Opportunities</h2>
                            <p style={{ color: 'var(--text-secondary)' }}>Institutional-grade assets with the highest projected yields.</p>
                        </div>
                    </div>
                    <div className="property-grid" style={{ gridTemplateColumns: 'repeat(2, 1fr)', gap: '32px' }}>
                        {featuredProperties.map((prop: any) => (
                            <PropertyCard
                                key={prop.id}
                                property={{
                                    id: prop.id,
                                    title: prop.name,
                                    location: prop.location,
                                    roi: prop.annualYieldPercent + '%',
                                    price: prop.pricePerUnitEth ? prop.pricePerUnitEth + ' ETH' : '$' + prop.approvedValuation?.toLocaleString(),
                                    type: prop.propertyType,
                                    tokensLeft: prop.availableUnits?.toLocaleString() + ' Units Left',
                                    imageUrl: prop.imageUrl,
                                    riskScore: prop.riskScore
                                }}
                            />
                        ))}
                    </div>
                </section>
            )}

            <div className="section-title">
                <div>
                    <h2>Real Estate Marketplace</h2>
                    <p style={{ color: 'var(--text-secondary)' }}>Discover and invest in institutional-grade real estate.</p>
                </div>
            </div>

            <div className="filters-bar" style={{ display: 'flex', gap: '16px', marginBottom: '40px' }}>
                <div style={{ position: 'relative', flex: 1 }}>
                    <Search size={18} style={{ position: 'absolute', left: 16, top: '50%', transform: 'translateY(-50%)', color: 'var(--text-muted)' }} />
                    <input
                        type="text"
                        placeholder="Search location, title..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                        style={{ width: '100%', padding: '12px 12px 12px 48px', backgroundColor: 'var(--glass-bg)', border: '1px solid var(--card-border)', borderRadius: '12px', color: '#fff', outline: 'none' }}
                    />
                </div>
                <select
                    value={type}
                    onChange={(e) => setType(e.target.value)}
                    style={{ padding: '12px 24px', background: 'var(--glass-bg)', border: '1px solid var(--card-border)', borderRadius: '12px', color: '#fff', outline: 'none' }}
                >
                    <option value="">All Types</option>
                    <option value="Residential">Residential</option>
                    <option value="Commercial">Commercial</option>
                    <option value="Industrial">Industrial</option>
                </select>
            </div>

            {isLoading ? (
                <div style={{ textAlign: 'center', padding: '100px' }}>Loading Marketplace...</div>
            ) : (
                <div className="property-grid">
                    {properties.length > 0 ? (
                        properties.map((prop: any) => (
                            <PropertyCard
                                key={prop.id}
                                property={{
                                    id: prop.id,
                                    title: prop.name,
                                    location: prop.location,
                                    roi: prop.annualYieldPercent + '%',
                                    price: prop.pricePerUnitEth ? prop.pricePerUnitEth + ' ETH' : '$' + prop.approvedValuation?.toLocaleString(),
                                    type: prop.propertyType,
                                    tokensLeft: prop.availableUnits?.toLocaleString() + ' Units Left',
                                    imageUrl: prop.imageUrl,
                                    riskScore: prop.riskScore
                                }}
                            />
                        ))
                    ) : (
                        <div style={{ gridColumn: '1/-1', textAlign: 'center', padding: '100px', background: 'var(--glass-bg)', borderRadius: '20px' }}>
                            <Building2 size={48} style={{ color: 'var(--text-muted)', marginBottom: '16px' }} />
                            <h3>No properties found</h3>
                            <p style={{ color: 'var(--text-secondary)' }}>Try adjusting your filters or search query.</p>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default Marketplace;
