import { create_element } from "../utils/utils";

export const COLLECTION_CARD_TEMPLATE =
`<div class="collection-item">
    <div class="collection-info">
        <svg class="music-icon" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M9 18V5l12-2v13"></path>
            <circle cx="6" cy="18" r="3"></circle>
            <circle cx="18" cy="16" r="3"></circle>
        </svg>
        <span class="collection-name">unknown</span>
    </div>
    <span class="collection-count">0 maps</span>
</div>`;

export const create_collection_card = (id, name, count) => {

    const container = create_element(COLLECTION_CARD_TEMPLATE);

    const data = {
        name: container.querySelector(".collection_name"),
        count: container.querySelector(".collection-count"),
    };

    data.name.textContent = name;
    data.count.textContent = count;

    return container;
};
