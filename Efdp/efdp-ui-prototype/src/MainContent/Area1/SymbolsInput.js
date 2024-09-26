import React, { useState, useEffect, useRef } from 'react';

function SymbolsInput() {
  const SymbolsInputStyle = {
    backgroundColor: 'gainsboro',
    boxShadow: '0px 4px 8px rgba(0, 0, 0, 0.2)',
    display: 'flex',
    flexDirection: 'column',
    width: 'auto' // Initially auto width
  };

  const SymbolsInputLabel = {
    display: 'block',
    textAlign: 'left',
    fontSize: '12px'
  };

  const SymbolsInputDiv = {
    textAlign: 'left',
    fontSize: '12px',
    backgroundColor: 'white',
    flex: 1,
    border: '2px dashed black',
    whiteSpace: 'pre-wrap'
  };

  const [inputText, setInputText] = useState('NVDA\nMSFT\nGOOG');
  const [inputWidth, setInputWidth] = useState('auto');
  const hiddenDivRef = useRef(null);

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

  // Measure the longest line and set the width dynamically
  useEffect(() => {
    if (hiddenDivRef.current) {
      const width = hiddenDivRef.current.offsetWidth;
      const adjustedWidth = Math.max(width + 10, 150); 
      setInputWidth(`${adjustedWidth}px`);
    }
  }, [inputText]);
  

  return (
    <div style={{ ...SymbolsInputStyle, width: inputWidth }}>
      <label style={SymbolsInputLabel}>
        Drag and drop or paste stock symbols below:
      </label>
      <div
        style={SymbolsInputDiv}
        onDrop={handleDrop}
        onDragOver={handleDragOver}
        onPaste={handlePaste}
      >
        {inputText ? inputText : ""}
      </div>
      {/* Hidden div to calculate the width of the longest line */}
      <div
        ref={hiddenDivRef}
        style={{
          position: 'absolute',
          visibility: 'hidden',
          whiteSpace: 'pre-wrap',
          fontSize: '12px',
          fontFamily: 'inherit'
        }}
      >
        {inputText}
      </div>
    </div>
  );
}

export default SymbolsInput;
