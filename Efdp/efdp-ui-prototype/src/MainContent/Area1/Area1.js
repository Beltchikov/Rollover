// Area1.js
import React from 'react';
import './Area1.css';
import SymbolsInput from './SymbolsInput';
import EfdpChart from '../Chart/EfdpChart'; // Assuming you have this component

function Area1() {
  return (
    <div className="Area1">
      <SymbolsInput />
      <EfdpChart />
    </div>
  );
}

export default Area1;
