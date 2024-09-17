// src/ChartComponent.js
import React from 'react';
import { Line } from 'react-chartjs-2';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';

// Register necessary components with Chart.js
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
);

const ChartComponent = ({ data }) => {
  
  // Options for the multi-axis chart
  const options = {
    scales: {
      'y-axis-1': {
        type: 'linear',
        position: 'left',
        beginAtZero: true,
        ticks: {
          callback: (value) => `${value} $`, // Custom label formatting (optional)
        },
      },
    //   'y-axis-2': {
    //     type: 'linear',
    //     position: 'right',
    //     beginAtZero: true,
    //     grid: {
    //       drawOnChartArea: false, // Optional: Do not draw grid lines on the right axis
    //     },
    //  },
    },
  };

  // Render the Line chart with multi-axis
  return <Line data={data} options={options} />;
};

export default ChartComponent;
