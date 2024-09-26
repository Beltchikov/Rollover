// App.js
import React from 'react';
import Banner from './Banner';
import Menu from './Menu';
// import MenuSidebar from './MenuSidebar';
import MainContent from './MainContent/MainContent';
import Footer from './Footer';
import './App.css';

function App() {
  return (
    <div className="App">
      <Banner />
      <Menu />
      <div className="MainLayout">
        {/* <MenuSidebar /> */}
        <MainContent />
      </div>
      <Footer />
    </div>
  );
}

export default App;