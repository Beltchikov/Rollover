import React from 'react';
import { useSelector } from 'react-redux';
import EfdpChart from './Chart/EfdpChart';
import AdTeaser from './AdTeaser';

function Area2() {
  // Get the symbolsInput from the Redux store
  const symbolsInput = useSelector((state) => state.global.symbolsInput);

 const { dataFcfCapExRatio, dataRetainedEarnings, dataGpm } = useSelector(
    (state) => state.global.area2
  );

  const area2Style = {
    flexBasis: '33%',
    display: 'flex',
    justifyContent: 'flex-start',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
    gap: '10px'
  };

  const chartStyle = {
    flex: '1 1 30%',  // Each chart takes up 30% of the width
    margin: '0 10px',  // Adds some spacing between charts
  };

  return (
    <div style={area2Style}>
      {dataFcfCapExRatio ? (
        <EfdpChart title="FCF-CapEx-Ratio" data={dataFcfCapExRatio} areaKey="area2" chartKey="dataFcfCapExRatio" style={chartStyle}/>
      ) : (
        <div style={area2Style}>
          <div>Loading...</div>
          <AdTeaser />  
        </div>
      )}

      {dataRetainedEarnings ? (
        <EfdpChart title="Retained Earnings" data={dataRetainedEarnings} areaKey="area2" chartKey="dataRetainedEarnings" style={chartStyle}/>
      ) : (
        <div style={area2Style}>
          <div>Loading...</div>
          <AdTeaser />  
        </div>
      )}

      {dataGpm ? (
        <EfdpChart title="Gross Profit Margin" data={dataGpm} areaKey="area2" chartKey="dataGpm" style={chartStyle}/>
      ) : (
        <div style={area2Style}>
          <div>Loading...</div>
          <AdTeaser />  
        </div>
      )}
    </div>
  );
}

export default Area2;
