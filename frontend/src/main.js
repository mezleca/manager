import { initialize_config, config, load_files } from "./config.js";
import { ipc } from "./ipc/message.js";

const tabs = document.querySelectorAll(".tab");
const tab_contents = document.querySelectorAll(".tab-content");
const expand_buttons = document.querySelectorAll(".expand-btn");
const search_expandeds = document.querySelectorAll(".search-expanded");

const open_links_on_browser = () => {
	const elements = [...document.querySelectorAll("a")];
	
	for (let i = 0; i < elements.length; i++) {
		const element = elements[i];
		const href = element.getAttribute("href");
		
		element.addEventListener("click", (e) => {
			e.preventDefault();
			ipc.send("open", { url: href });
		});
	}
};

// setup tabs
const switch_tab = (id) => {
	tabs.forEach((tab) => tab.classList.remove("active"));
	tab_contents.forEach((content) => content.classList.remove("active"));
	document.querySelector(`.tab[data-tab="${id}"]`)?.classList.add("active");
	document.getElementById(`${id}-content`)?.classList.add("active");
};

tabs.forEach((tab) => {
	tab.addEventListener("click", async () => {
		switch_tab(tab.getAttribute("data-tab"));
	});
});

// setup expandable buttons
expand_buttons.forEach((btn, index) => {
    btn.addEventListener("click", () => {
        btn.classList.toggle("active");
        search_expandeds[index].classList.toggle("active");
    });
});

// remove it until i implement window movement with chromeless mode
document.querySelector(".window-controls").style.display = "none";

// setup message handler
window.external.receiveMessage(ipc.handle_message);

// disable page zoom
window.addEventListener("mousewheel", (e) => { if (e.ctrlKey) e.preventDefault() }, { passive: false });

const test_get_beatmaps = async () => {

	const collection = await ipc.send("get_collection", { name: "mzle" });

	const beatmaps = [], promises = [];
	const start = performance.now();

	for (let i = 0; i < 16; i++) {

		if (i >= collection.hashes.length) {
			break;
		}

		promises.push(ipc.send("get_beatmap", { md5: collection.hashes[i] }));
	}

	const result = await Promise.all(promises);
	beatmaps.push(...result);

	const end = performance.now();
	console.log(`took ${(end - start).toFixed(2)} ms`);
};

(async () => {

	// get saved values / add listeners
	await initialize_config();
	await ipc.send("update_config", config);
	await load_files();

	const collections = await ipc.send("get_collections");

	if (collections.success) {

		const name = collections.collections[0].name;
		console.log(name);
		const set_collection = await ipc.send("set_collection", { name: name });
	
		if (!set_collection.success) {
			console.log("failed to set collection");
			return;
		}

		const non_filtered_beatmaps = await ipc.send("get_beatmaps", { index: 0, filtered: false });
		console.log("non filtered", non_filtered_beatmaps);
		const filtered_beatmaps = await ipc.send("get_beatmaps", { index: 0, filtered: true, name: name });
		console.log("filtered", filtered_beatmaps);
		const non_filtered_beatmaps2 = await ipc.send("get_beatmaps", { index: 50, amount: 999 }); // clamp to max amount available
		console.log(non_filtered_beatmaps2);
	}

	document.querySelector(".add-btn").addEventListener("click", test_get_beatmaps);
	open_links_on_browser();
})();
