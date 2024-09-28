import React, { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import EfdpChart from './Chart/EfdpChart';
import { fetchRetainedEarnings } from '../store';
import AdTeaser from './AdTeaser';

function Area2() {
  const dispatch = useDispatch();

  // Fetch retained earnings data when the component is mounted
  useEffect(() => {
    dispatch(fetchRetainedEarnings());
  }, [dispatch]);

  const { dataFcfCapExRatio, dataRetainedEarnings, dataGpm } = useSelector(
    (state) => state.global.area2
  );

  const area2Style = {
    flexBasis: '33%',
    display: 'flex',
    justifyContent: 'flex-start',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
    gap: '10px'
  };

  const chartStyle = {
    flex: '1 1 30%',  // Each chart takes up 30% of the width
    margin: '0 10px',  // Adds some spacing between charts
  };

  return (
    <div style={area2Style}>
      <EfdpChart title="FCF-CapEx-Ratio" data={dataFcfCapExRatio} areaKey="area2" chartKey="dataFcfCapExRatio" style={chartStyle}/>
      {dataRetainedEarnings ? (
        <EfdpChart title="Retained Earnings" data={dataRetainedEarnings} areaKey="area2" chartKey="dataRetainedEarnings" style={chartStyle}/>
      ) : (
        <div style={area2Style}>
          <div>Loading...</div>
          <AdTeaser />  
        </div>
      )}
      <EfdpChart title="Gross Profit Margin" data={dataGpm} areaKey="area2" chartKey="dataGpm" style={chartStyle}/>
    </div>
  );
}

export default Area2;
