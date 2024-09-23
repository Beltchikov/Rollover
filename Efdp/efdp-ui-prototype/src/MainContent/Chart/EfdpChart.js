import React, { useState } from 'react';
import './EfdpChart.css'; // Import the extracted CSS
import LineChartComponent from './LineChartComponent'; 
import BarChartComponent from './BarChartComponent';

const EfdpChart = ({type} ) => {
    // State for data
    const [data, setData] = useState({
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
                borderWidth: 1, // Set line width to 1px
            },
            {
                label: 'GOOG',
                data: [16348000000, 17913000000, 19478000000, 16070000000, 21699000000],
                borderColor: 'rgba(54, 162, 235, 1)',
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                yAxisID: 'y-axis-1',
                hidden: false,
                borderWidth: 1, // Set line width to 1px
            },
            // Continue this process for MSFT, AAPL, AMZN, META, TSLA
        ]
    });

    // State for chart options
    const [options, setOptions] = useState({
        responsive: true,
        scales: {
            yAxes: [
                {
                    id: 'y-axis-1',
                    type: 'linear',
                    position: 'left',
                    ticks: {
                        beginAtZero: true,
                    },
                },
            ],
        },
    });

    // Handle checkbox change to show/hide datasets
    const handleCheckboxChange = (index) => {
        // Create a new datasets array with updated 'hidden' values
        const updatedDatasets = data.datasets.map((dataset, i) =>
            i === index ? { ...dataset, hidden: !dataset.hidden } : dataset
        );

        // Update the data state with a new object reference
        setData({ ...data, datasets: updatedDatasets });
    };

    return (
        <div className="EfdpChartContainer">
            {/* Checkbox Section */}
            <div className="CheckboxSection">
                {data.datasets.map((dataset, index) => (
                    <label key={index}>
                        <input
                            type="checkbox"
                            checked={!dataset.hidden} // If dataset is not hidden, checkbox is checked
                            onChange={() => handleCheckboxChange(index)}
                        />
                        {dataset.label}
                    </label>
                ))}
            </div>

            {/* Chart Section */}
            <div className="ChartSection">
                {type === 'bar' ? (
                    <BarChartComponent data={data} options={options} />
                ) : (
                    <LineChartComponent data={data} options={options} />
                )}
            </div>
        </div>
    );
};

export default EfdpChart;
