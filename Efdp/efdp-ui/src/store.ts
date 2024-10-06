import { configureStore, createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { EFDP_API_BASE_URL, USE_MOCK_RESPONSES } from './config';
import { fetchIncomeStatementMockData } from './Api/income-statement-mock-endpoint';
import { fetchBalanceSheetStatementData } from './Api/balance-sheet-statement-endpoint';
import { fetchIncomeStatementData } from './Api/income-statement-endpoint';
import { fetchBalanceSheetStatementMockData } from './Api/balance-sheet-statement-mock-endpoint';
import { fetchCashFlowStatementData } from './Api/cash-flow-statement-endpoint';
import { fetchCashFlowStatementMockData } from './Api/cash-flow-statement-mock-endpoint';
import { getRandomColor } from './helpers';
import { createSymbolsTable, interpolateSymbolsTable, createChartData } from './Api/responseProcessing.ts';
import dayjs from 'dayjs';
import { ChartData } from './ChartData.ts';

interface GlobalState {
    symbolsInput: string;
    incomeStatementDict: Record<string, any>;
    cashFlowStatementDict: Record<string, any>;
    balanceSheetStatementDict: Record<string, any>;
    area1: {
        dataCagrFcf: ChartData | null;
        dataFcf: ChartData | null;
    };
    area2: {
        dataFcfCapExRatio: ChartData | null;
        dataRetainedEarnings: ChartData | null;
        dataGpm: ChartData | null;
    };
    area3: {
        dataLongTermDebtToFcf: ChartData | null;
    };
}

// Define the initial state for the data
const initialState: GlobalState = {
    symbolsInput: 'NVDA\nMSFT\nGOOG',
    incomeStatementDict: {},
    cashFlowStatementDict: {},
    balanceSheetStatementDict: {},
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
        dataFcfCapExRatio: null,
        dataRetainedEarnings: null,
        dataGpm: null
    },
    area3: {
        dataLongTermDebtToFcf: null,
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

// Combined thunk for fetching all data
export const fetchAllData = createAsyncThunk(
    'global/fetchAllData',
    async (_, { dispatch }) => {
        await dispatch(fetchIncomeStatementDict());
        await dispatch(fetchCashFlowStatementDict());
        await dispatch(fetchBalanceSheetStatementDict());
    }
);

// Async thunk for fetching income statement data
export const fetchIncomeStatementDict = createAsyncThunk(
    'global/fetchIncomeStatementDict',
    async (_, { getState }) => {
        const state = getState() as { global: GlobalState };
        const stockSymbols = state.global.symbolsInput.split('\n').map(symbol => symbol.trim()).filter(Boolean);

        const response = USE_MOCK_RESPONSES
            ? await fetchIncomeStatementMockData(EFDP_API_BASE_URL)
            : await fetchIncomeStatementData(stockSymbols, EFDP_API_BASE_URL);

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
        builder.addCase(fetchIncomeStatementDict.fulfilled, (state, action) => {
            const filteredIncomeStatementDict = filterStatementsOlderThan(action.payload, 10);
            state.incomeStatementDict = filteredIncomeStatementDict;

            state.area2.dataGpm = createChartDataForArea(
                state.incomeStatementDict,
                (is: { grossProfit: number; revenue: number; }) =>
                    Math.round(is.grossProfit * 100 / (is.revenue !== 0 ? is.revenue : 1)),
                (is: { date: any }) => is.date
            );
        });

        builder.addCase(fetchCashFlowStatementDict.fulfilled, (state, action) => {
            const filteredCashFlowStatementDict = filterStatementsOlderThan(action.payload, 10);
            state.cashFlowStatementDict = filteredCashFlowStatementDict;

            state.area1.dataFcf = createChartDataForArea(
                state.cashFlowStatementDict,
                (s: { operatingCashFlow: any; capitalExpenditure: any; }) => s.operatingCashFlow + s.capitalExpenditure,
                (s: { date: any }) => s.date
            );

            state.area2.dataFcfCapExRatio = createChartDataForArea(
                state.cashFlowStatementDict,
                (s: { operatingCashFlow: number; capitalExpenditure: number; }) =>
                    Math.round((s.operatingCashFlow + s.capitalExpenditure) * -100 / (s.capitalExpenditure !== 0 ? s.capitalExpenditure : 1)),
                (s: { date: any }) => s.date
            );
        });

        builder.addCase(fetchBalanceSheetStatementDict.fulfilled, (state, action) => {
            const filteredBalanceSheetStatementDict = filterStatementsOlderThan(action.payload, 10);
            state.balanceSheetStatementDict = filteredBalanceSheetStatementDict;

            state.area2.dataRetainedEarnings = createChartDataForArea(
                state.balanceSheetStatementDict,
                (bs: { retainedEarnings: any }) => bs.retainedEarnings,
                (bs: { date: any }) => bs.date
            );

            // TODO Variant1: use datalongTermDebt to dataFcf instead of longTermDebt only
            state.area3.dataLongTermDebtToFcf = createChartDataForArea(
                state.balanceSheetStatementDict,
                (bs: { longTermDebt: any }) => bs.longTermDebt,
                (bs: { date: any }) => bs.date
            );

            const dataLongTermDebt: ChartData = createChartDataForArea(
                state.balanceSheetStatementDict,
                (bs: { longTermDebt: any }) => bs.longTermDebt,
                (bs: { date: any }) => bs.date
            );

            const dataFcf: ChartData = state.area1.dataFcf ?? { labels: [], datasets: [] };

            // //TODO
            // const computedChartData: ChartData = computeChartData(dataLongTermDebt, dataFcf, (d1: number | null, d2: number | null): number | null => {
            //     if (d1 !== null && d2 !== null) {
            //         return Math.round(d1 *100 / (d2!==0?d2:1));  
            //     }
            //     return null;  // If either value is null, return null
            // });

        });
    },
});

const createChartDataForArea = (
    statementDict: any,
    financialAttributeSelector: (item: any) => number,
    dateSelector: (item: any) => any
) => {
    const symbolsTable = createSymbolsTable(statementDict, financialAttributeSelector, dateSelector);
    const interpolatedSymbolsTable = interpolateSymbolsTable(symbolsTable);
    return createChartData(interpolatedSymbolsTable, getRandomColor);
};


// Export actions
export const { updateSymbolsInput, toggleDatasetVisibility } = globalSlice.actions;

// Configure and export the Redux store
export const store = configureStore({
    reducer: {
        global: globalSlice.reducer,
    },
});
