# osu manager
temp repo for [osu-stuff](https://github.com/mezleca/osu-stuff) v2<br>
had a lot of problems trying to manage branchs so will use this repo for now.<br>
also using c# (Photino) instead of go (wails)

# TODO
- [ ] reader
    - [ ] osu! stable parser
    - [ ] osdb parser
    - [ ] osu! lazer realm connection
- [ ] ipc
    - [ ] logic to receive / send messages with messagepack
        - [x] base
        - [ ] frontend doesn't have a handler
        - [ ] make it stable enough to use
    - [ ] implement handler to get a x ammount of beatmaps (to use on virtual list)
- [ ] window
    - [ ] fix window not opening on wayland (not sure if is nvidia only, also current method can issues)
    - [ ] figure out a way to implement chromeless (frameless) mode (well, theres an option to enable but i have no idea how to make the window move on drag)
- [ ] discover tab (from the osu! api)
- [ ] browse beatmaps tab (from your osu! database)
- [ ] build script
- [ ] github workflow
- [ ] move the rest of v1 logic to c# (filters, get_sr, get_bpm, etc...)