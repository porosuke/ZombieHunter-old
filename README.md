日本語の説明は [README_JPN.md](./README_JPN.md) をご覧ください。

# Zombie_Hunter

![header](/header.png)

# Overview

C# shooting game co-produced with bitken1113.

# Requirement

Windows10 / 11

# Usage

To play, download the Game folders and launch shooting.exe.\
Do not rename or delete the text files.\
Reading and writing data will no longer be possible.

# Description
## Control
__[Attack]__\
Left mouse click

__[Move]__\
Player : WASD keys\
(up, down, left and right in relation to the screen)\
Aiming: Mouse cursor\
(The player faces the direction of the sight)

__[WeaponsSwitching]__\
Knife : Q\
Handgun : 1\
Shotgun : 2\
Assault rifle : 3\
Sniper rifles : 4\
(You cannot switch between these if you do not have one in your possession)

__[Reload]__\
Reload : R\
(Attack/weapon switching is not possible while reloading)\
(If you reload with ammunition remaining in the magazine, you will only be loaded short of the maximum number of rounds)

__[Debug]__\
Debug Mode : G

## How to Play

Players are hunters, enemies are zombies.\
The game is about destroying all the zombies.\
Touch the green floor to advance to the next stage.\
If there are surviving zombies on the stage, they are shown in red and cannot be touched.\
Weapons and ammunition can be obtained by touching the icons on the stage.\
Ammunition is not reloaded automatically.Please perform the reload operation.\
Green zombies will attack with their claws if the hunter is within a certain distance.\
Zombies with red edges will fire blood bullets in addition to scratching.\
What is the boss of the final stage?You'll have to wait until you play to find out.\
The player's health is restored in stages in a certain amount of time after being hit.\
If the hunter stands still at this time, the recovery speed increases.\
Knives can slash multiple zombies in a single attack.\
The assault rifle can fire continuously by holding down the attack button.\
Sniper rifle bullets have the property of penetrating zombies.\
Good luck with the game! Aim for a high score!

## Program

__[Game]__
* shooting : Game executable files.
* All_Points : Map coordinates are stored.
* All_Rect : The vertices of the map's obstacles are stored.
* All_Rect_ex : The vertices of the map's obstacles are stored in an expanded form.
* All_Table : The pathfinding map derived from vertex information is stored here.
* Highscore : The fastest times are recorded in milliseconds.Strictly forbidden to tamper with!

__[WayPointDemo]__
* WayPoint : You can learn how pathfinding works.
* All_Points : Identical to the files in the Game folder.
* All_Rect : Identical to the files in the Game folder.
* All_Rect_ex : Identical to the files in the Game folder.
* All_Table : Identical to the files in the Game folder.

__[Source]__
* WayPoint : A project to pre-generate pathfinding maps.
* shooting : The project that actually runs the game.

__[WayPoint]__
* WayPoint.sln : Generate a pathfinding map from the stage's obstacle information \
and write it to the shooting/bin/Debug folder.

__[shooting]__
* shooting.sln : Load files from the shooting/bin/Debug folder and build the stage.\
Manage and move players, zombies, weapons and bullets.

# Author

Porosuke\
bitKen1113

# Licence

This project is licensed under the MIT License, see the LICENSE file for details.
