import React from 'react';
import EfdpChart from '../Chart/EfdpChart';

function Area2() {
  const area2Style = {
    flexBasis:'33%',
    display: 'flex',
    justifyContent: 'flex-start',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
  };

  return (
    <div style={area2Style}>
      <EfdpChart title='FCF-CapEx-Ratio' />
      <span style={{ width: '10px' }} />
      <EfdpChart title='Retained Earnings' />
      <span style={{ width: '10px' }} />
      <EfdpChart title='Gross Profit Margin' />
    </div>
  );
}

export default Area2;
