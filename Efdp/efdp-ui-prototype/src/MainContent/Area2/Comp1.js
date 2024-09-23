import React, { useState } from 'react';

function Comp1() {
  const Comp1Style = {
    flexGrow: 1,
    flexBasis: '10%',
    backgroundColor: 'gainsboro',
    boxShadow: '0px 4px 8px rgba(0, 0, 0, 0.2)',
    display:'flex',
    flexDirection:'column'
  };

  const SymbolsInputLabel = {
    display: 'block',
    textAlign: 'left',
    fontSize: '12px'
  };

  const SymbolsInput = {
    textAlign: 'left',
    fontSize: '12px',
    backgroundColor: 'white',
    height:'100%',
    border: '2px dashed black',
    whiteSpace: 'pre-wrap'
  };

  const [inputText, setInputText] = useState('NVDA\nMSFT\nGOOG');

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
    <div style={Comp1Style}>
      <label style={SymbolsInputLabel}>
        Drag and drop or paste stock symbols below:
      </label>
      <div style={{height:'10px'}}></div>
      <div
        style={SymbolsInput}
        onDrop={handleDrop}
        onDragOver={handleDragOver}
        // contentEditable={false} 
        onPaste={handlePaste}
      >
        {inputText ? inputText : ""}
      </div>
    </div>
  );
}

export default Comp1;
