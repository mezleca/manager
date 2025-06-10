import { create_collection_card } from "../components/cards/collection.js";
import { create_beatmap_card, update_beatmap_card } from "../components/cards/beatmap.js";
import { create_alert } from "../components/utils/popup.js";
import { ipc } from "../ipc/message.js";
import { create_virtual_list } from "../components/utils/virtual.js";
import { get_beatmap } from "../components/utils/utils.js";

export let collections = new Map();

export const get_selected_collection = () => {
    const all_collections = [...document.querySelectorAll(".collection-item")];
    
    for (let i = 0; i < all_collections.length; i++) {
        const collection = all_collections[i];
        
        if (collection.classList.contains("active")) {
            const name = collection.querySelector(".collection-name").innerText;
            return { name: name, element: collection };
        }
    }

    return { name: "", element: null };
};

export const manager_list = create_virtual_list("collection-list", document.querySelector(".manager-beatmaps-container"));

// get collections, create listeners, etc...
export const initialize_collections = async () => {
    const container = document.querySelector(".collections");
    const result = await ipc.send("get_collections");

    if (!result.success) {
        create_alert("failed to get collections", { type: "error" });
        return;
    }

    const cards = [];

    for (let i = 0; i < result.collections.length; i++) {
        const collection = result.collections[i];
        const new_card = create_collection_card(crypto.randomUUID(), collection.name, collection.size);

        new_card.container.addEventListener("click", async () => {
            // remove active from old element
            document.querySelector(".collection-item.active")?.classList.remove("active");
            new_card.container.classList.add("active");

            manager_list.create = ((index) => {
                return { element: create_beatmap_card() };
            });

            manager_list.update = (async ({ element, index }) => {
                const hash = collection.hashes[index];
                const beatmap = await get_beatmap(hash);

                element.addEventListener("click", () => {
                    collection.hashes.splice(index, 1); 
                    manager_list.remove(element);
                });

                update_beatmap_card(element, beatmap);
            });

            manager_list.length = collection.size;
            manager_list.refresh(true);
        });

        cards.push(new_card.container);
        collections.set(collection.name, collection);
    }

    // replace old elements with the new ones
    container.replaceChildren(...cards);
};