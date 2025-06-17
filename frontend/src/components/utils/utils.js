import { ipc } from "../../ipc/message.js";

const beatmap_cache = new Map();

export const PLAY_SVG = 
`<svg viewBox="0 0 84 100" fill="currentColor" style="width: 14px;">
    <polygon points="10,0 10,100 90,50"/>
</svg>`;

export const PAUSE_SVG = 
`<svg viewBox="0 0 100 100" fill="currentColor" style="width: 14px;">
    <rect x="15" y="0" width="25" height="100"/>
    <rect x="65" y="0" width="25" height="100"/>
</svg>`;

export const REMOVE_SVG =
`<svg viewBox="0 0 10 10" width="14px" height="14px" stroke="currentColor" stroke-width="2">
    <path d="M1,1 9,9 M9,1 1,9" />
</svg>`;

export const DOWNLOAD_SVG =
`<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-file-earmark-arrow-down-fill" viewBox="0 0 16 16">
    <path d="M9.293 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V4.707A1 1 0 0 0 13.707 4L10 .293A1 1 0 0 0 9.293 0M9.5 3.5v-2l3 3h-2a1 1 0 0 1-1-1m-1 4v3.793l1.146-1.147a.5.5 0 0 1 .708.708l-2 2a.5.5 0 0 1-.708 0l-2-2a.5.5 0 0 1 .708-.708L7.5 11.293V7.5a.5.5 0 0 1 1 0"/>
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

/*
    --- BEATMAP SHIT ---
*/

export const get_beatmap = async (md5) => {

    // limit cache size
    if (beatmap_cache.size >= 500) {
        const first = beatmap_cache.keys().next().value;
        if (first != undefined) {
            beatmap_cache.delete(first);
        }
    }

    if (beatmap_cache.has(md5)) {
        return beatmap_cache.get(md5);
    }

    const result = await ipc.send("get_beatmap", { md5: md5 });

    if (result.found) {
        beatmap_cache.set(md5, result);
    }

    return result;
};

/*
    --- REQUEST SHIT ---
*/
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
