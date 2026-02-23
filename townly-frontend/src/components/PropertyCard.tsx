import React from 'react';
import { MapPin } from 'lucide-react';

interface Property {
    id: string;
    title: string;
    location: string;
    roi: string;
    price: string;
    type: string;
    tokensLeft: string;
    imageUrl?: string;
    riskScore?: number;
}

interface PropertyCardProps {
    property: Property;
    onInvest?: (id: string) => void;
}

const PropertyCard: React.FC<PropertyCardProps> = ({ property, onInvest }) => {
    return (
        <div className="property-card">
            <div className="property-image" style={{ backgroundImage: `url(${property.imageUrl || 'https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=800&q=80'})`, backgroundSize: 'cover', backgroundPosition: 'center' }}>
                <div className="property-badge">{property.roi} APY</div>
                {property.riskScore && (
                    <div className="property-badge" style={{ left: 16, right: 'auto', background: 'rgba(99, 102, 241, 0.6)', color: '#fff', borderColor: 'var(--secondary-glow)' }}>
                        Risk: {property.riskScore}/10
                    </div>
                )}
            </div>
            <div className="property-content">
                <h3>{property.title}</h3>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6, color: 'var(--text-muted)', fontSize: 13, marginBottom: 16 }}>
                    <MapPin size={14} /> {property.location}
                </div>

                <div className="property-info">
                    <div className="info-item">
                        <span>Price</span>
                        <strong>{property.price}</strong>
                    </div>
                    <div className="info-item">
                        <span>Type</span>
                        <strong>{property.type}</strong>
                    </div>
                    <div className="info-item">
                        <span>Progress</span>
                        <strong style={{ color: 'var(--primary)' }}>{property.tokensLeft}</strong>
                    </div>
                </div>

                <button
                    className="btn-premium"
                    style={{ width: '100%', marginTop: '20px', padding: '12px', fontSize: '14px' }}
                    onClick={() => onInvest?.(property.id)}
                >
                    Invest Now
                </button>
            </div>
        </div>
    );
};

export default PropertyCard;
