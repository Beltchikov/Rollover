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

const ChartComponent = () => {
  // Data for the multi-axis chart
  const data = {
    labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
    datasets: [
      {
        label: 'Dataset 1 (Left Axis)',
        data: [65, 59, 80, 81, 56, 55, 40],
        borderColor: 'rgba(255, 99, 132, 1)',
        backgroundColor: 'rgba(255, 99, 132, 0.2)',
        yAxisID: 'y-axis-1', // Specifies the axis for this dataset
      },
      {
        label: 'Dataset 2 (Right Axis)',
        data: [28, 48, 40, 19, 86, 27, 90],
        borderColor: 'rgba(54, 162, 235, 1)',
        backgroundColor: 'rgba(54, 162, 235, 0.2)',
        yAxisID: 'y-axis-2', // Specifies the axis for this dataset
      },
    ],
  };

  // Options for the multi-axis chart
  const options = {
    scales: {
      'y-axis-1': { // Configuration for the left Y-axis
        type: 'linear',
        position: 'left',
        beginAtZero: true,
        ticks: {
          callback: (value) => `${value} units`, // Custom label formatting (optional)
        },
      },
      'y-axis-2': { // Configuration for the right Y-axis
        type: 'linear',
        position: 'right',
        beginAtZero: true,
        grid: {
          drawOnChartArea: false, // Optional: Do not draw grid lines on the right axis
        },
      },
    },
  };

  // Render the Line chart with multi-axis
  return <Line data={data} options={options} />;
};

export default ChartComponent;
