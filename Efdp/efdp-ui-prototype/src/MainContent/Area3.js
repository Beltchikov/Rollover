import React from 'react';
import EfdpChart from './Chart/EfdpChart';
import AdTeaser from './AdTeaser';

const dataForDebtToEarnings = {
  labels: [
      '2009-09-26', '2009-12-31', '2010-06-30', '2010-09-25', '2010-12-31',
      // other labels...
  ],
  datasets: [
      {
          label: 'NVDA',
          data: [253146000, 417118000, 581090000, 571813000, 562536000],
          borderColor: 'rgba(255, 99, 132, 1)',
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          yAxisID: 'y-axis-1',
          hidden: false,
          borderWidth: 1,
      },
      {
          label: 'GOOG',
          data: [16348000000, 17913000000, 19478000000, 16070000000, 21699000000],
          borderColor: 'rgba(54, 162, 235, 1)',
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          yAxisID: 'y-axis-1',
          hidden: false,
          borderWidth: 1,
      },
  ]
};

function Area3() {
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
      <EfdpChart title='Long-Term Debt to Earnings Ratio' data={dataForDebtToEarnings} />
      <span style={{ width: '10px' }} />
      <AdTeaser />
    </div>
  );
}

export default Area3;
