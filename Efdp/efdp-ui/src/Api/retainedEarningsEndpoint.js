// retainedEarningsEndpoint.js

export const fetchRetainedEarningsData = (stockSymbols) => {
    return new Promise((resolve, reject) => {
        // Build the query string from the stockSymbols array
        const query = stockSymbols.map(symbol => `stockSymbols=${encodeURIComponent(symbol)}`).join('&');

        // Fetch data from the provided API
        fetch(`http://localhost:5266/retainedEarnings?${query}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                resolve(data); // Resolve the promise with the fetched data
            })
            .catch(error => {
                reject(error); // Reject the promise in case of an error
            });
    });
};

  