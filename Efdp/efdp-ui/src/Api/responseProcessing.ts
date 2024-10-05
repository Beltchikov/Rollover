import { ChartDataset } from "../ChartDataset";
import { ChartData } from "../ChartData";

interface Statement {
    date: string;
    [key: string]: any; // Allows other fields besides date
}

type StatementDict = Record<string, Statement[]>;  // e.g., { 'GOOG': [ { date: '2023-12-31', ... }, ... ] }

export function extractLabels(statementDict: StatementDict, dateSelector: (statement: Statement) => string): Record<string, string[]> {
    const labels: Record<string, string[]> = {};

    for (const [key, statements] of Object.entries(statementDict)) {
        labels[key] = statements.map(dateSelector);
    }

    return labels;
}

export function createSymbolsTable(
    statementDict: StatementDict,
    financialAttributeSelector: (statement: Statement) => number | null,
    dateSelector: (statement: Statement) => string
): string[] {
    const labelsAsDict = extractLabels(statementDict, dateSelector);
    const labels = Object.values(labelsAsDict).flat().filter((v, i, a) => a.indexOf(v) === i).sort();
    const uniqueLabels = [...new Set(labels)].sort();

    const symbolsTable: string[] = [];
    const header = ["Symbol", ...uniqueLabels].join("\t");
    symbolsTable.push(header);

    for (const symbol in statementDict) {
        let row = symbol;
        const statements = statementDict[symbol];

        for (const label of uniqueLabels) {
            const statement = statements.find(s => dateSelector(s) === label);
            row += "\t" + (statement ? financialAttributeSelector(statement) : "");
        }

        symbolsTable.push(row);
    }

    return symbolsTable;
}

export function interpolateSymbolsTable(symbolsTable: string[]): string[] {
    const interpolatedTable = [symbolsTable[0]];

    for (let i = 1; i < symbolsTable.length; i++) {
        const row = symbolsTable[i].split("\t");
        const symbol = row[0];
        let dataColumns = row.slice(1);

        let values = dataColumns.map(val => (val === "" ? null : Number(val)));

        for (let j = 1; j < values.length - 1; j++) {
            if (values[j] === null) {
                let prevIndex = j - 1;
                let nextIndex = j + 1;

                while (prevIndex >= 0 && values[prevIndex] === null) {
                    prevIndex--;
                }

                while (nextIndex < values.length && values[nextIndex] === null) {
                    nextIndex++;
                }

                if (prevIndex >= 0 && nextIndex < values.length && values[prevIndex] !== null && values[nextIndex] !== null) {
                    const prevValue : number | null = values[prevIndex];
                    const nextValue: number | null = values[nextIndex];
                    const gap = nextIndex - prevIndex;

                    if (prevValue !== null && nextValue !== null) {
                    values[j] = prevValue + ((nextValue - prevValue) * (j - prevIndex)) / gap;
                    }
                }
            }
        }

        const interpolatedRow = [symbol, ...values.map(v => (v !== null ? v : ""))].join("\t");
        interpolatedTable.push(interpolatedRow);
    }

    return interpolatedTable;
}

export function createChartData(
    interpolatedSymbolsTable: string[],
    getRandomRgbColors: (count: number) => string[]
): ChartData {
    const colors = getRandomRgbColors(interpolatedSymbolsTable.length - 1);
    const labels = interpolatedSymbolsTable[0].split("\t").slice(1);

    const datasets: ChartDataset[] = interpolatedSymbolsTable.slice(1).map((row, index) => {
        const columns = row.split("\t");
        const symbol = columns[0];
        const data: Array<number | null> = columns.slice(1).map(val => (val === "" ? null : Number(val)));


        return {
            label: symbol,
            data: data,
            borderColor: colors[index],
            backgroundColor: colors[index].replace('1)', '0.2)'), // Making the color transparent
            yAxisID: 'y-axis-1',
            hidden: false,
            borderWidth: 1,
        };
    });

    return { labels, datasets };
}
