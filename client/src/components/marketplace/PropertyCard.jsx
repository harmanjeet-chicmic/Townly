import React from 'react';
import { useNavigate } from 'react-router-dom';
import { 
  formatCurrency, 
  formatEth, 
  formatPercentage, 
  formatPropertyType,
  calculateFundedPercentage 
} from '../../utils/formatters';
import './PropertyCard.css';

const PropertyCard = ({ property }) => {
  const navigate = useNavigate();

  const handleInvestClick = () => {
    navigate(`/property/${property.id}`);
  };

  const fundedPercentage = calculateFundedPercentage(
    property.totalUnits, 
    property.availableUnits
  );

  return (
    <div className="property-card">
      <div className="property-image">
        <img 
          src={property.imageUrl || 'https://via.placeholder.com/400x300?text=Property'} 
          alt={property.name} 
        />
        <span className={`property-type ${property.propertyType?.toLowerCase()}`}>
          {formatPropertyType(property.propertyType)}
        </span>
      </div>

      <div className="property-content">
        <h3 className="property-title">{property.name}</h3>
        
        <p className="property-location">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
            <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7z" strokeWidth="2"/>
            <circle cx="12" cy="9" r="2.5" strokeWidth="2"/>
          </svg>
          {property.location || 'Location TBD'}
        </p>

        <div className="property-details">
          <div className="detail-item">
            <span className="detail-label">Valuation</span>
            <span className="detail-value">{formatCurrency(property.approvedValuation)}</span>
          </div>
          
          <div className="detail-item">
            <span className="detail-label">Annual Yield</span>
            <span className="detail-value highlight">{formatPercentage(property.annualYieldPercent)}</span>
          </div>
          
          <div className="detail-item">
            <span className="detail-label">Price/Unit</span>
            <span className="detail-value">{formatEth(property.pricePerUnitEth)}</span>
          </div>
          
          <div className="detail-item">
            <span className="detail-label">Risk Score</span>
            <span className="detail-value">{property.riskScore}/10</span>
          </div>
        </div>

        <div className="property-stats">
          <div className="stat">
            <span className="stat-label">Available Units</span>
            <span className="stat-value">{property.availableUnits.toLocaleString()}</span>
          </div>
          <div className="stat">
            <span className="stat-label">Total Units</span>
            <span className="stat-value">{property.totalUnits.toLocaleString()}</span>
          </div>
        </div>

        <div className="property-footer">
          <div className="funding-progress">
            <div className="progress-bar">
              <div 
                className="progress-fill" 
                style={{ width: `${fundedPercentage}%` }}
              ></div>
            </div>
            <span className="funded-text">{fundedPercentage}% Funded</span>
          </div>

          <button className="invest-btn" onClick={handleInvestClick}>
            Invest Now
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <path d="M5 12h14M12 5l7 7-7 7" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </button>
        </div>
      </div>
    </div>
  );
};

export default PropertyCard;