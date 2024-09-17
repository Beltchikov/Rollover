// MainContent.js
import React from 'react';
import './MainContent.css';

function MainContent() {
  return (
    <div className="MainContent">
      <div className="Area1">Area 1</div>
      <div className="Area2">
        <div className="Area2Section">Area 2 Section 1</div>
        <div className="Area2Section">Area 2 Section 2</div>
        <div className="Area2Section">Area 2 Section 3</div>
      </div>
      <div className="Area3">Area 3</div>
    </div>
  );
}

export default MainContent;
