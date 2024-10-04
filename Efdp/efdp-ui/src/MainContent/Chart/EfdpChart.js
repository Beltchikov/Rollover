import React from 'react';
import './EfdpChart.css';
import LineChartComponent from './LineChartComponent'; 
import BarChartComponent from './BarChartComponent';
import { useDispatch } from 'react-redux';
import { toggleDatasetVisibility } from '../../store.ts'; 

const EfdpChart = ({ type, title, data, areaKey, chartKey  }) => {
    const dispatch = useDispatch();

    const handleCheckboxChange = (index) => {
        dispatch(toggleDatasetVisibility({ areaKey, datasetKey: chartKey, datasetIndex: index }));
    };

    return (
        <div className="EfdpChartContainer">
            {/* Title Section */}
            <div className="TitleSection">
                <label>{title}</label>
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
                        <BarChartComponent data={data} options={{ responsive: true, maintainAspectRatio: false }} />
                    ) : (
                        <LineChartComponent data={data} options={{ responsive: true, maintainAspectRatio: false }} />
                    )}
                </div>
            </div>
        </div>
    );
};

export default EfdpChart;
