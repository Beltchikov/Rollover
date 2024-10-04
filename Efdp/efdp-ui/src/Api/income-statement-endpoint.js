import axios from 'axios';

export const fetchIncomeStatementData = (stockSymbols, baseUrl) => {
    return new Promise((resolve, reject) => {
        const query = stockSymbols.map(symbol => `stockSymbols=${encodeURIComponent(symbol)}`).join('&');
        const url = `${baseUrl}/income-statement?${query}`;
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

