import { create_element, REMOVE_SVG, PLUS_SVG, DOWNLOAD_SVG } from "../utils/utils.js";

const BEATMAP_CARD_TEMPLATE = `
<div class="beatmap-card">
    <div class="beatmap-bg">
        <img></img>
    </div>
    <div class="beatmap-controls">
        <button class="control-icon">▶</button>
        <button class="control-icon">×</button>
    </div>
    <div class="beatmap-info">
        <div class="beatmap-title">UNKNOWN</div>
        <div class="beatmap-subtitle">UNKNOWN</div>
        <div class="beatmap-stats">
            <span class="stat-badge ranked">UNKNOWN</span>
            <span class="star-rating">★ 0.00</span>
        </div>
    </div>
</div>
`;

export const set_control_to_preview = (el) => {
    el.textContent = "▶";
    el.dataset.type = "preview";
};

export const set_control_to_download = (el) => {
    el.innerHTML = DOWNLOAD_SVG;
    el.dataset.type = "download";
};

export const set_control_to_remove = (el) => {
    el.innerHTML = REMOVE_SVG;
    el.dataset.type = "remove";
};

export const set_control_to_add = (el) => {
    el.innerHTML = PLUS_SVG;
    el.dataset.type = "add";
};

export const toggle_control_preview = (el) => {
    el.textContent = el.textContent == "▶" ? el.textContent = "⏸" : el.textContent = "⏸";
};

export const create_beatmap_card = () => {
    return create_element(BEATMAP_CARD_TEMPLATE);
};

export const update_beatmap_card = (container, beatmap, remove) => {

    const metadata = {
        title: container?.querySelector(".beatmap-title"),
        subtitle: container?.querySelector(".beatmap-subtitle"),
        badge: container?.querySelector(".stat-badge"),
        star_rating: container?.querySelector(".star-rating"),
        background: container?.querySelector(".beatmap-bg").children[0]
    };

    const controls = {
        left: container.querySelector(".beatmap-controls").children[0],
        right: container.querySelector(".beatmap-controls").children[1],
    };

    // left by default is preview
    set_control_to_preview(controls.left);

    // from discover
    if (!beatmap?.local) {
        set_control_to_add(controls.right);
    }
    // from discover but not downloaded
    else if (!beatmap?.local && beatmap.downloaded) {
        set_control_to_download(controls.right);
    }
    // remove by default 
    else {
        set_control_to_remove(controls.right);
        controls.right.addEventListener("click", remove);
    }

    metadata.title.textContent = `${beatmap.artist} - ${beatmap.title}`;
    metadata.subtitle.textContent = beatmap.difficulty;
    metadata.background.src = `https://assets.ppy.sh/beatmaps/${beatmap.beatmapset_id}/covers/cover.jpg`;

    container.id = beatmap.md5;
};