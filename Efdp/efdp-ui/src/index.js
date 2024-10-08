import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { EFDP_API_BASE_URL, USE_MOCK_RESPONSES, YEARS_AGO } from './config';

console.log(`EFDP_API_BASE_URL:${EFDP_API_BASE_URL}`);
console.log(`USE_MOCK_RESPONSES:${USE_MOCK_RESPONSES}`);
console.log(`YEARS_AGO:${YEARS_AGO}`);

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
