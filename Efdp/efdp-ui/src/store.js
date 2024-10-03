// store.js
import { EFDP_API_BASE_URL, USE_MOCK_RESPONSES } from './config';
import { configureStore, createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { fetchBalanceSheetStatementData } from './Api/balance-sheet-statement-endpoint';
import { fetchBalanceSheetStatementMockData } from './Api/balance-sheet-statement-mock-endpoint';
import { fetchCashFlowStatementData } from './Api/cash-flow-statement-endpoint';
import { fetchCashFlowStatementMockData } from './Api/cash-flow-statement-mock-endpoint';
import { getRandomColor } from './helpers'
import { createSymbolsTable, interpolateSymbolsTable, createChartData } from './Api/responseProcessing'

// Define the initial state for the data
const initialState = {
    symbolsInput: 'NVDA\nMSFT\nGOOG', // List of stock symbols, separated by new lines
    incomeStatementDict: {},
    cashFlowStatementDict: {},
    balanceSheetStatementDict: {},
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

export const fetchAllData = createAsyncThunk(
    'global/fetchAllData',
    async (_, { dispatch, getState }) => {
        // Get state if needed
        const state = getState();

        // Dispatch both thunks (fetchRetainedEarnings and fetchFreeCashFlow)
        const [retainedEarningsResponse, freeCashFlowResponse] = await Promise.all([
            dispatch(fetchRetainedEarnings()), // Dispatch retained earnings fetch
            dispatch(fetchFreeCashFlow())      // Dispatch free cash flow fetch
        ]);

        // Combine the responses into one object
        return {
            retainedEarnings: retainedEarningsResponse.payload,
            freeCashFlow: freeCashFlowResponse.payload
        };
    }
);

// Create an async thunk for fetching retained earnings data
export const fetchRetainedEarnings = createAsyncThunk(
    'global/fetchRetainedEarnings',
    async (_, { getState }) => {
        const state = getState();
        const stockSymbols = state.global.symbolsInput.split('\n').map(symbol => symbol.trim()).filter(Boolean);
        const response = USE_MOCK_RESPONSES
            ? await fetchBalanceSheetStatementMockData(EFDP_API_BASE_URL)
            : await fetchBalanceSheetStatementData(stockSymbols, EFDP_API_BASE_URL);

        return response;
    }
);

// Create an async thunk for fetching free cash flow data (empty body for now)
export const fetchFreeCashFlow = createAsyncThunk(
    'global/fetchFreeCashFlow',
    async (_, { getState }) => {
        const state = getState();
        const stockSymbols = state.global.symbolsInput.split('\n').map(symbol => symbol.trim()).filter(Boolean);
        const response = USE_MOCK_RESPONSES
            ? await fetchCashFlowStatementMockData(EFDP_API_BASE_URL)
            : await fetchCashFlowStatementData(stockSymbols, EFDP_API_BASE_URL);

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
            console.log(`EFDP_API_BASE_URL:${EFDP_API_BASE_URL}`);
            console.log(`USE_MOCK_RESPONSES:${USE_MOCK_RESPONSES}`);

            state.balanceSheetStatementDict = action.payload;

            var symbolsTable = createSymbolsTable(state.balanceSheetStatementDict, bs => bs.retainedEarnings, bs=>bs.date);
            var interpolatedsymbolsTable = interpolateSymbolsTable(symbolsTable);
            var chartData = createChartData(interpolatedsymbolsTable, getRandomColor);

            state.area2.dataRetainedEarnings = chartData;
        })
        .addCase(fetchFreeCashFlow.fulfilled, (state, action) => {
            state.cashFlowStatementDict = action.payload;
            console.log('state.cashFlowStatementDict:', state.cashFlowStatementDict);

            const symbolsTable = createSymbolsTable(state.cashFlowStatementDict, s=>s.operatingCashFlow + s.capitalExpenditure, s=>s.date);
            // const interpolatedSymbolsTable = interpolateSymbolsTable(symbolsTable);
            // const chartData = createChartData(interpolatedSymbolsTable, getRandomColor);

            // state.area1.dataFcf = chartData; // Assuming the dataFcf field in area1 is for free cash flow
        });;
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
