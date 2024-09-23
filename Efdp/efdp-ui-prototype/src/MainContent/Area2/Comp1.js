import React from 'react';

function Comp1() {
  const Comp1Style = {
    flexGrow: 1,
    flexBasis: '10%',
    backgroundColor: 'gainsboro',
    boxShadow: '0px 4px 8px rgba(0, 0, 0, 0.2)'
  };

  const SymbolsInputLabel = {
    display: 'block',
    textAlign: 'left',
    fontSize: '12px'
  };

  return (
    <div style={Comp1Style}>
      <label style={SymbolsInputLabel}>
        Drag and drop or paste your stock symbols below - one symbol per line:
      </label>
    </div>
  );
}

export default Comp1;
