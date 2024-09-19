// SymbolsInput.js
import React, { useState } from 'react';
import './SymbolsInput.css';

function SymbolsInput() {
  const [inputText, setInputText] = useState('');

  const handleDrop = (event) => {
    event.preventDefault();
    const text = event.dataTransfer.getData('text/plain');
    if (text) {
      setInputText(text);
      alert(text); // Show the input in an alert box
    }
  };

  const handleDragOver = (event) => {
    event.preventDefault();
  };

  return (
    <div
      className="SymbolsInput"
      onDrop={handleDrop}
      onDragOver={handleDragOver}
    >
      Drag and drop a multiline string here
    </div>
  );
}

export default SymbolsInput;
