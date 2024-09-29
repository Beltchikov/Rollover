// store.js
import { EFDP_API_BASE_URL } from './config';
import { configureStore, createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { fetchRetainedEarningsData } from './Api/retainedEarningsEndpoint';  // Update this import to the correct file

// Define the initial state for the data
const initialState = {
    symbolsInput: 'NVDA\nMSFT\nGOOG', // List of stock symbols, separated by new lines
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
        dataRetainedEarnings: null,
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

// Create an async thunk for fetching retained earnings data
export const fetchRetainedEarnings = createAsyncThunk(
    'global/fetchRetainedEarnings',
    async (_, { getState }) => {
        const state = getState();
        const stockSymbols = state.global.symbolsInput.split('\n').map(symbol => symbol.trim()).filter(Boolean); 
        const response = await fetchRetainedEarningsData(stockSymbols, EFDP_API_BASE_URL);  
        return response;
    }
);

// Create a slice of the state
const globalSlice = createSlice({
    name: 'global',
    initialState,
    reducers: {
        updateSymbolsInput: (state, action) => {
            state.symbolsInput = action.payload;
        },
        toggleDatasetVisibility: (state, action) => {
            const { areaKey, datasetKey, datasetIndex } = action.payload;

            // Check if the area and dataset keys exist in the state
            const areaState = state[areaKey];
            if (areaState && areaState[datasetKey] && areaState[datasetKey].datasets) {
                const datasets = areaState[datasetKey].datasets;

                // Toggle the hidden state of the dataset if it exists
                const dataset = datasets[datasetIndex];
                if (dataset) {
                    dataset.hidden = !dataset.hidden;
                }
            }
        }
    },
    extraReducers: (builder) => {
        builder.addCase(fetchRetainedEarnings.fulfilled, (state, action) => {
            state.area2.dataRetainedEarnings = action.payload;
        });
    },
});

// Export actions
export const { updateSymbolsInput, toggleDatasetVisibility } = globalSlice.actions;

// Configure and export the Redux store
export const store = configureStore({
    reducer: {
        global: globalSlice.reducer,
    },
});
