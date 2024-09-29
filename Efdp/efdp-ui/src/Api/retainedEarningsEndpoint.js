import axios from 'axios';

export const fetchRetainedEarningsData = (stockSymbols) => {
    return new Promise((resolve, reject) => {
        const BASE_URL = '';
        //const BASE_URL = 'http://localhost:5266';
        const query = stockSymbols.map(symbol => `stockSymbols=${encodeURIComponent(symbol)}`).join('&');
        const url = `${BASE_URL}/retainedEarnings?${query}`;
        //const url = `/retainedEarnings?${query}`;
        console.log(url);
        
        axios.get(url)
        .then(response => {
            console.log(response.data);
            resolve(response.data); // Resolve with the data from axios
        })
        .catch(error => {
            console.log(error);
            reject(error); // Reject in case of an error
        });
    });
};

