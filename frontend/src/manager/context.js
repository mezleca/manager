import { create_element } from "../components/utils/utils.js";

class ContextMenu {

    constructor() {
        this.menus = [];
        this.timeout = null;
        this.body = document.body;
        this.menu_data = new WeakMap(); // store menu data
        
        document.addEventListener("click", () => this.close_all());
        document.addEventListener("keydown", (e) => {
            if (e.key == "Escape") { 
                this.close_all(); 
            }
        });
    }

    show = (x, y, data) => {
        this.close_all();
        const menu = this.create_menu(data, 0);
        console.log("menu")
        this.position_menu(menu, x, y);
        this.show_menu(menu);
    }

    close_all = () => {
        // remove all menu elements
        for (let i = 0; i < this.menus.length; i++) {
            this.menus[i].remove();
        }
        this.clear_timeout();
        this.menus.length = 0;
    }

    close_from_level = (level) => {
        this.clear_timeout();
        const filtered_menus = [];
        
        for (let i = 0; i < this.menus.length; i++) {
            const menu = this.menus[i];
            const menu_level = parseInt(menu.dataset.level);
            
            if (menu_level >= level) {
                menu.remove();
            } else {
                filtered_menus.push(menu);
            }
        }
        
        this.menus = filtered_menus;
    }

    clear_timeout = () => {
        if (this.timeout) {
            clearTimeout(this.timeout);
            this.timeout = null;
        }
    }

    create_menu = (data, level) => {
        const fragment = document.createDocumentFragment();
        const menu = create_element(`<div class="context-menu" data-level="${level}"></div>`);

        for (let i = 0; i < data.length; i++) {
            const item = data[i];
            if (item.type == "separator") {
                const sep = create_element(`<div class="menu-separator"></div>`);
                fragment.appendChild(sep);
            } else {
                fragment.appendChild(this.create_item(item, level));
            }
        }
        
        menu.appendChild(fragment);
        console.log(menu);
        return menu;
    }

    create_item = (item, level) => {
        const disabled_class = item.disabled ? "disabled" : "";
        const el = create_element(`<div class="menu-item ${disabled_class}"></div>`);
        
        // store item data for event handling
        this.menu_data.set(el, { item, level });

        if (item.icon) {
            const icon = create_element(`<div class="icon">${item.icon}</div>`);
            el.appendChild(icon);
        }

        const text = create_element(`<div class="text">${item.label}</div>`);
        el.appendChild(text);

        if (item.submenu) {
            const arrow = create_element(`<div class="arrow"></div>`);
            el.appendChild(arrow);
            
            el.addEventListener("mouseenter", () => {
                this.clear_timeout();
                this.timeout = setTimeout(() => {
                    this.show_submenu(el, item.submenu, level);
                }, 100);
            });
        }

        // single click handler for all items
        if (!item.disabled && item.callback && !item.submenu) {
            el.addEventListener("click", this.handle_item_click);
        }

        return el;
    }

    handle_item_click = (e) => {
        e.stopPropagation();
        const item_data = this.menu_data.get(e.currentTarget);
        if (item_data && item_data.item.callback) {
            item_data.item.callback();
            this.close_all();
        }
    }

    show_submenu = (parent, data, parent_level) => {
        const level = parent_level + 1;
        this.close_from_level(level);
        
        const submenu = this.create_menu(data, level);
        this.position_submenu(submenu, parent);
        this.show_menu(submenu);
    }

    show_menu = (menu) => {
        this.body.appendChild(menu);
        this.menus.push(menu);
        requestAnimationFrame(() => menu.classList.add("show"));
    }

    position_menu = (menu, x, y) => {
        menu.style.visibility = "hidden";
        menu.style.position = "absolute";
        this.body.appendChild(menu);
        
        const rect = menu.getBoundingClientRect();
        
        const max_x = window.innerWidth - 10;
        const max_y = window.innerHeight - 10;
        
        // avoid window collision
        let final_x = Math.min(x, max_x - rect.width);
        let final_y = Math.min(y, max_y - rect.height);
        
        final_x = Math.max(10, final_x);
        final_y = Math.max(10, final_y);
        
        menu.style.left = `${final_x}px`;
        menu.style.top = `${final_y}px`;
        menu.style.visibility = "visible";
        
        // remove temp element
        this.body.removeChild(menu);
    }

    position_submenu = (submenu, parent) => {
        // yep
        submenu.style.visibility = "hidden";
        submenu.style.position = "absolute";

        this.body.appendChild(submenu);

        const parent_rect = parent.getBoundingClientRect();
        const submenu_rect = submenu.getBoundingClientRect();
        
        const max_x = window.innerWidth - 10;
        const max_y = window.innerHeight - 10;
        
        let x = parent_rect.right + 5;
        let y = parent_rect.top;
        
        // check if submenu fits on right side
        if (x + submenu_rect.width > max_x) {
            x = parent_rect.left - submenu_rect.width - 5;
        }
        
        // check if submenu fits vertically
        if (y + submenu_rect.height > max_y) {
            y = max_y - submenu_rect.height;
        }
        
        // ensure minimum margins
        x = Math.max(10, Math.min(x, max_x - submenu_rect.width));
        y = Math.max(10, y);
        
        submenu.style.left = `${x}px`;
        submenu.style.top = `${y}px`;
        submenu.style.visibility = "visible";
        
        // remove temp element
        this.body.removeChild(submenu); 
    }
}

// will reuse later
export const context_menu = new ContextMenu();