import axios from 'axios';

export const fetchRetainedEarningsData = (stockSymbols, baseUrl) => {
    return new Promise((resolve, reject) => {
        const query = stockSymbols.map(symbol => `stockSymbols=${encodeURIComponent(symbol)}`).join('&');
        const url = `${baseUrl}/retainedEarnings?${query}`;
        axios.get(url)
        .then(response => {
            resolve(response.data); // Resolve with the data from axios
        })
        .catch(error => {
            console.log(error);
            reject(error); // Reject in case of an error
        });
    });
};

