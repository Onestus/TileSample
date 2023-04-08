# TileSample (v1.0)
[TShock](https://github.com/Pryaxis/TShock) plugin. Used to get information about the certain tile(block, wall, paints, coatings, wiring, actuation).

### Commands
`/sample (s[show]/g[ive])` - The main command. Will show all the information about the selected tile whether it is a wall, block, liquid or a wire.
- if the `/sample (s[how])` command is entered, will only show information about current tile.
- if the `/sample g[ive]` command is entered, will show info and give all things except wrenches and bottomless liquid buckets since they are used only to show information. However, if the tile is actuated or contains wires, stack of actuators or/and wires will be given to a player.

### Permissions
`tilesample.use` - Required to use the `sample` command.

`tilesample.admin` - Required to debug command & plugin.

### Example
![Example](https://i.ibb.co/GRTcdmD/95bz-Nfg-Epu-U.jpg)

### Options
:white_check_mark: Get info about one tile
- [ ] Get info about area
- [ ] Configure the issuance of helpful items
- [ ] Show furniture
