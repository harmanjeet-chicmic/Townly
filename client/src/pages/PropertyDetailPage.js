import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useWallet } from '../context/WalletContext';
import { getPropertyById, getPropertyInvestInfo } from '../services/propertyService';
import { formatCurrency, formatEth, formatPercentage, formatPropertyType } from '../utils/formatters';
import './PropertyDetailPage.css';

const PropertyDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { wallet, connectWallet } = useWallet();
  
  const [property, setProperty] = useState(null);
  const [investInfo, setInvestInfo] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [investmentAmount, setInvestmentAmount] = useState(1);
  const [isInvesting, setIsInvesting] = useState(false);

  useEffect(() => {
    fetchPropertyDetails();
  }, [id]);

  const fetchPropertyDetails = async () => {
    setLoading(true);
    try {
      const propertyData = await getPropertyById(id);
      setProperty(propertyData);
      
      // Fetch investment info if needed
      const investData = await getPropertyInvestInfo(id);
      setInvestInfo(investData);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleInvest = async () => {
    if (!wallet.isConnected) {
      await connectWallet();
      return;
    }

    setIsInvesting(true);
    try {
      // Implement investment logic here
      // This would call your investment API
      console.log('Investing:', {
        propertyId: id,
        units: investmentAmount,
        totalEth: investmentAmount * property.pricePerUnitEth
      });
      
      // Show success message
      alert('Investment successful!');
    } catch (err) {
      alert('Investment failed: ' + err.message);
    } finally {
      setIsInvesting(false);
    }
  };

  if (loading) {
    return (
      <div className="property-detail-loading">
        <div className="spinner"></div>
        <p>Loading property details...</p>
      </div>
    );
  }

  if (error || !property) {
    return (
      <div className="property-detail-error">
        <h2>Error Loading Property</h2>
        <p>{error || 'Property not found'}</p>
        <button onClick={() => navigate('/marketplace')} className="back-btn">
          Back to Marketplace
        </button>
      </div>
    );
  }

  const fundedPercentage = ((property.totalUnits - property.availableUnits) / property.totalUnits * 100).toFixed(1);

  return (
    <div className="property-detail-page">
      <div className="property-detail-container">
        {/* Back Button */}
        <button onClick={() => navigate('/marketplace')} className="back-btn">
          ← Back to Marketplace
        </button>

        {/* Main Content */}
        <div className="property-detail-grid">
          {/* Left Column - Images */}
          <div className="property-gallery">
            <img 
              src={property.imageUrl} 
              alt={property.name} 
              className="main-image"
            />
          </div>

          {/* Right Column - Info */}
          <div className="property-info">
            <span className="property-type-badge">
              {formatPropertyType(property.propertyType)}
            </span>
            <h1 className="property-name">{property.name}</h1>
            
            <p className="property-location">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7z" strokeWidth="2"/>
                <circle cx="12" cy="9" r="2.5" strokeWidth="2"/>
              </svg>
              {property.location}
            </p>

            <div className="property-metrics">
              <div className="metric">
                <span className="metric-label">Valuation</span>
                <span className="metric-value">{formatCurrency(property.totalValue)}</span>
              </div>
              <div className="metric">
                <span className="metric-label">Annual Yield</span>
                <span className="metric-value highlight">{formatPercentage(property.annualYieldPercent)}</span>
              </div>
              <div className="metric">
                <span className="metric-label">Risk Score</span>
                <span className="metric-value">{property.riskScore}/10</span>
              </div>
              <div className="metric">
                <span className="metric-label">Demand Score</span>
                <span className="metric-value">{property.demandScore}/10</span>
              </div>
            </div>

            <div className="property-description">
              <h3>Description</h3>
              <p>{property.description}</p>
            </div>

            <div className="property-stats-detailed">
              <div className="stat-row">
                <span>Total Units:</span>
                <strong>{property.totalUnits.toLocaleString()}</strong>
              </div>
              <div className="stat-row">
                <span>Available Units:</span>
                <strong>{property.availableUnits.toLocaleString()}</strong>
              </div>
              <div className="stat-row">
                <span>Price per Unit:</span>
                <strong>{formatCurrency(property.pricePerUnit)} USD / {formatEth(property.pricePerUnitEth)}</strong>
              </div>
              <div className="stat-row">
                <span>Funding Progress:</span>
                <strong>{fundedPercentage}%</strong>
              </div>
            </div>

            {/* Investment Section */}
            <div className="investment-section">
              <h3>Investment Calculator</h3>
              
              <div className="investment-calculator">
                <div className="input-group">
                  <label>Number of Units</label>
                  <input 
                    type="number" 
                    min="1" 
                    max={property.availableUnits}
                    value={investmentAmount}
                    onChange={(e) => setInvestmentAmount(parseInt(e.target.value) || 1)}
                  />
                </div>

                <div className="investment-summary">
                  <div className="summary-row">
                    <span>Price per Unit:</span>
                    <span>{formatEth(property.pricePerUnitEth)}</span>
                  </div>
                  <div className="summary-row total">
                    <span>Total Investment:</span>
                    <span>{formatEth(investmentAmount * property.pricePerUnitEth)}</span>
                  </div>
                  <div className="summary-row">
                    <span>USD Value:</span>
                    <span>{formatCurrency(investmentAmount * property.pricePerUnit)}</span>
                  </div>
                </div>

                <button 
                  className="invest-now-btn"
                  onClick={handleInvest}
                  disabled={isInvesting || property.availableUnits === 0}
                >
                  {isInvesting ? (
                    <>
                      <span className="spinner"></span>
                      Processing...
                    </>
                  ) : !wallet.isConnected ? (
                    'Connect Wallet to Invest'
                  ) : property.availableUnits === 0 ? (
                    'Sold Out'
                  ) : (
                    'Invest Now'
                  )}
                </button>

                {!wallet.isConnected && (
                  <p className="wallet-warning">
                    Connect your wallet to start investing in this property
                  </p>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PropertyDetailPage;