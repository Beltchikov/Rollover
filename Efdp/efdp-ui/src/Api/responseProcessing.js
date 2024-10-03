export function createSymbolsTableBalanceSheet(balanceSheetStatementDict, selector) {
    // Extract and sort unique labels (dates)
    const uniqueLabels = Object.values(balanceSheetStatementDict)
        .flatMap(statements => statements.map(bs => bs.date))
        .filter((value, index, self) => self.indexOf(value) === index) // Unique values
        .sort(); // Sort the dates

    // Initialize the table with the header row
    const symbolsTable = [];

    // Create header with "Symbol" followed by the labels (dates)
    const header = ["Symbol", ...uniqueLabels].join("\t");
    symbolsTable.push(header);

    // Iterate over each stock symbol in the balanceSheetStatementDict
    for (const symbol in balanceSheetStatementDict) {
        // Start the row with the symbol name
        let row = [symbol];

        // Iterate over unique labels (dates)
        for (const label of uniqueLabels) {
            // Try to find the balance sheet statement for the current label (date)
            const balanceSheet = balanceSheetStatementDict[symbol].find(bs => bs.date === label);
            row.push(balanceSheet ? selector(balanceSheet) : "");
        }

        // Add the row to the table
        symbolsTable.push(row.join("\t"));
    }

    return symbolsTable;
}

export function interpolateSymbolsTable(symbolsTable) {
    const interpolatedTable = [symbolsTable[0]]; // Copy the header row

    // Iterate through each row (skip the header)
    for (let i = 1; i < symbolsTable.length; i++) {
        const row = symbolsTable[i].split("\t"); // Split the row into columns
        const symbol = row[0]; // First column is the symbol
        let dataColumns = row.slice(1); // The rest are retained earnings values

        // Convert data columns to numbers, treating empty strings as null
        let values = dataColumns.map(val => (val === "" ? null : Number(val)));

        // Perform interpolation (skip first and last columns)
        for (let j = 1; j < values.length - 1; j++) {
            if (values[j] === null) {
                // Find the previous and next non-null values
                let prevIndex = j - 1;
                let nextIndex = j + 1;

                // Find previous non-null value
                while (prevIndex >= 0 && values[prevIndex] === null) {
                    prevIndex--;
                }

                // Find next non-null value
                while (nextIndex < values.length && values[nextIndex] === null) {
                    nextIndex++;
                }

                // If both previous and next values exist, interpolate
                if (prevIndex >= 0 && nextIndex < values.length && values[prevIndex] !== null && values[nextIndex] !== null) {
                    const prevValue = values[prevIndex];
                    const nextValue = values[nextIndex];
                    const gap = nextIndex - prevIndex;

                    // Linear interpolation formula
                    values[j] = prevValue + ((nextValue - prevValue) * (j - prevIndex)) / gap;
                }
            }
        }

        // Rebuild the row with interpolated values
        const interpolatedRow = [symbol, ...values.map(v => (v !== null ? v : ""))].join("\t");
        interpolatedTable.push(interpolatedRow);
    }

    return interpolatedTable;
}

export function createChartData(interpolatedSymbolsTable, getRandomRgbColors) {
    // Get the color scheme for datasets (skip the header row)
    const colors = getRandomRgbColors(interpolatedSymbolsTable.length - 1);

    // Extract the labels (dates) from the first row of the table (skipping the "Symbol" column)
    const labels = interpolatedSymbolsTable[0].split('\t').slice(1); // Skip the first column ("Symbol")

    // Extract the datasets (starting from the second row)
    const datasets = interpolatedSymbolsTable.slice(1).map((row, index) => {
        const columns = row.split('\t');
        const symbol = columns[0];  // The first column is the stock symbol
        const data = columns.slice(1).map(val => (val === "" ? null : Number(val))); // Retained earnings values

        return {
            label: symbol,           // Stock symbol as dataset label
            data: data,              // Retained earnings values
            borderColor: colors[index], // Assign a random color to each dataset
            backgroundColor: colors[index].replace('1)', '0.2)'), // Use a transparent background color
            yAxisID: 'y-axis-1',
            hidden: false,
            borderWidth: 1
        };
    });

    // Return the chart data object with labels and datasets
    return {
        labels: labels,
        datasets: datasets
    };
}


