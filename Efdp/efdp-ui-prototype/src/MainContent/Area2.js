import React, { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import EfdpChart from './Chart/EfdpChart';
import { fetchRetainedEarnings } from '../store';

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

  return (
    <div style={area2Style}>
      <EfdpChart title="FCF-CapEx-Ratio" data={dataFcfCapExRatio} />
      {dataRetainedEarnings ? (
        <EfdpChart title="Retained Earnings" data={dataRetainedEarnings} />
      ) : (
        <div style={area2Style}>Loading...</div>
      )}
      <EfdpChart title="Gross Profit Margin" data={dataGpm} />
    </div>
  );
}

export default Area2;
