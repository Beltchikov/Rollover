import React from 'react';

function AdTeaser() {

  const AdTeaser = {
    flexBasis: '20%',
    backgroundColor: 'white',
    whiteSpace: 'pre-wrap'
  };

  const AdTeaserLabel = {
    display: 'block',
    textAlign: 'center',
    // verticalAlign:'middle',
    fontSize: '20px'
  };

  return (
    <div style={AdTeaser}>
      <label style={AdTeaserLabel}>
        Your ad could be catching eyes right here!
      </label>
      <div style={{ height: '10px' }}></div>
    </div>
  );
}

export default AdTeaser;
