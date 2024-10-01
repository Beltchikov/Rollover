export function getRandomColor(numberOfColors) {
    const colors = [];

    // Generate distinct colors (similar to Excel-like charts)
    for (let i = 0; i < numberOfColors; i++) {
        // Create random RGB values with high contrast
        const r = Math.floor(Math.random() * 256);
        const g = Math.floor(Math.random() * 256);
        const b = Math.floor(Math.random() * 256);

        // Create the rgba string and add it to the colors array
        colors.push(`rgba(${r}, ${g}, ${b}, 1)`);
    }

    return colors;
}
