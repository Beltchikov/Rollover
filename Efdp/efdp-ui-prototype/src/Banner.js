// Banner.js
import React from 'react';
import './Banner.css';

function Banner() {
  // return <div className="Banner">Essential Finance Data Portal</div>;
  return <div className="Banner">
    {/* <image src="../public/EfdpTitle.png"></image> */}
    <img src={`${process.env.PUBLIC_URL}/EfdpTitle.png`} alt="Essential Finance Data Portal" className="BannerImage" />
    </div>;
}

export default Banner;
