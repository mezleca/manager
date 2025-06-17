import { create_element, debounce } from "./utils.js";

export const virtual_lists = new Map();

const element_size_cache = new Map();
const elements_cache = new Map();
const BUFFER_SIZE = 2;

const element_pool = new Map();

const get_pooled_element = (id, create_fn) => {
    if (!element_pool.has(id)) {
        element_pool.set(id, []);
    }
    
    const pool = element_pool.get(id);
    return pool.pop() || create_fn();
};

const return_to_pool = (id, element) => {
    if (!element_pool.has(id)) {
        element_pool.set(id, []);
    }
    element_pool.get(id).push(element);
};

export const get_element_size = (element, id, force_recalc = false) => {
    if (!force_recalc && element_size_cache.has(id)) {
        return element_size_cache.get(id);
    }

    const temp_element = element.cloneNode(true);
    
    Object.assign(temp_element.style, { 
        visibility: "hidden", 
        position: "absolute", 
        top: "-9999px"
    });

    document.body.appendChild(temp_element);

    const rect = temp_element.getBoundingClientRect();
    temp_element.remove();
    
    element_size_cache.set(id, rect);
    return rect;
};

export const render = (list_id, force_render = false) => {
    const virtual_list = virtual_lists.get(list_id);

    // ensure we have a valid list
    if (!virtual_list || !virtual_list.length) {
        return;
    }

    const { container, fake_height, rows, columns, length, last_render } = virtual_list;
    
    // calc visible area
    const scroll_top = container.scrollTop;
    const viewport_height = container.clientHeight;
    const item_height = fake_height.clientHeight / rows;

    // ccalc which rows to render + buffer
    const first_visible_row = Math.max(0, Math.floor(scroll_top / item_height) - BUFFER_SIZE);
    const last_visible_row = Math.min(rows, Math.ceil((scroll_top + viewport_height) / item_height) + BUFFER_SIZE);

    const start_index = first_visible_row * columns;
    const end_index = Math.min(length, last_visible_row * columns);

    // skip render if we have the same range as last time
    if (!force_render && last_render && 
        start_index >= last_render.start && 
        end_index <= last_render.end && 
        length == last_render.total_length) {
        return;
    }

    const fragment = document.createDocumentFragment();
    const current_elements = [];

    if (last_render.elements) {
        for (let i = 0; i < last_render.elements.length; i++) {
            const el = last_render.elements[i];
            if (el.dataset.pool_id) {
                return_to_pool(el.dataset.pool_id, el);
            }
        }
    }
    
    // create/update visible elements
    for (let i = start_index; i < end_index; i++) {
        const item_id = virtual_list.get(i);
        let element_data = null;
        
        // check if we cached the element, if not create a new one
        if (elements_cache.has(item_id)) {
            element_data = elements_cache.get(item_id);
        } else {
            const pooled = get_pooled_element(item_id, () => virtual_list.create(i));
            element_data = pooled.element ? pooled : virtual_list.create(i);
            
            if (element_data.element) {
                element_data.element.dataset.pool_id = item_id;
            }
            
            elements_cache.set(item_id, element_data);
        }

        // update element if needed
        if (virtual_list.update && element_data.needs_update != false) {
            virtual_list.update({ ...element_data, index: i });
            element_data.needs_update = false;
        }

        if (element_data.element) {
            fragment.appendChild(element_data.element);
            current_elements.push(element_data.element);
        }
    }
    
    // update dom
    virtual_list.list_container.style.transform = `translate3d(0, ${first_visible_row * item_height}px, 0)`;
    virtual_list.list_container.innerHTML = "";
    virtual_list.list_container.appendChild(fragment);
    
    // cache more info
    virtual_list.last_render = { 
        start: start_index, 
        end: end_index, 
        total_length: length,
        elements: current_elements 
    };
};

export const get_column_count = (container) => {
    const computed_style = window.getComputedStyle(container);
    return computed_style.getPropertyValue("grid-template-columns").split(" ").length;
};

export const create_virtual_list = (list_id, target_element) => {
    const container = create_element(`<div class="virtual-list ${list_id}"></div>`);
    const list_container = create_element(`<div class="virtual-column-grid ${list_id}"></div>`);
    const fake_height = create_element(`<div class="fake-height" style="height: 100%;"></div>`);

    // scroll optmization?
    container.style.willChange = "scroll-position";
    list_container.style.willChange = "transform";

    const virtual_list = {
        id: list_id,
        target: target_element,
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
            if (!virtual_list.create) {
                return;
            }
            
            const { element, id } = await virtual_list.create();
            const columns = get_column_count(list_container);
            const element_size = get_element_size(element, id, true);
            const rows = Math.ceil(virtual_list.length / columns);

            fake_height.style.height = `${rows * element_size.height}px`;

            Object.assign(virtual_list, { rows, columns });
            
            if (callback) {
                callback();
            }
        },
        
        // trigger render if needed
        refresh: (force_refresh = false) => {
            if (force_refresh == true) {
                virtual_list.should_render = true;
                virtual_list.calc(virtual_list.refresh);
                // reset scroll position
                container.scrollTop = 0; 
                return;
            }
            
            if (!virtual_list.should_render) {
                return;
            }
            
            render(list_id);
        },
        
        // remove item from list
        remove: (element) => {
            if (virtual_list.length > 0) {
                virtual_list.length--;
                element.remove();
                virtual_list.refresh(true);
            }
        }
    };

    // update list on scroll 
    container.addEventListener("scroll", debounce(() => {
        virtual_list.refresh();
    }, 10));
    
    // handle window resize
    window.addEventListener("resize", debounce(() => {
        virtual_list.calc(() => virtual_list.refresh(true));
    }, 50));

    container.appendChild(list_container);
    container.appendChild(fake_height);
    target_element.appendChild(container);
    
    virtual_lists.set(list_id, virtual_list);
    return virtual_list;
};