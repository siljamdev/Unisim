# Unisim
![Icon of the application](res/icon.png)

Application to simulate particles and their interactions

Also, simulates collisions between particles in a continuous way

## Forces

Includes gravity, electrical forces and an invented weak force
You an set up the different constants of particles in the add menu

You can look at how forces work in [this desmos graph](https://www.desmos.com/calculator/8bq31utqb4)

## World Border

You can toggle the world border in the scene settings
It has rectangular shape and the size can be modified
Particles are supposed to stay inside of it, unless when error occur

## Multithreading

Multithreading is set up in most aspects of the simulation that support it. It **should** be deterministic, but that behaviour is not completely proven
You can disable it in the options menu

### How to use

You can open the menu with Escape, and everything is explained in the Help menu
You can additionaly find a changelog in the Info menu
The Control menu, found in the second page of the Options menu, lets you change most keybinds
Screenshots are saved in `[APPDATA]/ashproject/unisim/screenshots`
Saved files are usually saved in `[APPDATA]/ashproject/unisim/saves`, although you can change it in the options

### Additional information

You can find some more information on my [instagram](https://www.instagram.com/siljamdev/)

### A note on scenes

You can probably make an external program that generates scenes. The scene files (*.unisim*) are [AshFiles](https://github.com/siljamdev/AshLib), a structure format from a library I made.
This is the structure of the scene files(separator is .):

```
Root
├── name: (string) scene name
├── worldBorder: (Vec2) x and y size of the world border (if there is a world border, if there is not, then this camp wont exist)
├── numOfParticles: (int) number of particles
├── x  (particles from 0 to numOfParticles - 1)
│   ├── n: (string) particle name
│   ├── c: (Color3) particle color
│   ├── p: (Vec2) particle position
│   ├── r: (double) particle radius
│   ├── m: (double) particle mass
│   ├── e: (double) particle charge value
│   └── w: (double) particle weak charge value
└── x + 1  (next particle and so on)
    └── ...
```