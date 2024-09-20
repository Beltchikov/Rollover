// SymbolsInput.js
import React, { useState } from 'react';
import './SymbolsInput.css';

function SymbolsInput() {
  const [inputText, setInputText] = useState('');

  const handleDrop = (event) => {
    event.preventDefault();
    const text = event.dataTransfer.getData('text/plain');
    if (text) {
      setInputText(text); // Replace the content with dropped text
    }
  };

  const handleDragOver = (event) => {
    event.preventDefault();
  };

  const handlePaste = (event) => {
    const pastedText = event.clipboardData.getData('text');
    if (pastedText) {
      setInputText(pastedText); // Replace the content with pasted text
    }
  };

  return (
    <div
      className="SymbolsInput"
      onDrop={handleDrop}
      onDragOver={handleDragOver}
      contentEditable={false} // Disable text input but keep it editable via drag and paste
      onPaste={handlePaste}
    >
      {inputText ? inputText : "Drag and drop or paste your stock symbols below - one symbol per line."}
    </div>
  );
}

export default SymbolsInput;
