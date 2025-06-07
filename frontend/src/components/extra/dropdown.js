import { safe_id, create_element } from "../utils/utils.js";

// @TODO: css
export const create_dropdown = (options = { id: crypto.randomUUID(), name: "dropdown", values: ["abc", "fgh", "qre"]}) => {

    const container = create_element(`
        <div class="filter-group" id="${safe_id(options.id.replace(/\s+/g, '_'))}">
            <div class="filter-label">TODO</div>
            <select class="filter-select">
                <option>TODO</option>
            </select>
        </div>
    `);

    return container;
};