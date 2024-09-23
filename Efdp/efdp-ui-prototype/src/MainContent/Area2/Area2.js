import React from 'react';
import Comp1 from '../Area1/SymbolsInput'
import Comp2 from './Comp2'
import Comp3 from './Comp3'

function Area2() {
  const area2Style = {
    display: 'flex',
    justifyContent: 'flex-start',
    height: '40%',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
  };

  return (
    <div style={area2Style}>
      <Comp1/>
      <span style={{width:'10px'}}/>
      <Comp2/>
      <span style={{width:'10px'}}/>
      <Comp2/>
      <span style={{width:'10px'}}/>
      <Comp3/>
    </div>
  );
}

export default Area2;
