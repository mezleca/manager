import { initialize_config, config, load_files } from "./config.js";
import { ipc } from "./ipc/message.js";
import { show_collections } from "./manager/manager.js";

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

// removd until i implement window movement with chromeless mode
document.querySelector(".window-controls").style.display = "none";
window.external.receiveMessage(ipc.handle_message);

// disable page zoom
window.addEventListener("mousewheel", (e) => { if (e.ctrlKey) e.preventDefault() }, { passive: false });

// config description links
open_links_on_browser();

// disable default context menu
window.addEventListener("contextmenu", (event) => event.preventDefault());

(async () => {
	await initialize_config();
	await ipc.send("update_config", config);

	const load_result = await load_files();

	if (!load_result) {
		return;
	}

	await show_collections();
})();
