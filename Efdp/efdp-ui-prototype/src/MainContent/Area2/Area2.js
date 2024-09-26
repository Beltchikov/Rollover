import React from 'react';
import EfdpChart from '../Chart/EfdpChart';
import Comp3 from './Comp3'

function Area2() {
  const area2Style = {
    display: 'flex',
    justifyContent: 'flex-start',
    height: '40%',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
  };

  return (
    <div style={area2Style}>
      <EfdpChart title='FCF-CapEx-Ratio' />
      <span style={{width:'10px'}}/>
      <EfdpChart title='Retained Earnings' />
      <span style={{width:'10px'}}/>
      <EfdpChart title='Gross Profit Margin' />
      <span style={{width:'10px'}}/>
      <Comp3/>
    </div>
  );
}

export default Area2;
