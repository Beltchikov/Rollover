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
import { ChartData, IChartData } from './ChartData.ts';
import { IGlobalState } from './IGlobalState.ts';

// Define the initial state for the data
const initialState: IGlobalState = {
    symbolsInput: 'NVDA\nMSFT\nGOOG',
    incomeStatementDict: {},
    cashFlowStatementDict: {},
    balanceSheetStatementDict: {},
    area1: {
        dataCagrFcf: {
            labels: [''],  // Labels
            datasets: [
                {
                    label: 'NVDA',
                    data: [30],
                    borderColor: 'rgba(255, 99, 132, 1)',
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    yAxisID: 'y-axis-1',
                    hidden: false,
                    borderWidth: 1,
                },
                {
                    label: 'GOOG',
                    data: [20],
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
        const state = getState() as { global: IGlobalState };
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
        const state = getState() as { global: IGlobalState };
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
        const state = getState() as { global: IGlobalState };
        const stockSymbols = state.global.symbolsInput.split('\n').map(symbol => symbol.trim()).filter(Boolean);

        const response = USE_MOCK_RESPONSES
            ? await fetchBalanceSheetStatementMockData(EFDP_API_BASE_URL)
            : await fetchBalanceSheetStatementData(stockSymbols, EFDP_API_BASE_URL);

        return response;
    }
);

// Create a slice of the state
const globalSlice = createGlobalSlice();

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

function createGlobalSlice() {
    return createSlice({
        name: 'global',
        initialState,
        reducers: {
            updateSymbolsInput: (state, action: PayloadAction<string>) => {
                state.symbolsInput = action.payload;
            },
            toggleDatasetVisibility: (state, action: PayloadAction<{ areaKey: keyof IGlobalState; datasetKey: string; datasetIndex: number; }>) => {
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

                state.area2.dataGpm = {
                    labels: createChartDataForArea(
                        state.incomeStatementDict,
                        (is: { grossProfit: number; revenue: number; }) => Math.round(is.grossProfit * 100 / (is.revenue !== 0 ? is.revenue : 1)),
                        (is: { date: any; }) => is.date
                    ).labels,
                    datasets: createChartDataForArea(
                        state.incomeStatementDict,
                        (is: { grossProfit: number; revenue: number; }) => Math.round(is.grossProfit * 100 / (is.revenue !== 0 ? is.revenue : 1)),
                        (is: { date: any; }) => is.date
                    ).datasets.map(dataset => ({
                        label: dataset.label,
                        data: dataset.data,
                        borderColor: dataset.borderColor,
                        backgroundColor: dataset.backgroundColor,
                        yAxisID: dataset.yAxisID,
                        hidden: dataset.hidden,
                        borderWidth: dataset.borderWidth
                    }))
                };
            });

            builder.addCase(fetchCashFlowStatementDict.fulfilled, (state, action) => {
                const filteredCashFlowStatementDict = filterStatementsOlderThan(action.payload, 10);
                state.cashFlowStatementDict = filteredCashFlowStatementDict;

                state.area1.dataFcf = {
                    labels: createChartDataForArea(
                        state.cashFlowStatementDict,
                        (s: { operatingCashFlow: any; capitalExpenditure: any; }) => s.operatingCashFlow + s.capitalExpenditure,
                        (s: { date: any; }) => s.date
                    ).labels,
                    datasets: createChartDataForArea(
                        state.cashFlowStatementDict,
                        (s: { operatingCashFlow: any; capitalExpenditure: any; }) => s.operatingCashFlow + s.capitalExpenditure,
                        (s: { date: any; }) => s.date
                    ).datasets.map(dataset => ({
                        label: dataset.label,
                        data: dataset.data,
                        borderColor: dataset.borderColor,
                        backgroundColor: dataset.backgroundColor,
                        yAxisID: dataset.yAxisID,
                        hidden: dataset.hidden,
                        borderWidth: dataset.borderWidth
                    }))
                };

                var dataCagrFcf:IChartData = {labels:[''], datasets:[]};
                for(var i=0; i < state.area1.dataFcf.datasets.length; i++)
                {
                    const dataset = state.area1.dataFcf.datasets[i];
                    dataCagrFcf.datasets.push({
                        label: dataset.label,
                        data: [cagr(dataset.data, 10)],
                        borderColor: dataset.borderColor,
                        backgroundColor: dataset.backgroundColor,
                        yAxisID: dataset.yAxisID,
                        hidden: dataset.hidden,
                        borderWidth: dataset.borderWidth
                    });
                }
                state.area1.dataCagrFcf = dataCagrFcf;

                state.area2.dataFcfCapExRatio = {
                    labels: createChartDataForArea(
                        state.cashFlowStatementDict,
                        (s: { operatingCashFlow: number; capitalExpenditure: number; }) => Math.round((s.operatingCashFlow + s.capitalExpenditure) * -100 / (s.capitalExpenditure !== 0 ? s.capitalExpenditure : 1)),
                        (s: { date: any; }) => s.date
                    ).labels,
                    datasets: createChartDataForArea(
                        state.cashFlowStatementDict,
                        (s: { operatingCashFlow: number; capitalExpenditure: number; }) => Math.round((s.operatingCashFlow + s.capitalExpenditure) * -100 / (s.capitalExpenditure !== 0 ? s.capitalExpenditure : 1)),
                        (s: { date: any; }) => s.date
                    ).datasets.map(dataset => ({
                        label: dataset.label,
                        data: dataset.data,
                        borderColor: dataset.borderColor,
                        backgroundColor: dataset.backgroundColor,
                        yAxisID: dataset.yAxisID,
                        hidden: dataset.hidden,
                        borderWidth: dataset.borderWidth
                    }))
                };
            });

            builder.addCase(fetchBalanceSheetStatementDict.fulfilled, (state, action) => {
                const filteredBalanceSheetStatementDict = filterStatementsOlderThan(action.payload, 10);
                state.balanceSheetStatementDict = filteredBalanceSheetStatementDict;

                state.area2.dataRetainedEarnings = {
                    labels: createChartDataForArea(
                        state.balanceSheetStatementDict,
                        (bs: { retainedEarnings: any; }) => bs.retainedEarnings,
                        (bs: { date: any; }) => bs.date
                    ).labels,
                    datasets: createChartDataForArea(
                        state.balanceSheetStatementDict,
                        (bs: { retainedEarnings: any; }) => bs.retainedEarnings,
                        (bs: { date: any; }) => bs.date
                    ).datasets.map(dataset => ({
                        label: dataset.label,
                        data: dataset.data,
                        borderColor: dataset.borderColor,
                        backgroundColor: dataset.backgroundColor,
                        yAxisID: dataset.yAxisID,
                        hidden: dataset.hidden,
                        borderWidth: dataset.borderWidth
                    }))
                };

                const dataLongTermDebt = {
                    labels: createChartDataForArea(
                        state.balanceSheetStatementDict,
                        (bs: { longTermDebt: any; }) => bs.longTermDebt,
                        (bs: { date: any; }) => bs.date
                    ).labels,
                    datasets: createChartDataForArea(
                        state.balanceSheetStatementDict,
                        (bs: { longTermDebt: any; }) => bs.longTermDebt,
                        (bs: { date: any; }) => bs.date
                    ).datasets.map(dataset => ({
                        label: dataset.label,
                        data: dataset.data,
                        borderColor: dataset.borderColor,
                        backgroundColor: dataset.backgroundColor,
                        yAxisID: dataset.yAxisID,
                        hidden: dataset.hidden,
                        borderWidth: dataset.borderWidth
                    }))
                };

                const dataFcf = state.area1.dataFcf ?? { labels: [], datasets: [] };
                state.area3.dataLongTermDebtToFcf = computeChartData(dataLongTermDebt, dataFcf, (d1: number | null, d2: number | null): number | null => {
                    if (d1 !== null && d2 !== null) {
                        return Math.round(d1 * 100 / (d2 !== 0 ? d2 : 1));
                    }
                    return null;  // If either value is null, return null
                });
            });
        },
    });
}


function computeChartData(
    chartData1: ChartData,
    chartData2: ChartData,
    computeFn: (d1: number | null, d2: number | null) => number | null) {

    // Check if labels are the same in both datasets
    if (JSON.stringify(chartData1.labels) !== JSON.stringify(chartData2.labels)) {
        throw new Error('Labels in the two datasets do not match.');
    }

    const resultChartData: ChartData = new ChartData([], []);
    for (let i = 0; i < chartData1.labels.length; i++) {
        resultChartData.labels.push(chartData1.labels[i]);
    }

    for (let ii = 0; ii < chartData1.datasets.length; ii++) {
        const data1 = chartData1.datasets[ii].data;
        const data2 = chartData2.datasets[ii].data;
        const computedData = data1.map((v, i) => computeFn(v, data2[i]));
        resultChartData.datasets.push({ ...chartData1.datasets[ii], data: computedData });

    }

    // Convert the class instance into a plain JS object
    const serializableResultData = {
        labels: [...resultChartData.labels], // Spread to ensure plain array
        datasets: resultChartData.datasets.map(dataset => ({
            label: dataset.label,
            data: [...dataset.data],  // Spread to ensure plain array
            borderColor: dataset.borderColor,
            backgroundColor: dataset.backgroundColor,
            yAxisID: dataset.yAxisID,
            hidden: dataset.hidden,
            borderWidth: dataset.borderWidth
        }))
    };

    // Return the final computed chart data
    return serializableResultData;
}


function cagr(data: (number | null)[], years:number): (number | null) {
    // Filter out any null values to find the first and last non-null data points
    const filteredData = data.filter(d => d !== null) as number[];

    // If there are not enough data points, return null
    if (filteredData.length < 2) {
        return null;
    }

    // Get the first and last data points
    const firstData = filteredData[0]; // First non-null data
    const lastData = filteredData[filteredData.length - 1]; // Last non-null data

    // Handle case where the first value is 0 to prevent division by 0
    const growth = lastData / (firstData !== 0 ? firstData : 1);

    // Calculate CAGR: (lastData / firstData)^(1/years) - 1
    const cagrValue = Math.round((Math.pow(growth, 1 / years) - 1) * 1000) / 1000;

    return cagrValue;
}

