export const formatCurrency = (amount) => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(amount);
};

export const formatEth = (ethAmount) => {
  return `${ethAmount.toFixed(6)} ETH`;
};

export const formatPercentage = (value) => {
  return `${value}%`;
};

export const formatPropertyType = (type) => {
  const types = {
    'Residential': '🏠 Residential',
    'Commercial': '🏢 Commercial',
    'Industrial': '🏭 Industrial',
    'Land': '🌲 Land'
  };
  return types[type] || type;
};

export const calculateFundedPercentage = (totalUnits, availableUnits) => {
  if (!totalUnits || totalUnits === 0) return 0;
  const soldUnits = totalUnits - availableUnits;
  return Math.round((soldUnits / totalUnits) * 100);
};