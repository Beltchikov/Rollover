import React, { useState } from 'react';
import './EfdpChart.css';
import LineChartComponent from './LineChartComponent'; 
import BarChartComponent from './BarChartComponent';

const EfdpChart = ({ type, title }) => {
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
    });

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

    const handleCheckboxChange = (index) => {
        const updatedDatasets = data.datasets.map((dataset, i) =>
            i === index ? { ...dataset, hidden: !dataset.hidden } : dataset
        );
        setData({ ...data, datasets: updatedDatasets });
    };

    return (
        <div className="EfdpChartContainer">
            {/* Title Section */}
            <div className="TitleSection">
                <label>{title}</label> {/* Dynamically renders the title prop */}
            </div>

            {/* Content Section */}
            <div className="ContentSection">
                {/* Checkbox Section */}
                <div className="CheckboxSection">
                    {data.datasets.map((dataset, index) => (
                        <label key={index}>
                            <input
                                type="checkbox"
                                checked={!dataset.hidden}
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
        </div>
    );
};

export default EfdpChart;
