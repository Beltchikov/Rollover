import React from 'react';
import { useSelector } from 'react-redux';
import SymbolsInput from './SymbolsInput';
import EfdpChart from './Chart/EfdpChart';
import AdTeaser from './AdTeaser';

function Area1() {
  const { dataCagrFcf, dataFcf } = useSelector((state) => state.global.area1);

  const area1Style = {
    display: 'flex',
    justifyContent: 'flex-start',
    backgroundColor: '#14141328',
    padding: '10px',
    marginBottom: '10px',
    gap: '10px',
  };

  const chartStyle = {
    width: '35%',
  };

  return (
    <div style={area1Style}>
      <SymbolsInput />
      
      {dataCagrFcf ? (
        <EfdpChart type={'bar'} title='Annual Growth of FCF' data={dataCagrFcf} areaKey="area1" chartKey="dataCagrFcf" style={chartStyle} />
      ) : (
        <div style={area1Style}>
          <div>Loading...</div>
          <AdTeaser />
        </div>
      )}

      {dataFcf ? (
        <EfdpChart title='Free Cash Flow (FCF)' data={dataFcf} areaKey="area1" chartKey="dataFcf" style={chartStyle} />
      ) : (
        <div style={area1Style}>
          <div>Loading...</div>
          <AdTeaser />
        </div>
      )}

      <AdTeaser  />
    </div>
  );
}

export default Area1;
