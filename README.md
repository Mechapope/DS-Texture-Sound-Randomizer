# DS-Texture-Sound-Randomizer
Randomizes the textures and sounds for Dark Souls 1 PTDE. Probably doesn't work on remaster but I don't own it so I can't test it. This is a new version of my Dark Souls Asset Randomizer rebuilt from scratch, this time it unpacks/repacks the textures and sounds from the games files so no DSFix or huge download required. 

Requires your game to be unpacked by [Unpack Dark Souls for Modding](https://github.com/HotPocketRemix/UnpackDarkSoulsForModding).

If you get an infinite loading screen when trying to load into the game, try using the Fix main sound file option in the program.

The application will freeze/become unresponsive while it is randomizing. It is still working, I promise. Just wait. I tried to fix this but threading is hard.

Note that the sound re-packing is very slow. The sound inserting is done by DSSI, an external library, and it can't go any faster than it does. There is no way I can make it go faster and believe me I tried. Expect a full randomization to take a couple hours.


# How 2 use
Unzip and run the exe
Point it at your game folder
Click Create Backups and Extract Game Files if this is your first run
Select your options and hit Go
Wait a long time for it to finish
Click Open Output Folder and copy the files to your Dark Souls DATA directory


# Credits
[DSSI by RavagerChris37](https://www.nexusmods.com/darksouls/mods/1193)
[fsbext](https://aluigi.altervista.org/search.php?src=fsbext)
[Texconv](https://github.com/Microsoft/DirectXTex/wiki/Texconv)
[DSR-TPUP](https://github.com/JKAnderson/DSR-TPUP) that I copied a bunch of code from
