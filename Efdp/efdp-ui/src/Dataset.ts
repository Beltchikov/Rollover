// Define interfaces for datasets and statements
export interface Dataset {
    label: string;
    data: number[];
    borderColor: string;
    backgroundColor: string;
    yAxisID: string;
    hidden: boolean;
    borderWidth: number;
}
