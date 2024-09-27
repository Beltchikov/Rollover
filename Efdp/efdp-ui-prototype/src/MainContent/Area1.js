import React from 'react';
import SymbolsInput from './SymbolsInput';
import EfdpChart from './Chart/EfdpChart';
import AdTeaser from './AdTeaser';

const dataForChart1 = {
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

const dataForChart2 = {
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

function Area1() {
  const area1Style = {
    display: 'flex',
    justifyContent: 'flex-start',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
    gap: '10px',
  };

  const chartStyle = {
    width: '35%',
  };

  const adTeaserStyle = {
    flexGrow: 1,
  };

  return (
    <div style={area1Style}>
      <SymbolsInput />
      <EfdpChart type={'bar'} title='Annual Growth of FCF' data={dataForChart1} style={chartStyle} />
      <EfdpChart title='Free Cash Flow' data={dataForChart2} style={chartStyle} />
      <AdTeaser style={adTeaserStyle} />
    </div>
  );
}

export default Area1;
