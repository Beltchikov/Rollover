import React from 'react';

// Define keyframes for rotate-scale-up effect using a <style> element
const styles = `
  @keyframes rotateScaleUp {
    0% {
      transform: scale(1) rotate(0deg);
    }
    25% {
      transform: scale(0.5) rotate(360deg);
    }
    50% {
      transform: scale(1) rotate(0deg);
    }
    100% {
      transform: scale(1) rotate(0deg); /* No change here to create the pause */
    }
  }
`;

function AdTeaser() {

  const AdTeaser = {
    backgroundColor: 'white',
    whiteSpace: 'pre-wrap',
    display: 'flex', // Use flexbox
    justifyContent: 'center', // Horizontally center the label
    alignItems: 'center', // Vertically center the label
  };

  const AdTeaserLabel = {
    display: 'block',
    textAlign: 'center',
    fontSize: '20px',
    // The animation runs for 14s (4s animation + 10s pause)
    animation: 'rotateScaleUp 14s ease-in-out infinite',
  };

  return (
    <div style={AdTeaser}>
      {/* Inject keyframes using a <style> element */}
      <style>{styles}</style>
      <label style={AdTeaserLabel}>
        Your ad could be catching eyes right here!
      </label>
      <div style={{ height: '10px' }}></div>
    </div>
  );
}

export default AdTeaser;
