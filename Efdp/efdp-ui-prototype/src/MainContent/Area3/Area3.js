// Area3.js
import React from 'react';
import './Area3.css';
import EfdpChart from '../Chart/EfdpChart';
import AdTeaser from '../AdTeaser';

function Area3() {
  return (
    <div className="Area3">
      <AdTeaser />
      <span style={{ width: '10px' }} />
      {/* <EfdpChart title='Long-Term Debt to Earnings Ratio' /> */}
    </div>
  );
}

export default Area3;
