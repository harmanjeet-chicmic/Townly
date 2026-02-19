import React from 'react';

interface Column {
  key: string;
  header: string;
  render?: (value: any, item: any) => React.ReactNode;
}

interface TableProps {
  columns: Column[];
  data: any;
  onApprove?: (id: string) => void;
  onReject?: (id: string) => void;
  showActions?: boolean;
}

const Table: React.FC<TableProps> = ({
  columns,
  data,
  onApprove,
  onReject,
  showActions = true,
}) => {
  // Ensure data is an array
  const safeData = React.useMemo(() => {
    if (Array.isArray(data)) {
      return data;
    } else if (data && typeof data === 'object') {
      return [data];
    }
    return [];
  }, [data]);

  // Get the correct ID based on what's available in the item
  const getItemId = (item: any): string | undefined => {
    // Log what keys are available (remove in production)
    console.log('Getting ID from item:', item);
    
    // For property updates, it MUST be updateRequestId
    if (item.updateRequestId) {
      console.log('✅ Using updateRequestId:', item.updateRequestId);
      return item.updateRequestId;
    }
    
    // Fallbacks for other pages
    if (item.kycId) {
      console.log('✅ Using kycId:', item.kycId);
      return item.kycId;
    }
    
    if (item.propertyId && !item.updateRequestId) {
      console.log('⚠️ Using propertyId (might be wrong for updates):', item.propertyId);
      return item.propertyId;
    }
    
    if (item.requestId) {
      console.log('✅ Using requestId:', item.requestId);
      return item.requestId;
    }
    
    if (item.id) {
      console.log('✅ Using id:', item.id);
      return item.id;
    }
    
    console.warn('❌ No ID found in item:', item);
    return undefined;
  };

  return (
    <div className="table-container">
      <table>
        <thead>
          <tr>
            {columns.map((col) => (
              <th key={col.key}>{col.header}</th>
            ))}
            {showActions && <th>Actions</th>}
          </tr>
        </thead>
        <tbody>
          {safeData.length > 0 ? (
            safeData.map((item, index) => {
              const itemId = getItemId(item);
              return (
                <tr key={itemId || index}>
                  {columns.map((col) => (
                    <td key={col.key}>
                      {col.render
                        ? col.render(item[col.key], item)
                        : item[col.key] || '-'}
                    </td>
                  ))}
                  {showActions && (
                    <td>
                      <div className="action-buttons">
                        {onApprove && itemId && (
                          <button
                            onClick={() => {
                              console.log('Approve clicked with ID:', itemId);
                              onApprove(itemId);
                            }}
                            className="btn btn-success"
                          >
                            ✓ Approve
                          </button>
                        )}
                        {onReject && itemId && (
                          <button
                            onClick={() => {
                              console.log('Reject clicked with ID:', itemId);
                              onReject(itemId);
                            }}
                            className="btn btn-danger"
                          >
                            ✗ Reject
                          </button>
                        )}
                      </div>
                    </td>
                  )}
                </tr>
              );
            })
          ) : (
            <tr>
              <td colSpan={columns.length + (showActions ? 1 : 0)} style={{ textAlign: 'center', padding: '40px' }}>
                No pending items found
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default Table;