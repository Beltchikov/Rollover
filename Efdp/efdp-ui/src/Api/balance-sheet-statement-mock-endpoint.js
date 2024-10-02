import axios from 'axios';

export const fetchBalanceSheetStatementMockData = (baseUrl) => {
    return new Promise((resolve, reject) => {
        const url = `${baseUrl}/balance-sheet-statement-mock`;
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

