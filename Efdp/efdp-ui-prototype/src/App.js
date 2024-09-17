// App.js
import React from 'react';
import Banner from './Banner';
import MenuSidebar from './MenuSidebar';
import MainContent from './MainContent';
import Footer from './Footer';
import './App.css';

function App() {
  return (
    <div className="App">
      <Banner />
      <div className="MainLayout">
        <MenuSidebar />
        <MainContent />
      </div>
          {/* <div className="MainLayout">
          <MenuSidebar />
              <MainContent />
      </div> */}
      <Footer />
    </div>
  );
}

export default App;