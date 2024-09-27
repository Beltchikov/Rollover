// Area2.js
import React from 'react';
import { useSelector } from 'react-redux';
import EfdpChart from './Chart/EfdpChart';

function Area2() {
  const { dataFcfCapExRatio, dataRetainedEarnings, dataGpm } = useSelector((state) => state.global.area2);

  const area2Style = {
    flexBasis: '33%',
    display: 'flex',
    justifyContent: 'flex-start',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
  };

  return (
    <div style={area2Style}>
      <EfdpChart title='FCF-CapEx-Ratio' data={dataFcfCapExRatio} />
      <EfdpChart title='Retained Earnings' data={dataRetainedEarnings} />
      <EfdpChart title='Gross Profit Margin' data={dataGpm} />
    </div>
  );
}

export default Area2;
