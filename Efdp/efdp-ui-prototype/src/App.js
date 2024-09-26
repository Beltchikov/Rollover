// App.js
import React from 'react';
import Banner from './Banner';
import MainContent from './MainContent/MainContent';
import Footer from './Footer';
import './App.css';
import Info from './Info';

function App() {
  return (
    <div className="App">
      <Banner />
      <Info />
      <div className="MainLayout">
        <MainContent />
      </div>
      <Footer />
    </div>
  );
}

export default App;