import axios from 'axios';

export const fetchCashFlowStatementData = (stockSymbols, baseUrl) => {
    return new Promise((resolve, reject) => {
        const query = stockSymbols.map(symbol => `stockSymbols=${encodeURIComponent(symbol)}`).join('&');
        const url = `${baseUrl}/cash-flow-statement?${query}`;
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

