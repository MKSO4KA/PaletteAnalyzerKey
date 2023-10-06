# [Steam WorkShop](https://steamcommunity.com/sharedfiles/filedetails/?id=2989170867)

## PaletteAnalyzerKey
The modification scans an area of the world defined in the area from 0:0 to maximum tiles (must be specified in the config). The scanned area goes through one stage of sorting and filtering (Exceptions and Flares), and then the filtered palette goes to the same directory where the configuration file is located. **To start the mod**, you need to press the F5 key (or any other key specified in the control settings). The mod will display information in the chat that the config file does not exist or is empty. You need to fill out the file and run the mod again (press F5. This can be done without leaving the game)
The mod was created based on the QS(Quick Save) mod.
## Notes
The first time you click on the shortcut button, a configuration file with four lines is created. Subsequent clicks will not recreate the file. In the file, you need to add key values (after >), namely, maximum tiles and paths to Exceptions and to Torches.
  ```
      MaxTile X> 200
      MaxTile Y> 1100
      Torchs path> C:\torchs.txt
      Exceptions path> C:\exceptions.txt
  ```
## A short guide to using the mod.
### 1. Main Files
For the mod to work, **you will need three files**, two of which you need to specify the path to in the config. 
The exception file is designed this way: each line characterizes a specific pixel of the game (tile or wall). You can understand whether it is a block or not by the number before the colon (":"), these are the values of truth or falsehood presented in integer form. (1 - block, 0 - wall)
```
      1:54
      0:21
```
A torch file is not really a torch file. Torches in this case are any blocks that will fall if placed on Air, but they can be held on the wall. Each line of this file represents the id of a block, such as torch(4).
```
      4
      136
```

> Exception and Torchs files are configured by you personally, I just attached my files to the release.
---
The world file is the main file that is needed to create the palette. This file must be downloaded from the release. It is from this that two variables are taken **(maxtileX and maxtileY)**. If you download my world from the release, then all the necessary exceptions have already been taken into account (almost), and stone walls have been placed under the torch blocks. **By downloading my file, you also don’t have to change the values of the two variables mentioned above.**
> You can create the world file yourself, using my Tedit plugin or using the strategy described by [xXCrypticNightXx](https://forums.terraria.org/index.php?members/xxcrypticnightxx.98963/).

