import React from 'react';
import { useSelector } from 'react-redux';
import EfdpChart from './Chart/EfdpChart';
import AdTeaser from './AdTeaser';

function Area3() {
  const { dataLongTermDebtToFcf } = useSelector((state) => state.global.area3);

  const area3Style = {
    flexBasis: '33%',
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
      
      {dataLongTermDebtToFcf ? (
        <EfdpChart title='Long-Term Debt to FCF Ratio' data={dataLongTermDebtToFcf} areaKey="area3" chartKey="dataLongTermDebtToFcf" />
      ) : (
        <div style={area3Style}>
          <div>Loading...</div>
          <AdTeaser />
        </div>
      )}

      <span style={{ width: '10px' }} />
      <AdTeaser />
    </div>
  );
}

export default Area3;
