import React, { useState } from 'react';
import PropertyGrid from '../components/marketplace/PropertyGrid';
import PropertyFilters from '../components/marketplace/PropertyFilters';
import SearchBar from '../components/marketplace/SearchBar';
import { useProperties } from '../hooks/useProperties';
import './MarketplacePage.css';

const MarketplacePage = () => {
  const [searchQuery, setSearchQuery] = useState('');
  const {
    properties,
    loading,
    error,
    page,
    totalCount,
    hasMore,
    nextPage,
    prevPage
  } = useProperties(1, 9);

  const handleSearch = (query) => {
    setSearchQuery(query);
    // You'll need to add search functionality to your useProperties hook
    console.log('Searching for:', query);
  };

  const handleFilterChange = (filters) => {
    console.log('Filters changed:', filters);
    // You'll need to add filter functionality to your useProperties hook
  };

  return (
    <div className="marketplace-page">
      {/* Hero Section */}
      <section className="marketplace-hero">
        <div className="hero-content">
          <h1>Discover Premium Real Estate Investments</h1>
          <p>Browse through our curated selection of tokenized properties and start your investment journey today</p>
        </div>
      </section>

      {/* Main Content */}
      <section className="marketplace-content">
        <div className="container">
          {/* Search and Filters */}
          <div className="marketplace-toolbar">
            <SearchBar onSearch={handleSearch} />
            <PropertyFilters onFilterChange={handleFilterChange} />
          </div>

          {/* Results Count */}
          <div className="results-info">
            <p>Showing <strong>{properties.length}</strong> of <strong>{totalCount}</strong> properties</p>
          </div>

          {/* Property Grid */}
          <PropertyGrid 
            properties={properties} 
            loading={loading} 
            error={error} 
          />

          {/* Pagination */}
          {!loading && !error && properties.length > 0 && (
            <div className="pagination">
              <button 
                onClick={prevPage} 
                disabled={page === 1}
                className="pagination-btn"
              >
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                  <path d="M15 18l-6-6 6-6" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                </svg>
                Previous
              </button>
              
              <span className="page-info">
                Page {page} of {Math.ceil(totalCount / 9)}
              </span>
              
              <button 
                onClick={nextPage} 
                disabled={!hasMore}
                className="pagination-btn"
              >
                Next
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                  <path d="M9 18l6-6-6-6" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                </svg>
              </button>
            </div>
          )}
        </div>
      </section>
    </div>
  );
};

export default MarketplacePage;