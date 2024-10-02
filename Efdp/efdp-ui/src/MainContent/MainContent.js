// MainContent.js
import React, { useEffect } from 'react';
import Area1 from './Area1';
import Area2 from './Area2';
import Area3 from './Area3';
import { useSelector, useDispatch } from 'react-redux';
import { fetchAllData } from '../store';

function MainContent() {
  const dispatch = useDispatch();

  // Get the symbolsInput from the Redux store
  const symbolsInput = useSelector((state) => state.global.symbolsInput);

// Use useEffect to dispatch fetchRetainedEarnings after any change in symbolsInput
useEffect(() => {
  if (symbolsInput) {  // Ensure symbolsInput is not an empty string
      dispatch(fetchAllData());
  }
}, [symbolsInput, dispatch]); // Add symbolsInput as a dependency

  const mainContentStyle = {
    display: 'flex',
    flexDirection: 'column',
    flex: 1,
    backgroundColor: '#e0f0d9',
    padding: '10px',
  };

  return (
    <div className="MainContent" style={mainContentStyle}>
      <Area1 />
      <Area2 />
      <Area3 />
    </div>
  );
}

export default MainContent;
