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

export class ChartDataset implements IChartDataset {
    label: string;
    data: (number | null)[];
    borderColor: string;
    backgroundColor: string;
    yAxisID: string;
    hidden: boolean;
    borderWidth: number;

    constructor(
        label: string, 
        data: (number | null)[], 
        borderColor: string, 
        backgroundColor: string, 
        yAxisID: string, 
        hidden: boolean, 
        borderWidth: number
    ) {
        this.label = label;
        this.data = data;
        this.borderColor = borderColor;
        this.backgroundColor = backgroundColor;
        this.yAxisID = yAxisID;
        this.hidden = hidden;
        this.borderWidth = borderWidth;
    }
}

export class ChartData implements IChartData {
    labels: string[];
    datasets: ChartDataset[];

    constructor(labels: string[], datasets: ChartDataset[]) {
        this.labels = labels;
        this.datasets = datasets;
    }
}