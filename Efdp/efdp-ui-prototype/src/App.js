// import logo from './logo.svg';
// import './App.css';

// function App() {
//   return (
//     <div className="App">
//       <header className="App-header">
//         <img src={logo} className="App-logo" alt="logo" />
//         <p>
//           Edit <code>src/App.js</code> and save to reload.
//         </p>
//         <a
//           className="App-link"
//           href="https://reactjs.org"
//           target="_blank"
//           rel="noopener noreferrer"
//         >
//           Learn React
//         </a>
//       </header>
//     </div>
//   );
// }

// export default App;

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

