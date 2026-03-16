import React, { useState } from 'react';
import './PropertyFilters.css';

const PropertyFilters = ({ onFilterChange }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [filters, setFilters] = useState({
    propertyType: '',
    minYield: '',
    maxRisk: '',
  });

  const handleFilterChange = (key, value) => {
    const newFilters = { ...filters, [key]: value };
    setFilters(newFilters);
    onFilterChange(newFilters);
  };

  const clearFilters = () => {
    const emptyFilters = {
      propertyType: '',
      minYield: '',
      maxRisk: '',
    };
    setFilters(emptyFilters);
    onFilterChange(emptyFilters);
  };

  return (
    <div className="property-filters">
      <button 
        className="filter-toggle"
        onClick={() => setIsOpen(!isOpen)}
      >
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
          <path d="M3 6h18M6 12h12M10 18h4" strokeWidth="2" strokeLinecap="round"/>
        </svg>
        Filters
        {isOpen ? '▲' : '▼'}
      </button>

      {isOpen && (
        <div className="filter-panel">
          <div className="filter-group">
            <label>Property Type</label>
            <select 
              value={filters.propertyType}
              onChange={(e) => handleFilterChange('propertyType', e.target.value)}
            >
              <option value="">All Types</option>
              <option value="Residential">Residential</option>
              <option value="Commercial">Commercial</option>
              <option value="Industrial">Industrial</option>
            </select>
          </div>

          <div className="filter-group">
            <label>Min. Annual Yield (%)</label>
            <input 
              type="number"
              min="0"
              max="100"
              value={filters.minYield}
              onChange={(e) => handleFilterChange('minYield', e.target.value)}
              placeholder="Any"
            />
          </div>

          <div className="filter-group">
            <label>Max. Risk Score</label>
            <input 
              type="number"
              min="0"
              max="10"
              step="0.5"
              value={filters.maxRisk}
              onChange={(e) => handleFilterChange('maxRisk', e.target.value)}
              placeholder="Any"
            />
          </div>

          <button className="clear-filters" onClick={clearFilters}>
            Clear All
          </button>
        </div>
      )}
    </div>
  );
};

export default PropertyFilters;