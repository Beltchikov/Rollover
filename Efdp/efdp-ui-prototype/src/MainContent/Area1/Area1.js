// Area1.js
import React from 'react';
import './Area1.css';
import SymbolsInput from './SymbolsInput';
import EfdpChart from '../Chart/EfdpChart'

function Area1() {
  return (
    <div className="Area1">
      <EfdpChart></EfdpChart>
      <SymbolsInput />
    </div>
  );
}

export default Area1;
