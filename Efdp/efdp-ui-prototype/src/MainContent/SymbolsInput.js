// SymbolsInput.js
import React, { useEffect, useRef } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { updateSymbolsInput } from '../store';

function SymbolsInput() {
  const dispatch = useDispatch();
  const inputText = useSelector((state) => state.global.symbolsInput);  // Access global state

  const SymbolsInputStyle = {
    backgroundColor: 'gainsboro',
    display: 'flex',
    flexDirection: 'column',
    width: 'auto',
  };

  const SymbolsInputLabel = {
    display: 'block',
    textAlign: 'left',
    fontSize: '12px',
  };

  const SymbolsInputDiv = {
    textAlign: 'left',
    fontSize: '12px',
    backgroundColor: 'white',
    flex: 1,
    border: '2px dashed black',
    whiteSpace: 'pre-wrap',
  };

  const hiddenDivRef = useRef(null);
  const [inputWidth, setInputWidth] = React.useState('auto');

  const handleDrop = (event) => {
    event.preventDefault();
    const text = event.dataTransfer.getData('text/plain');
    if (text) {
      dispatch(updateSymbolsInput(text));
    }
  };

  const handleDragOver = (event) => {
    event.preventDefault();
  };

  const handlePaste = (event) => {
    const pastedText = event.clipboardData.getData('text');
    if (pastedText) {
      dispatch(updateSymbolsInput(pastedText));
    }
  };

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
        {inputText}
      </div>
      <div
        ref={hiddenDivRef}
        style={{
          position: 'absolute',
          visibility: 'hidden',
          whiteSpace: 'pre-wrap',
          fontSize: '12px',
        }}
      >
        {inputText}
      </div>
    </div>
  );
}

export default SymbolsInput;
