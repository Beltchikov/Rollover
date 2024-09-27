// store.js
import { configureStore, createSlice } from '@reduxjs/toolkit';

// Define the initial state for the data
const initialState = {
  symbolsInput: 'NVDA\nMSFT\nGOOG',
  area1: {
    dataCagrFcf: {
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
      },
    dataFcf: {
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
      },
  },
  area2: {
    dataFcfCapExRatio: {
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
      },
    dataRetainedEarnings: {
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
      },
    dataGpm: {
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
      },
  },
  area3: {
    dataLongTermDebtToEarnings: {
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
      },
  },
};

// Create a slice of the state
const globalSlice = createSlice({
  name: 'global',
  initialState,
  reducers: {
    updateSymbolsInput: (state, action) => {
      state.symbolsInput = action.payload;
    },
  },
});

// Export actions
export const { updateSymbolsInput } = globalSlice.actions;

// Configure and export the Redux store
export const store = configureStore({
  reducer: {
    global: globalSlice.reducer,
  },
});
