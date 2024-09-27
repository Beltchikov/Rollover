// MainContent.js
import React from 'react';
import Area1 from './Area1';
import Area2 from './Area2';
import Area3 from './Area3';

function MainContent() {
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
