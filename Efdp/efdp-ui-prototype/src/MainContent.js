// MainContent.js
import React from 'react';
import './MainContent.css';
import Area1 from './Area1';

function MainContent() {
  return (
    <div className="MainContent">
      <Area1></Area1>
      <div className="Area2"></div>
      <div className="Area3">Area 3</div>
    </div>
  );
}

export default MainContent;
