## Dependencies
Included under the `Lib` section of the root directory of the project;
- Nez for MonoGame 3.8
- LiteNetLib 0.9.5.2

## About
A passion project with the goal of emulating Vanilla World of Warcraft for educational purposes.

## Roadmap
Below is a general overall roadmap for the current goals of the project:

- [ ] General 1.12 WoW game content (quests, NPCs, locations, art) within the immediate starting zones listed below
- [ ] Level to 10 with accurate experience modifiers/formulas
- [ ] Grouping/Party system (this includes shared exp, item rolling, shared money)
- Two available classes:
	- [ ] Warrior
	- [ ] Mage
- Two available races:
	- [ ] Human
	- [ ] Undead
- [ ] AI routines and sub-routines for NPCs
- [ ] Buy/sell at vendors
- [ ] Trading
- [ ] Questing within the means of the immediate starting zones
- Pixel artwork for both staring zones:
	- [ ] Human
	- [ ] Undead
- [ ] Chat system with channels, GM chatting, whispering, etc.
- In-game support systems:
	- [ ] GM tickets
	- [ ] Unstuck
- [x] MySQL support
	- EFCore (code-first) MySQL support has been added. Still determining whether to use file managerment for rapid development, though. 
	Having to migrate, update, delete and backtrack database updates is time consuming.
- [ ] Client plugin/addon support
- [ ] Client file management
	- The client will need a way of keeping track of different types of files such as display id's, textures, etc.

In no way, shape or form is this project intended to become a full-fledged product. I work on this in my free-time, which is very limited, and for fun. 
The roadmap above is a best-case scenario final product, but may not make it out of prototyping. Any length I can go with this project will be considered a feat in and of itself.

In the future, a more technical breakdown of each major milestone is planned.

## Disclaimer
This project is undergoing rapid change in its codebase, data structures, object definitions, and general infrastructure and as such, should be considered *wildly* unstable.

Use at your own risk.

## Resources
XP To Level - https://wowwiki-archive.fandom.com/wiki/Formulas:XP_To_Level

Damage/DPS - https://vanilla-wow-archive.fandom.com/wiki/Damage_per_second#Weapon_Damage_Per_Second_(DPS)_Formula

## License
None, just don't be malicious.