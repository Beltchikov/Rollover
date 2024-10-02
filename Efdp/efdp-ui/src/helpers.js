export function getRandomColor(numberOfColors, maxRgbValue = 200) {
    const colors = [];

    // Ensure maxRgbValue is between 0 and 255
    const maxValue = Math.min(Math.max(maxRgbValue, 0), 255);

    // Generate distinct colors within the specified RGB range
    for (let i = 0; i < numberOfColors; i++) {
        // Create random RGB values with a range between 0 and maxValue
        const r = Math.floor(Math.random() * maxValue); // Red value between 0 and maxValue
        const g = Math.floor(Math.random() * maxValue); // Green value between 0 and maxValue
        const b = Math.floor(Math.random() * maxValue); // Blue value between 0 and maxValue

        // Ensure color contrast by avoiding overly dark colors (optional)
        if (r + g + b < 100) {
            i--; // Regenerate the color if it's too dark
            continue;
        }

        // Create the rgba string and add it to the colors array
        colors.push(`rgba(${r}, ${g}, ${b}, 1)`);
    }

    return colors;
}

