import { IChartData } from "./ChartData";

export interface IGlobalState {
    symbolsInput: string;
    incomeStatementDict: Record<string, any>;
    cashFlowStatementDict: Record<string, any>;
    balanceSheetStatementDict: Record<string, any>;
    area1: {
        dataCagrFcf: IChartData | null;
        dataFcf: IChartData | null;
    };
    area2: {
        dataFcfCapExRatio: IChartData | null;
        dataRetainedEarnings: IChartData | null;
        dataGpm: IChartData | null;
    };
    area3: {
        dataLongTermDebtToFcf: IChartData | null;
    };
}
