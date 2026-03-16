import { useState, useEffect } from 'react';
import { getMarketplaceProperties } from '../services/propertyService';

export const useProperties = (initialPage = 1, initialPageSize = 9) => {
  const [properties, setProperties] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [page, setPage] = useState(initialPage);
  const [totalCount, setTotalCount] = useState(0);
  const [hasMore, setHasMore] = useState(false);
  const [pageSize] = useState(initialPageSize); // Fixed: renamed parameter to initialPageSize

  useEffect(() => {
    fetchProperties();
  }, [page, pageSize]);

  const fetchProperties = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await getMarketplaceProperties(page, pageSize);
      setProperties(response.items || []);
      setTotalCount(response.totalCount || 0);
      setHasMore(response.hasMore || false);
    } catch (err) {
      setError(err.message);
      console.error("Error in useProperties:", err);
    } finally {
      setLoading(false);
    }
  };

  const nextPage = () => {
    if (hasMore) {
      setPage(prev => prev + 1);
    }
  };

  const prevPage = () => {
    if (page > 1) {
      setPage(prev => prev - 1);
    }
  };

  const goToPage = (pageNumber) => {
    setPage(pageNumber);
  };

  return {
    properties,
    loading,
    error,
    page,
    totalCount,
    hasMore,
    pageSize,
    nextPage,
    prevPage,
    goToPage,
    refresh: fetchProperties,
  };
};