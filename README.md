# osu manager
temp repo for [osu-stuff](https://github.com/mezleca/osu-stuff) v2<br>
had a lot of problems trying to manage branchs so will use this repo for now.<br>
also using c# (Photino) instead of go (wails)

# TODO
- [x] ipc
    - [x] logic to receive / send messages with messagepack
        - [x] base
        - [x] frontend doesn't have a handler
        - [x] make it stable enough to use (works well enough)
- [ ] window
    - [ ] fix window not opening on wayland (added a hotfix but theres prob a better way to do this)
    - [ ] figure out a way to implement chromeless (frameless) mode (well, theres an option to enable but i have no idea how to make the window move on drag)
- [ ] osu! stable parser
- [ ] osdb parser
- [ ] osu! lazer realm connection
- [ ] add real time realm update (if possible)
- [ ] discover tab (from osu! api)
- [ ] browse beatmaps tab (from osu! local database)
- [ ] build script
- [ ] github workflow
- [ ] move the rest of v1 logic to c# (virtual list, filters, get_sr, get_bpm, etc...)
