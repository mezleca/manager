# osu manager
temp repo for [osu-stuff](https://github.com/mezleca/osu-stuff) v2<br>
had a lot of problems trying to manage branchs so will use this repo for now.<br>
also using c# (Photino) instead of go (wails)

## wanna help osu-manager development?
- bug report and pull requests are always welcome, just make sure to include what you're adding or the issue you're reporting.

# TODO LIST
## * = priority

#### webview / window
- [ ] figure out a way to implement frameless window (well, theres an option to enable but i have no idea how to make the window move on drag)
- [x] *window icons
- [x] *fix window not opening with nvidia drivers
- [x] tell the user if we need to install de system webview (windows does that automatically)

#### realm / parser 
- [x] *realm connection function
- [ ] osdb parser
- [x] *collections.db parser 
- [x] *osu.db parser
- [ ] add real time realm update (if possible)

#### beatmaps / collections
- [ ] function to get sr
- [ ] function to get common bpm
- [ ] filter system
- [x] *beatmap downloader (still need more testing)

#### ipc
- [x] *send / received typed data using webview message functions + messagepack
- [ ] *multiple file sellection support for dialog
- [ ] return file buffer on dialog (dont even know when im gonna use this tbh)
- [ ] *open links on browser

#### frontend related stuff
- [ ] *get missing beatmaps
- [ ] *export collection
- [ ] *export beatmaps
- [ ] *import collections (osdb and stable db)
- [ ] *get from player
- [ ] *get from osu!Collector
- [ ] merge collections
- [ ] remove collections
- [ ] discover tab (get new beatmaps using the osu! api)
- [ ] *browse tab (browse all downloaded beatmaps)
- [x] better color scheme (more darker)
- [ ] better beatmap card
- [x] *virtual list
- [x] *popup system
- [x] *context menu
- [x] config system using indexed db
- [ ] progress box
- [ ] *download tab
- [ ] beatmap preview (offline if possible)

#### binary / extra
- [x] *build script
- [x] workflow for auto release on tag creation (windows / linux binaries)
- [ ] bundle / minify all of the js files using webpack
- [ ] notification system
- [x] *c# fetch helper (get and download)
