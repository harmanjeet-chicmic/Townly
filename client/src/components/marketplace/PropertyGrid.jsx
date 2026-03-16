import React from 'react';
import PropertyCard from './PropertyCard';
import './PropertyGrid.css';

const PropertyGrid = ({ properties, loading, error }) => {
  if (loading) {
    return (
      <div className="property-grid-loading">
        <div className="spinner"></div>
        <p>Loading properties...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="property-grid-error">
        <h3>Error loading properties</h3>
        <p>{error}</p>
        <button onClick={() => window.location.reload()}>Try Again</button>
      </div>
    );
  }

  if (!properties || properties.length === 0) {
    return (
      <div className="property-grid-empty">
        <h3>No properties found</h3>
        <p>Check back later for new investment opportunities</p>
      </div>
    );
  }

  return (
    <div className="property-grid">
      {properties.map((property) => (
        <PropertyCard key={property.id} property={property} />
      ))}
    </div>
  );
};

export default PropertyGrid;