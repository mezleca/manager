# osu manager
temp repo for [osu-stuff](https://github.com/mezleca/osu-stuff) v2<br>
had a lot of problems trying to manage branchs so will use this repo for now.<br>
also using c# (Photino) instead of go (wails)

## wanna help osu-manager development?
- bug report and pull requests are always welcome, just make sure to include what you're adding or the issue you're reporting.

# TODO LIST
## not priority but also needed for v2
- [ ] window: figure out a way to implement chromeless (frameless) mode (well, theres an option to enable but i have no idea how to make the window move on drag)
- [x] window: icons on build
- [ ] osdb: .osdb parser (literally just copy & paste from the collection manager project)
- [x] workflow: auto release on tag creation (windows / linux binaries)
- [ ] lazer: add real time realm update (if possible)
- [ ] app: discover tab (from osu! api)
- [ ] ipc: multiple file sellection support on dialog
- [ ] ipc: return file buffer (ehh, dont even know when im gonna use this tbh)
- [x] process: open links on browser
- [ ] app: bundle / minify all of the js files using webpack or something
- [ ] css: do some minor changes to beatmap-card (again...)

## need this for the rest of the features
- [x] ipc: handlers on both c# & js to send typed information using messagepack (well, js dont need types but you get it) 
- [x] ipc: make it stable
- [x] window: fix window not opening with nvidia drivers
- [x] parser: collections.db
- [x] parser: osu!.db
- [x] stable: helpers (get beatmap, get collection, etc...)
- [x] lazer: class to get realm instance
- [x] lazer: helpers (get beatmap, get collection, etc...)
- [x] config: html / css
- [x] config: load / save
- [x] ipc: handlers to open dialogs
- [ ] app: browse beatmaps tab (from osu! local database)
- [ ] app: beatmap downloader (needs more testing)
- [x] app: fetch helper (get data / download binary)
- [x] app: build script
- [ ] beatmaps: filter system
- [ ] beatmaps: helpers (get sr, get bpm, etc...)
- [ ] app: move v1 frontend logic to js
    - [x] virtual list
    - [x] popup system
    - [x] context menu
    - [ ] progress box
    - [ ] download tab
