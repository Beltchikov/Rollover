import React from 'react';
import SymbolsInput from './SymbolsInput';
import EfdpChart from '../Chart/EfdpChart';
import Comp3 from '../Area2/Comp3'

function Area1() {
  const area1Style = {
    display: 'flex',
    justifyContent: 'flex-start',
    height: '40%',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
  };

  return (
    <div style={area1Style}>
      <SymbolsInput />
      <span style={{ width: '10px' }} />
      <EfdpChart />
      <span style={{ width: '10px' }} />
      <EfdpChart />
      <span style={{ width: '10px' }} />
      <Comp3 />
    </div>
  );
}

export default Area1;
