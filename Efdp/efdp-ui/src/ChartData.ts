export interface IChartData {
    labels: string[];
    datasets: IChartDataset[];
}

export interface IChartDataset {
    label: string;
    data: (number | null)[]; 
    borderColor: string;
    backgroundColor: string;
    yAxisID: string;
    hidden: boolean;
    borderWidth: number;
}
