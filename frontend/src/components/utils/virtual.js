import { create_element, debounce } from "./utils.js";

export const virtual_lists = new Map();

const element_size_cache = new Map();
const BUFFER = 4;

export const get_element_size = (element, id, force = false) => {
    if (!force && element_size_cache.has(id)) return element_size_cache.get(id);

    const new_el = element.cloneNode(true);

    Object.assign(new_el.style, { visibility: "hidden", position: "absolute", top: "-9999px" });

    document.body.appendChild(new_el);
    const rect = new_el.getBoundingClientRect();
    new_el.remove();
    element_size_cache.set(id, rect);
    return rect;
};

export const render = (id, force) => {
    const virtual_list = virtual_lists.get(id);
    if (!virtual_list || !virtual_list.length) return;

    const { container, fake_height, rows, columns, length, last_render } = virtual_list;
    
    const scroll_top = container.scrollTop;
    const visible_height = container.clientHeight;
    const item_height = fake_height.clientHeight / rows;

    const first_row = Math.max(0, Math.floor(scroll_top / item_height) - BUFFER);
    const last_row = Math.min(rows, Math.ceil((scroll_top + visible_height) / item_height) + BUFFER);

    const start_index = first_row * columns;
    const end_index = Math.min(length, last_row * columns);

    if (!force && last_render && 
        start_index >= last_render.start && 
        end_index <= last_render.end && 
        length == last_render.total_length) {
        return;
    }
    
    const elements = [];
    
    for (let i = start_index; i < end_index; i++) {
        const data = virtual_list.create(i);
        if (virtual_list.update) {
            virtual_list.update({ ...data, index: i });
        }
        elements.push(data.element);
    }
    
    virtual_list.list_container.style.transform = `translateY(${first_row * item_height}px)`;
    virtual_list.list_container.replaceChildren(...elements);
    
    virtual_list.last_render = { start: start_index, end: end_index };
};

export const get_column_amount = (container) => {
    const computed = window.getComputedStyle(container);
    return computed.getPropertyValue("grid-template-columns").split(" ").length;
};

export const create_virtual_list = (id, target) => {
    const container = create_element(`<div class="virtual-list ${id}"></div>`);
    const list_container = create_element(`<div class="virtual-column-grid ${id}"></div>`);
    const fake_height = create_element(`<div class="fake-height" style="height: 100%;"></div>`);

    const list = {
        id,
        target,
        length: 0,
        container,
        list_container,
        fake_height,
        rows: 0,
        columns: 0,
        last_render: {},
        should_render: false,
        create: null,
        update: null,
        show: () => { container.style.display = "grid"; },
        hide: () => { container.style.display = "none"; },
        calc: async (callback) => {
            if (!list.create) return;
            const { element, id } = await list.create();

            const columns = get_column_amount(list_container);
            const element_size = get_element_size(element, id, true);
            const rows = Math.ceil(list.length / columns);

            fake_height.style.height = `${rows * element_size.height}px`;

            Object.assign(list, { rows, columns });
            if (callback) callback();
        },
        refresh: (force) => {
            if (force == true) {
                list.should_render = true;
                list.calc(list.refresh);
                return;
            }
            if (!list.should_render) return;
            render(id);
        },
        remove: (element) => {
            if (list.length > 0)  {
                list.length--;
                element.remove();
                list.refresh(true);
            }
        }
    };

    window.addEventListener("resize", () => { list.calc(); });
    container.addEventListener("scroll", debounce(list.refresh, 5));

    container.appendChild(list_container);
    container.appendChild(fake_height);

    target.appendChild(container);
    virtual_lists.set(id, list);
    return list;
};