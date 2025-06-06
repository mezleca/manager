# osu manager
temp repo for [osu-stuff](https://github.com/mezleca/osu-stuff) v2<br>
had a lot of problems trying to manage branchs so will use this repo for now.<br>
also using c# (Photino) instead of go (wails)

## wanna help osu-manager development?
- bug report and pull requests are always welcome, just make sure to include what you're adding or the issue you're reporting.

# TODO LIST
## not priority but needed to v2 release
- [ ] window: figure out a way to implement chromeless (frameless) mode (well, theres an option to enable but i have no idea how to make the window move on drag)
- [ ] window: uhhh, how can i actually use icons (linux doenst even show on the window title lol)?
- [ ] osdb: .osdb parser (literally just copy & paste from the collection manager project)
- [ ] workflow: auto release on tag creation (windows / linux binaries)
- [ ] lazer: add real time realm update (if possible)
- [ ] app: discover tab (from osu! api)
- [ ] ipc: multiple file sellection support on dialog
- [ ] ipc: return file buffer (ehh, dont even know when im gonna use this tbh)
- [x] process: open links on browser
- [ ] app: bundle / minify all of the js files using webpack or something

## need this to add the rest of the features
- [x] ipc: handlers on both c# & js to send typed information using messagepack (well, js dont need types but you get it) 
- [x] ipc: make it stable
- [x] window: fix window not opening with nvidia drivers
- [x] parser: collections.db
- [ ] parser: osu!.db
- [ ] stable: helpers (get beatmap, get collection, etc...)
- [x] lazer: class to get realm instance
- [ ] lazer: helpers (get beatmap, get collection, etc...)
- [x] basic config stuff (priority)
- [x] config: html / css
- [x] ipc: handlers to open dialogs
- [ ] app: browse beatmaps tab (from osu! local database)
- [ ] app: download manager
- [ ] app: build script
- [ ] app: move the v1 frontend logic (virtual list, etc...) 
- [ ] app: move the rest of v1 backend logic to c# (filters, get_sr, get_bpm, etc...)
