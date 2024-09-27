import React from 'react';
import EfdpChart from './Chart/EfdpChart';

const dataFcfCapExRatio = {
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

const dataRetainedEarnings = {
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

const dataGpm = {
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

function Area2() {
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
      <span style={{ width: '10px' }} />
      <EfdpChart title='Retained Earnings' data={dataRetainedEarnings} />
      <span style={{ width: '10px' }} />
      <EfdpChart title='Gross Profit Margin' data={dataGpm} />
    </div>
  );
}

export default Area2;
