// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function elasticSqlToRecords(json) {
    const columns = json.columns.map(col => col.name);

    return json.rows.map(row => {
        const obj = {};
        columns.forEach((col, i) => obj[col] = row[i]);
        return obj;
    });
}

function generateTable(records, columns = null) {
    let table = document.createElement('table');

    let thead = document.createElement('thead');
    let row = document.createElement('tr');

    let skipCols = [];

    let i = 0;
    for (var key in records[0]) {
        let headerName = key;
        if (columns != null) {
            if (!columns.hasOwnProperty(key)) {
                skipCols.push(key);
                continue;
            }
            headerName = columns[key];
            i++;
        }
        let cell = document.createElement('th');
        cell.innerHTML = headerName;
        row.appendChild(cell);
    }
    thead.appendChild(row);

    let tbody = document.createElement('tbody');

    for (let i = 0; i < records.length; i++) {
        let row = document.createElement('tr');
        for (let key in records[i]) {
            if (skipCols.includes(key)) {
                continue;
            }
            let cell = document.createElement('td');
            cell.innerHTML = records[i][key];
            row.appendChild(cell);
        }
        tbody.appendChild(row);
    }

    table.appendChild(thead);
    table.appendChild(tbody);

    return table;
}

function toSlug(input) {
    return input
    .toLowerCase()
    .trim()                         // Remove whitespace from both sides of a string
    .replace(/\s+/g, '-')           // Replace spaces with -
    .replace(/&/g, '-y-')           // Replace & with 'and'
    .replace(/[^\w\-]+/g, '')       // Remove all non-word chars
    .replace(/\-\-+/g, '-');
}
