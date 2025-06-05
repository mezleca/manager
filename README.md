# osu manager
temp repo for [osu-stuff](https://github.com/mezleca/osu-stuff) v2<br>
had a lot of problems trying to manage branchs so will use this repo for now.<br>
also using c# (Photino) instead of go (wails)

# TODO
- [x] ipc
    - [x] logic to receive / send messages with messagepack
        - [x] base
        - [x] frontend doesn't have a handler
        - [x] make it stable enough to use
- [ ] window
    - [ ] fix window not opening on wayland (added a hotfix but theres prob a better way to do this)
    - [ ] figure out a way to implement chromeless (frameless) mode (well, theres an option to enable but i have no idea how to make the window move on drag)
- [ ] osu! stable parser
    - [x] collections.db
    - [ ] osu!.db
- [ ] config tab
    - [x] html
    - [ ] open links on your browser
    - [ ] more js to check valid paths, rename file selectors, enable checkboxes, etc...
- [ ] osdb parser
- [x] osu! lazer realm connection
- [ ] osu! lazer helpers (get beatmap, get collection, etc...)
- [ ] add real time realm update (if possible)
- [ ] discover tab (from osu! api)
- [ ] browse beatmaps tab (from osu! local database)
- [ ] download manager (c#)
- [ ] build script
- [ ] github workflow (windows / linux)
- [ ] move the v1 frontend logic (virtual list, etc...) 
- [ ] move the rest of v1 backend logic to c# (filters, get_sr, get_bpm, etc...)
