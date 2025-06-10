import { create_element, debounce } from "./utils.js";

// @TODO: ts is not optimized at all

export const virtual_lists = new Map();

const element_size_cache = new Map();
const elements_cache = new Map();

const BUFFER = 2;

const element_pool = new Map();

const get_pooled_element = (id, create) => {
    if (!element_pool.has(id)) {
        element_pool.set(id, []);
    }
    
    const pool = element_pool.get(id);
    return pool.pop() || create();
};

const return_to_pool = (id, element) => {
    if (!element_pool.has(id)) element_pool.set(id, []);
    element_pool.get(id).push(element);
};

export const get_element_size = (element, id, force = false) => {
    if (!force && element_size_cache.has(id)) {
        return element_size_cache.get(id);
    }

    const fragment = document.createDocumentFragment();
    const new_el = element.cloneNode(true);
    
    Object.assign(new_el.style, { 
        visibility: "hidden", 
        position: "absolute", 
        top: "-9999px"
    });

    fragment.appendChild(new_el);
    document.body.appendChild(fragment);
    
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

    const fragment = document.createDocumentFragment();
    const visible_elements = [];

    if (last_render.elements) {
        for (let i = 0; i < last_render.elements.length; i++) {
            const el = last_render.elements[i];
            if (el.dataset.pool_id) return_to_pool(el.dataset.pool_id, el);
        }
    }
    
     for (let i = start_index; i < end_index; i++) {
        const item_id = virtual_list.get(i);
        let data = null;
        
        if (elements_cache.has(item_id)) {
            data = elements_cache.get(item_id);
        } else {
            const pooled = get_pooled_element(item_id, () => virtual_list.create(i));
            data = pooled.element ? pooled : virtual_list.create(i);
            
            if (data.element) {
                data.element.dataset.pool_id = item_id;
            }
            
            elements_cache.set(item_id, data);
        }

        if (virtual_list.update && data.needs_update != false) {
            virtual_list.update({ ...data, index: i });
            data.needs_update = false;
        }

        if (data.element) {
            fragment.appendChild(data.element);
            visible_elements.push(data.element);
        }
    }
    
    virtual_list.list_container.style.transform = `translate3d(0, ${first_row * item_height}px, 0)`;
    virtual_list.list_container.innerHTML = "";
    virtual_list.list_container.appendChild(fragment);
    
    virtual_list.last_render = { 
        start: start_index, 
        end: end_index, 
        total_length: length,
        elements: visible_elements 
    };
};

export const get_column_amount = (container) => {
    const computed = window.getComputedStyle(container);
    return computed.getPropertyValue("grid-template-columns").split(" ").length;
};

export const create_virtual_list = (id, target) => {
    const container = create_element(`<div class="virtual-list ${id}"></div>`);
    const list_container = create_element(`<div class="virtual-column-grid ${id}"></div>`);
    const fake_height = create_element(`<div class="fake-height" style="height: 100%;"></div>`);

    container.style.willChange = "scroll-position";
    list_container.style.willChange = "transform";

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
        get: null,
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
                container.scrollTop = 0; // dont preserve scroll
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

    container.addEventListener("scroll", debounce(() => {
        list.refresh();
    }, 10));
    
    window.addEventListener("resize", debounce(() => {
        list.calc(() => list.refresh(true));
    }, 50));

    container.appendChild(list_container);
    container.appendChild(fake_height);

    target.appendChild(container);
    virtual_lists.set(id, list);
    return list;
};