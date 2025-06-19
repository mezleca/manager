import { indexed } from "./database/indexed.js";
import { ipc } from "./ipc/message.js";
import { debounce } from "./components/utils/utils.js";
import { create_alert } from "./components/utils/popup.js";
import { show_collections } from "./manager/manager.js";

const groups = [...document.querySelectorAll(".field-group")];
const reload = document.querySelector(".reload-config");

export const config = {
    osu_id: "",
    osu_secret: "",
    token: "",
    stable_path: "",
    lazer_path: "",
    stable_songs_path: "",
    lazer: false,
    local: false,
};

export const save_field = async (name, value) => {
    indexed.save("config", name, value);
    config[name] = value;
    await ipc.send("update_config", config);
};

const load_config_data = async () => {
    const saved_fields = await indexed.all("config");
    for (const [k, v] of Object.entries(saved_fields)) {
        config[k] = v;
    }
};

// @TOFIX: not sure if the problem is here or on the backend but after updating the stable path / songs path, 
// i still need to reload the page in order to be able to read osu! data 
const setup_field_handlers = () => {

    for (let i = 0; i < groups.length; i++) {

        const id = groups[i].id;
        const field = groups[i].querySelector("input");
        const field_type = field.getAttribute("type");
        
        const update_field = (value) => {
            if (field_type == "custom-file") {
                groups[i].querySelector(".text").textContent = value;
            } else if (field_type == "checkbox") {
                field.checked = value;
            } else {
                field.value = value;
            }
        };
        
        if (field_type == "custom-file") {
            field.addEventListener("click", async () => {
                const result = await ipc.send("show_dialog", { type: "folder" });
                if (result?.path) {
                    await save_field(groups[i].id, result.path);
                    update_field(result.path);
                }
            });
        } 
        else if (field_type == "checkbox") {
            field.addEventListener("change", (e) => {
                save_field(groups[i].id, e.target.checked);
            });
        } 
        else {
            field.addEventListener("input", debounce((e) => {
                save_field(groups[i].id, e.target.value);
            }, 200));
        }

        // update the field text to use the current value
        if (config[id]) {
            update_field(config[id]);
        }
    }
};

export const initialize_config = async () => {
    await load_config_data();
    setup_field_handlers();
};

export const reload_config = async () => {

    await load_config_data();

    for (let i = 0; i < groups.length; i++) {

        const id = groups[i].id;
        const field = groups[i].querySelector("input");
        const field_type = field.getAttribute("type");

        if (config[id]) {
            if (field_type == "custom-file") {
                groups[i].querySelector(".text").textContent = config[id];
            } 
            else if (field_type == "checkbox") {
                field.checked = config[id];
            } 
            else {
                field.value = config[id];
            }
        }
    }

    await show_collections();
};

export const load_db = async () => {
    const db_result = await ipc.send("load_database");
    if (!db_result.success) {
        create_alert("failed to load database", { type: "error" });
    }
};

export const load_cl = async () => {
    const db_result = await ipc.send("load_collections");
    if (!db_result.success) {
        create_alert("failed to load collection", { type: "error" });
    }
};

export const load_files = async () => {
    const results = await Promise.all([load_db(), load_cl()]);
    return !results.includes(false)
};

// reload on click
reload.addEventListener("click", debounce(reload_config, 100));