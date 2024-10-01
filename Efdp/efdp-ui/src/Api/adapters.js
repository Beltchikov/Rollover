// Adapter function to convert balanceSheetStatementDict to the format needed for the chart
export const retainedEarningAdapter = (balanceSheetStatementDict, getRandomColor) => {
    // Initialize the labels and datasets
    const labelsSet = new Set(); // To ensure unique labels (dates)
    const datasets = [];

    // Loop through each symbol in the balance sheet statement dictionary
    Object.entries(balanceSheetStatementDict).forEach(([symbol, balanceSheets]) => {
        
        // Extract retained earnings and dates
        const retainedEarnings = [];
        balanceSheets.forEach(balanceSheet => {
            if (balanceSheet.retainedEarnings !== undefined && balanceSheet.date) {
                retainedEarnings.push(balanceSheet.retainedEarnings);
                labelsSet.add(balanceSheet.date); // Add date to labels set
            }
        });

        // Create a dataset for each symbol
        datasets.push({
            label: symbol,
            data: retainedEarnings,
            borderColor: getRandomColor(), // Use passed getRandomColor function for the border color
            backgroundColor: getRandomColor(true), // Use passed getRandomColor function for the background color
            yAxisID: 'y-axis-1',
            hidden: false,
            borderWidth: 1,
        });
    });

    // Convert the labels set into a sorted array
    const labels = Array.from(labelsSet).sort();

    // Return the data in the format expected by the chart
    return {
        labels: labels,
        datasets: datasets,
    };
};
