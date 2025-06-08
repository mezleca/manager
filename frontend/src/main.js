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

(async () => {
	await initialize_config();
	await ipc.send("update_config", config);
	await load_files();

	open_links_on_browser();
})();
