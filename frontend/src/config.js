import { indexed } from "./database/indexed.js";
import { ipc } from "./ipc/message.js";
import { debounce } from "./components/utils/utils.js";

const groups = [...document.querySelectorAll(".field-group")];

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
    // @TODO: since the c# side can handle single fields, send only the updated value
    await ipc.send("update_config", config);
};

// @NOTE: code is a little messy but works just fine
export const initialize_config = async () => {

    // get all saved fields from indexed db
    const saved_fields = await indexed.all("config");

    // update config object
    for (const [k, v] of Object.entries(saved_fields)) {
        config[k] = v;
    }

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
                console.log("updating field", value);
                field.value = value;
            }
        };
        
        if (field_type == "custom-file") {
            field.addEventListener("click", async (e) => {
                const result = await ipc.send("show_dialog", { type: "folder" });
                if (result?.path) {
                    await save_field(groups[i].id, result.path);
                    update_field(result.path);
                }
            });
        } else if (field_type == "checkbox") {
            field.addEventListener("change", (e) => {
                save_field(groups[i].id, e.target.checked);
            });
        } else {
            field.addEventListener("input", debounce((e) => {
                save_field(groups[i].id, e.target.value);
            }, 200));
        }

        // update the field text to use the current value
        if (config[id]) update_field(config[id]);
    }
    
    return config;
};