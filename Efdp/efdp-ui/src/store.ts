import { configureStore, createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { EFDP_API_BASE_URL, USE_MOCK_RESPONSES } from './config';
import { fetchBalanceSheetStatementData } from './Api/balance-sheet-statement-endpoint';
import { fetchBalanceSheetStatementMockData } from './Api/balance-sheet-statement-mock-endpoint';
import { fetchCashFlowStatementData } from './Api/cash-flow-statement-endpoint';
import { fetchCashFlowStatementMockData } from './Api/cash-flow-statement-mock-endpoint';
import { getRandomColor } from './helpers';
import { createSymbolsTable, interpolateSymbolsTable, createChartData } from './Api/responseProcessing';
import dayjs from 'dayjs';

// Define interfaces for datasets and statements
interface Dataset {
    label: string;
    data: number[];
    borderColor: string;
    backgroundColor: string;
    yAxisID: string;
    hidden: boolean;
    borderWidth: number;
}

interface AreaState {
    labels: string[];
    datasets: Dataset[];
}

interface GlobalState {
    symbolsInput: string;
    incomeStatementDict: Record<string, any>;
    incomeStatementUpdated: Date;
    cashFlowStatementDict: Record<string, any>;
    cashFlowStatementUpdated: Date;
    balanceSheetStatementDict: Record<string, any>;
    balanceSheetStatement: Date;
    area1: {
        dataCagrFcf: AreaState;
        dataFcf: AreaState | null;  // Could be null initially
    };
    area2: {
        dataFcfCapExRatio: AreaState;
        dataRetainedEarnings: AreaState | null;  // Could be null initially
        dataGpm: AreaState;
    };
    area3: {
        dataLongTermDebtToEarnings: AreaState;
    };
}

// Define the initial state for the data
const initialState: GlobalState = {
    symbolsInput: 'NVDA\nMSFT\nGOOG',
    incomeStatementDict: {},
    incomeStatementUpdated: new Date(),
    cashFlowStatementDict: {},
    cashFlowStatementUpdated: new Date(),
    balanceSheetStatementDict: {},
    balanceSheetStatement: new Date(),
    area1: {
        dataCagrFcf: {
            labels: ['2009-09-26', '2009-12-31', '2010-06-30', '2010-09-25', '2010-12-31'],
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
            ],
        },
        dataFcf: null,
    },
    area2: {
        dataFcfCapExRatio: {
            labels: ['2009-09-26', '2009-12-31', '2010-06-30', '2010-09-25', '2010-12-31'],
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
            ],
        },
        dataRetainedEarnings: null,
        dataGpm: {
            labels: ['2009-09-26', '2009-12-31', '2010-06-30', '2010-09-25', '2010-12-31'],
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
            ],
        },
    },
    area3: {
        dataLongTermDebtToEarnings: {
            labels: ['2009-09-26', '2009-12-31', '2010-06-30', '2010-09-25', '2010-12-31'],
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
            ],
        },
    },
};

// Utility function to filter out statements older than X years
function filterStatementsOlderThan<T extends { date: string }>(data: Record<string, T[]>, years: number): Record<string, T[]> {
    const currentDate = dayjs();
    const cutoffDate = currentDate.subtract(years, 'year');

    return Object.keys(data).reduce((filteredDict: Record<string, T[]>, symbol: string) => {
        const filteredStatements = data[symbol].filter((statement) => dayjs(statement.date).isAfter(cutoffDate));
        if (filteredStatements.length > 0) {
            filteredDict[symbol] = filteredStatements;
        }
        return filteredDict;
    }, {});
}

// Async thunk for fetching balance sheet data
export const fetchBalanceSheetStatementDict = createAsyncThunk(
    'global/fetchBalanceSheetStatementDict',
    async (_, { getState }) => {
        const state = getState() as { global: GlobalState };
        const stockSymbols = state.global.symbolsInput.split('\n').map(symbol => symbol.trim()).filter(Boolean);

        const response = USE_MOCK_RESPONSES
            ? await fetchBalanceSheetStatementMockData(EFDP_API_BASE_URL)
            : await fetchBalanceSheetStatementData(stockSymbols, EFDP_API_BASE_URL);

        return response;
    }
);

// Async thunk for fetching cash flow data
export const fetchCashFlowStatementDict = createAsyncThunk(
    'global/fetchCashFlowStatementDict',
    async (_, { getState }) => {
        const state = getState() as { global: GlobalState };
        const stockSymbols = state.global.symbolsInput.split('\n').map(symbol => symbol.trim()).filter(Boolean);

        const response = USE_MOCK_RESPONSES
            ? await fetchCashFlowStatementMockData(EFDP_API_BASE_URL)
            : await fetchCashFlowStatementData(stockSymbols, EFDP_API_BASE_URL);

        return response;
    }
);

// Combined thunk for fetching all data
export const fetchAllData = createAsyncThunk(
    'global/fetchAllData',
    async (_, { dispatch }) => {
        // Fetch balance sheet data
        await dispatch(fetchBalanceSheetStatementDict());
        // Fetch cash flow data
        await dispatch(fetchCashFlowStatementDict());
    }
);

// Create a slice of the state
const globalSlice = createSlice({
    name: 'global',
    initialState,
    reducers: {
        updateSymbolsInput: (state, action: PayloadAction<string>) => {
            state.symbolsInput = action.payload;
        },
        toggleDatasetVisibility: (state, action: PayloadAction<{ areaKey: keyof GlobalState; datasetKey: string; datasetIndex: number }>) => {
            const { areaKey, datasetKey, datasetIndex } = action.payload;

            const areaState = state[areaKey] as any;
            if (areaState && areaState[datasetKey] && areaState[datasetKey].datasets) {
                const datasets = areaState[datasetKey].datasets;
                const dataset = datasets[datasetIndex];

                if (dataset) {
                    dataset.hidden = !dataset.hidden;
                }
            }
        },
    },
    extraReducers: (builder) => {
        builder.addCase(fetchBalanceSheetStatementDict.fulfilled, (state, action) => {
            const filteredBalanceSheetStatementDict = filterStatementsOlderThan(action.payload, 10);
            state.balanceSheetStatementDict = filteredBalanceSheetStatementDict;

            const symbolsTable = createSymbolsTable(state.balanceSheetStatementDict, (bs) => bs.retainedEarnings, (bs) => bs.date);
            const interpolatedSymbolsTable = interpolateSymbolsTable(symbolsTable);
            const chartData = createChartData(interpolatedSymbolsTable, getRandomColor);

            state.area2.dataRetainedEarnings = chartData;
        });
        builder.addCase(fetchCashFlowStatementDict.fulfilled, (state, action) => {
            const filteredCashFlowStatementDict = filterStatementsOlderThan(action.payload, 10);
            state.cashFlowStatementDict = filteredCashFlowStatementDict;

            const symbolsTable = createSymbolsTable(
                state.cashFlowStatementDict,
                (s) => s.operatingCashFlow + s.capitalExpenditure,
                (s) => s.date
            );
            const interpolatedSymbolsTable = interpolateSymbolsTable(symbolsTable);
            const chartData = createChartData(interpolatedSymbolsTable, getRandomColor);

            state.area1.dataFcf = chartData;
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
