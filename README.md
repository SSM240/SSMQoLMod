# SSM's Quality of Life Mod
Celeste mod that adds some various quality of life changes and console commands.

This repo is just for the code - see [the GameBanana page](https://gamebanana.com/mods/53706) for more info and releases.

### Note for building

Unlike most Celeste mods, the csproj file references the XNA version of the game rather than the FNA version, 
so you may need to change the references locally if you're using the FNA version.
Removing all the `Microsoft.Xna.Framework` DLLs and referencing `FNA.dll` instead should suffice.
