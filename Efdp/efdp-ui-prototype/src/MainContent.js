// MainContent.js
import React from 'react';
import './MainContent.css';
import Area1 from './Area1';
import Area2 from './Area2';

function MainContent() {
  return (
    <div className="MainContent">
      <Area1></Area1>
      <Area2></Area2>
      <div className="Area3">Area 3</div>
    </div>
  );
}

export default MainContent;
