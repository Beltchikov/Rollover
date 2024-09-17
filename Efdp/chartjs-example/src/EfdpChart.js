// src/EfdpChart.js
import React from 'react';
import ChartComponent from './ChartComponent'; // Import the existing ChartComponent

const EfdpChart = ({data}, {options}) => {
 return (
    <div>
     <ChartComponent data={data} options={options} />
    </div>
  );
};

export default EfdpChart;
