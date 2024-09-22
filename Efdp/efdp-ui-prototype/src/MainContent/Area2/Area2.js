import React from 'react';
import Comp1 from './Comp1'
import Comp2 from './Comp2'

function Area2() {
  const area2Style = {
    display: 'flex',
    flexDirection: 'row',
    justifyContent: 'flex-start',
    height: '40%',
    backgroundColor: '#d1e7d1',
    padding: '10px',
    marginBottom: '10px',
  };

  return (
    <div style={area2Style}>
      <Comp1/>
      <Comp2/>
      <Comp2/>
    </div>
  );
}

export default Area2;
