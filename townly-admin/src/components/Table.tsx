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
      // If it's a single object, wrap in array
      return [data];
    }
    // Default to empty array
    return [];
  }, [data]);

  // Get the actual ID from the item, checking multiple possible ID fields
  const getItemId = (item: any): string | undefined => {
    return item.kycId || item.propertyId || item.requestId || item.updateRequestId || item.id;
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
            safeData.map((item, index) => (
              <tr key={getItemId(item) || index}>
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
                      {onApprove && (
                        <button
                          onClick={() => onApprove(getItemId(item) || '')}
                          className="btn btn-success"
                        >
                          ✓ Approve
                        </button>
                      )}
                      {onReject && (
                        <button
                          onClick={() => onReject(getItemId(item) || '')}
                          className="btn btn-danger"
                        >
                          ✗ Reject
                        </button>
                      )}
                    </div>
                  </td>
                )}
              </tr>
            ))
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