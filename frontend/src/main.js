import { Ipc } from "./ipc/message.js";

const tabs = document.querySelectorAll(".tab");
const tab_contents = document.querySelectorAll(".tab-content");
const expand_buttons = document.querySelectorAll(".expand-btn");
const search_expandeds = document.querySelectorAll(".search-expanded");

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
window.external.receiveMessage(Ipc.handleMessage);

// disable page zoom
window.addEventListener("mousewheel", (e) => { if (e.ctrlKey) e.preventDefault() }, { passive: false });
