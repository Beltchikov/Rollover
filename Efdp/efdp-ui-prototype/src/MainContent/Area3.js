// Area3.js
import React from 'react';
import EfdpChart from './Chart/EfdpChart';
import AdTeaser from './AdTeaser';

function Area3() {
  const area3Style = {
    flexBasis:'33%',
    display: 'flex',
    flexDirection: 'row',
    justifyContent: 'space-between',
    backgroundColor: '#d1e7d1',
    padding: '10px',
  };

  return (
    <div style={area3Style}>
      <AdTeaser />
      <span style={{ width: '10px' }} />
      <EfdpChart title='Long-Term Debt to Earnings Ratio' />
      <span style={{ width: '10px' }} />
      <AdTeaser />
    </div>
  );
}

export default Area3;
