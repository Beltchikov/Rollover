// fakeApi.js

export const fetchRetainedEarningsData = () => {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve({
            labels: [
                '2009-09-26', '2009-12-31', '2010-06-30', '2010-09-25', '2010-12-31',
                // other labels...
            ],
            datasets: [
                {
                    label: 'NVDA',
                    data: [253146000, 417118000, 581090000, 571813000, 562536000],
                    borderColor: 'rgba(255, 99, 132, 1)',
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    yAxisID: 'y-axis-1',
                    hidden: false,
                    borderWidth: 1,
                },
                {
                    label: 'GOOG',
                    data: [16348000000, 17913000000, 19478000000, 16070000000, 21699000000],
                    borderColor: 'rgba(54, 162, 235, 1)',
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    yAxisID: 'y-axis-1',
                    hidden: false,
                    borderWidth: 1,
                },
            ]
          });
      }, 3000); // Simulates a 3-second delay
    });
  };
  