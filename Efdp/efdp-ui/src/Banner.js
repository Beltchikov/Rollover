// Banner.js
import React from 'react';

function Banner() {
  const bannerStyle = {
    backgroundColor: '#14141328',
    height: '8%',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    textAlign: 'center',
  };

  const bannerImageStyle = {
    maxWidth: '100%',
    height: 'auto',
  };

  return (
    <div style={bannerStyle}>
      <img
        src={`${process.env.PUBLIC_URL}/EfdpTitle.png`}
        alt="Essential Finance Data Portal"
        style={bannerImageStyle}
      />
    </div>
  );
}

export default Banner;
