// MainContent.js
import React from 'react';
import './MainContent.css';
import Area1 from '../Area1';
import Area2 from '../Area2';
import Area3 from '../Area3';

function MainContent() {
  return (
    <div className="MainContent">
      <Area1></Area1>
      <Area2></Area2>
      <Area3></Area3>
    </div>
  );
}

export default MainContent;
