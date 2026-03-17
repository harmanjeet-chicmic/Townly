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
  onModify?: (id: string) => void;
  onRowClick?: (item: any) => void; // New prop for row click
  showActions?: boolean;
  showModifyButton?: boolean;
}

const Table: React.FC<TableProps> = ({
  columns,
  data,
  onApprove,
  onReject,
  onModify,
  onRowClick,
  showActions = true,
  showModifyButton = false,
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
    if (item.updateRequestId) return item.updateRequestId;
    if (item.kycId) return item.kycId;
    if (item.propertyId) return item.propertyId;
    if (item.requestId) return item.requestId;
    if (item.id) return item.id;
    return undefined;
  };

  const handleRowClick = (item: any) => {
    if (onRowClick) {
      onRowClick(item);
    }
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
                <tr
                  key={itemId || index}
                  onClick={() => handleRowClick(item)}
                  style={onRowClick ? { cursor: 'pointer' } : undefined}
                  className={onRowClick ? 'clickable-row' : ''}
                >
                  {columns.map((col) => (
                    <td key={col.key}>
                      {col.render
                        ? col.render(item[col.key], item)
                        : item[col.key] || '-'}
                    </td>
                  ))}
                  {showActions && (
                    <td onClick={(e) => e.stopPropagation()}> {/* Prevent row click when clicking actions */}
                      <div className="action-buttons">
                        {onApprove && itemId && (
                          <button
                            onClick={() => {
                              console.log('Approve clicked with ID:', itemId);
                              onApprove(itemId);
                            }}
                            className="btn btn-success"
                            title="Approve this item"
                          >
                            ✓ Approve
                          </button>
                        )}

                        {showModifyButton && onModify && itemId && (
                          <button
                            onClick={() => {
                              console.log('Modify clicked with ID:', itemId);
                              onModify(itemId);
                            }}
                            className="btn btn-warning"
                            title="Request modifications"
                          >
                            ✎ Modify
                          </button>
                        )}

                        {onReject && itemId && (
                          <button
                            onClick={() => {
                              console.log('Reject clicked with ID:', itemId);
                              onReject(itemId);
                            }}
                            className="btn btn-danger"
                            title="Reject this item"
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