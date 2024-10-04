// App.js
import React from 'react';
import { Provider } from 'react-redux';
import { store } from './store.ts';  // Import the store
import Banner from './Banner';
import MainContent from './MainContent/MainContent';
import Footer from './Footer';
import './App.css';
import Info from './Info';

function App() {
  return (
    <Provider store={store}>
      <div className="App">
        <Banner />
        <Info />
        <div className="MainLayout">
          <MainContent />
        </div>
        <Footer />
      </div>
    </Provider>
  );
}

export default App;
