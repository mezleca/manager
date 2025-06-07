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

export const safe_text = (text) => {

    if (!text) {
        return "";
    }

    return String(text).replace(/[<>&"']/g, char => {
        switch(char) {
            case '<': return '&lt;';
            case '>': return '&gt;';
            case '&': return '&amp;';
            case '"': return '&quot;';
            case "'": return '&#39;';
            default: return char;
        }
    });
};

export const safe_id = (id) => {
    return String(id).replace(/[^\w-]/g, '');
};

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

export const osu_login = async (id, secret) => {
    try {
        const form_data = new FormData();

        form_data.append("grant_type", 'client_credentials');
        form_data.append("client_id", id);
        form_data.append("client_secret", secret);
        form_data.append("scope", "public");
            
        const response = await fetch(`https://osu.ppy.sh/oauth/token`, { method: 'POST', body: form_data });
        const data = await response.json();

        if (response.status != 200) {
            create_alert("failed to login<br>make sure your osu_id/secret is valid", { type: "error", seconds: 10, html: true });
            return null;
        }
        
        return data;
    } catch(err) {
        console.log("[login] error:", err);
        return null;
    }
};

export const osu_fetch = async (url) => {
    try {
        const response = await fetch(url, {
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json",
                "Authorization": `Bearer ${core.login.access_token}`
            }
        });
        return await response.json();
    } catch (error) {
        console.error("fetch failed:", error);
        return null;
    }
};
