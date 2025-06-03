export const REMOVE_SVG =
`<svg viewBox="0 0 10 10" width="14px" height="14px" stroke="currentColor" stroke-width="2">
    <path d="M1,1 9,9 M9,1 1,9" />
</svg>`;

export const DOWNLOAD_SVG =
`<svg id="play-button" viewBox="0 0 84 100" version="1.1" xmlns="http://www.w3.org/2000/svg" fill="currentColor">
    <polygon points="10,0 10,100 90,50"/>
</svg>`;

export const PLUS_SVG =
`<svg id="add-btn" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 10 10" width="16" height="16" stroke="currentColor" fill="none">
    <line x1="5" y1="2" x2="5" y2="8" stroke-width="1"></line>
    <line x1="2" y1="5" x2="8" y2="5" stroke-width="1"></line>
</svg>`;

export const star_ranges = [
    [0, 2.99, "sr1"],
    [3, 4.99, "sr2"],
    [5, 6.99, "sr3"],
    [7, 7.99, "sr4"],
    [8, 8.99, "sr5"],
    [9, Infinity, "sr6"]
];

export const create_element = (data)  => {
    return new DOMParser().parseFromString(data, "text/html").body.firstElementChild;
};

export const debounce = (func, timeout = 100) => {
    let timer;
    return (...args) => {
        clearTimeout(timer);
        timer = setTimeout(() => func(...args), timeout);
    };
};
