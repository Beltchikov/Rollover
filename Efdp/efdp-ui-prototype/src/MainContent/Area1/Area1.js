import React from 'react';
import SymbolsInput from './SymbolsInput';
import EfdpChart from '../Chart/EfdpChart';
import AdTeaser from '../AdTeaser';

function Area1() {
  const area1Style = {
    display: 'flex',
    justifyContent: 'flex-start',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
    gap: '10px',  // Use gap for spacing instead of individual spans
  };

  const chartStyle = {
    width: '35%',  // Fixed 35% width for the charts
  };

  const adTeaserStyle = {
    flexGrow: 1,  // Let AdTeaser take the remaining space
  };

  return (
    <div style={area1Style}>
      {/* SymbolsInput will have auto width */}
      <SymbolsInput />

      {/* EfdpChart components with 35% width */}
      <EfdpChart type={'bar'} title='Annual Growth of FCF' style={chartStyle} />
      <EfdpChart title='Free Cash Flow' style={chartStyle} />

      {/* AdTeaser takes the remaining space */}
      <AdTeaser style={adTeaserStyle} />
    </div>
  );
}

export default Area1;
