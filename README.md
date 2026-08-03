<div align="center">

# Last Try
## An eye-tracking arcade strategy game where you build and command your army using only your gaze.

[![Status](https://img.shields.io/badge/status-prototype-informational)](#project-status)
[![Engine](https://img.shields.io/badge/engine-Unity-black)](#technology)
[![Language](https://img.shields.io/badge/language-C%23-blue)](#technology)
[![Project](https://img.shields.io/badge/project-university%20game-green)](#team)

</div>

---

## About the Game

**Last Try** is an arcade strategy prototype built around Tobii eye-tracking technology.

The player takes the role of a king who must recruit soldiers by looking at them. Before recruitment begins, the enemy army is briefly revealed. The player must memorize its composition and then select suitable units using only their gaze.

Once recruitment is complete, both armies fight automatically. Battle outcomes are determined by unit matchup advantages, disadvantages, and a D20 dice roll that introduces uncertainty and replayability.

The central design question behind the project was:

> Can gaze alone serve as a meaningful and enjoyable game input?

The game was developed as part of the Advanced User Interfaces course at Hochschule Bonn-Rhein-Sieg.

---

## Gameplay Trailer

<div align="center">

<a href="https://youtu.be/Md8u-3zo31Y?si=y4kyKYZ12CvHMw2M">
  <strong>▶ Watch the Gameplay Trailer</strong>
</a>

</div>

The main gameplay loop is:

1. Observe the enemy army during a short preview
2. Memorize its unit composition and identify possible counters
3. Recruit soldiers that counter the enemy units by looking at them
4. Confirm each selection using the keyboard
5. Watch both armies fight
6. Review the battle result and final score
7. Play again and try a different army composition

---

## Key Features

<table> <tr> <td width="50%" valign="top">

### Eye-Tracking Recruitment

Soldiers are selected by looking directly at them. The recruitment interface uses gaze position from the Tobii Eye Tracker as its primary input.

</td> <td width="50%" valign="top">

### Enemy Army Preview

Before recruitment begins, the enemy army is shown for a limited time. The player must memorize its composition and prepare an effective counter-army.

</td> </tr> <tr> <td width="50%" valign="top">

### Keyboard Confirmation

Gaze is used to choose a unit, while keyboard input confirms the selection. This reduces accidental activations and makes the interaction more reliable.

</td> <td width="50%" valign="top">

### Timed Recruitment

The player has a limited amount of time to recruit soldiers. This creates pressure and prevents the player from analysing every possible matchup indefinitely.

</td> </tr> <tr> <td width="50%" valign="top">

### Automatic Battles

After recruitment, both armies fight automatically. The result depends on the selected units and their matchup relationships.

</td> <td width="50%" valign="top">

### D20 Combat Variance

A twenty-sided dice roll adds uncertainty to combat. Even a strategically strong army is not guaranteed to win every battle.

</td> </tr> </table>

---

## Screenshots

<div align="center">

<img src="docs/readme/enemy-army-showacse.png" alt="Last Try enemy army showcase" width="49%">
<img src="docs/readme/recruit.png" alt="Last Try team recruitment" width="49%">

<img src="docs/readme/battle.png" alt="Last Try armies battle" width="49%">
<img src="docs/readme/result-panel.png" alt="Last Try result panel" width="49%">

</div>

---
## Technology

- **Engine:** Unity
- **Language:** C#
- **Audio:** Wwise
- **3d Modeling:** Blender
- **Textures creation:** Substance painter
- **2d Art:** Procreate
- **Data architecture:** ScriptableObjects for items, ingredients, recipes, adventurers and so on
- **Dialogue:** JSON-based dialogue system
- **System communication:** C# events
- **Reusable content:** Unity Prefabs for all items, characters, stations, and UI elements
- **Saving:** Custom save and load system

---

## Notable Systems

- Adventurer spawning and interaction
- Quest and reward system
- JSON-based dialogue
- Dice-based haggling
- Cooking and recipe system
- Nine-slot hotbar
- Item pickup, carrying, and placement
- Upgrade stations
- Furniture dismantling
- Dark Entity interactions
- Game recipe progression
- Save and load system
- Main menu, settings, and gameplay UI
- Notifications, cutscenes, and interaction feedback

---

## Team

| Team Member | Role | Main Responsibilities |
|---|---|---|
| **Taha Batur Şenli** | Game Designer & Project Manager | Game concept, gameplay design and narrative content |
| **Roman Shostak** | Unity Developer | Dialogue and quest systems, haggling, item upgrading and dismantling, and the Dark Entity system |
| **Iryna Huryn** | Unity Developer | Cooking system фтв progression, hotbar and item interactions, menu implementation, and save/load system |
| **Nazree Nadhir** | 3D Artist | Furniture, rewards, ingredients and finished dishes |
| **Lisa Grebe** | 2D & 3D Artist | House model, UI, icons, and other 2D assets |
| **Jonathan Glück** | Sound Designer & Audio Engineer | Music, sound effects, audio implementation, and mixing |

---

## Links

- **Playable build:** Coming later
- **Gameplay video:** [Link](https://youtu.be/Md8u-3zo31Y?si=y4kyKYZ12CvHMw2M)

---

## Credits

Created as a university team project by students from different specializations, including game design, Unity development, hardware development, and art.

Special thanks to everyone who contributed to the prototype, testing, presentation, and final delivery.

---

## Third-Party Assets

Dragon Soup uses some of third-party assets. These materials remain the property of their respective authors and are used according to their original licenses.

| Asset | Author / Source | License | Usage |
|---|---|---|---|
| **[Berry Rotunda](https://www.dafont.com/berry-rotunda.font)** | Typo-Graf / DaFont | Public Domain | Used for menus, dialogue panels, notifications, and other UI text |
| **[Tudor Wall 03](https://freestylized.com/material/tudor-wall-03/)** | FreeStylized | FreeStylized Custom CC0 / Royalty-Free License | Used for the tavern wall material |
| **[Wood Planks 05](https://freestylized.com/material/wood_planks_05/)** | FreeStylized | FreeStylized Custom CC0 / Royalty-Free License | Used for the tavern floor material |

Unless otherwise noted, the original code, artwork, game design materials, and hardware-related content were created by the Dragon Soup development team.

---

## License

Copyright © 2026 Dragon Soup Team. All rights reserved.

This project is publicly available for portfolio viewing and educational evaluation only. The source code, original assets, hardware materials, and other project contents may not be copied, modified, redistributed, or used in other projects without prior written permission from the respective copyright holders.

Third-party assets are excluded from this license and remain subject to their respective licenses and terms of use.

---
# 👁️ Last Try

> An eye-tracking arcade strategy game where you build and command your army using only your gaze.

## Overview

Last Try is a prototype game built around Tobii eye-tracking technology, developed as part of the Advanced User Interfaces course at Hochschule Bonn-Rhein-Sieg. You play as a king who must recruit soldiers by looking at them, then watch your army fight automatically — with outcomes decided by matchup logic and a D20 dice roll.

The core design challenge: can gaze alone serve as a meaningful game input?

## Gameplay Loop

1. **Enemy reveal** — the enemy army is briefly shown; memorize it
2. **Recruitment phase** — look at soldiers to select them; confirm with keyboard
3. **Battle phase** — armies fight automatically based on unit matchups
4. **Results** — score calculated; play again

## Features

- Eye-tracking unit selection via Tobii Eye Tracking 5.0
- Keyboard confirmation to reduce false activations
- Timed recruitment phase
- Automatic battle simulation with matchup advantages/disadvantages
- D20 dice roll mechanic for combat variance and replayability
- Main menu and results screen
- Pixel art visual style (32×32 sprites, 16-color palette)

## Tech Stack

| Tool | Purpose |
|------|---------|
| Unity | Game engine and logic |
| Tobii Eye Tracking 5.0 | Primary input device |
| C# | Game logic |
| Aseprite | Pixel art asset creation |
| Wwise | Audio integration |
| Ableton Live | Music production |

**Platform:** Windows 10+

## My Role

*Artist — Visual Design & Asset Integration*

- Designed and implemented the complete visual style of the game
- Created all 2D pixel-art assets (32×32 sprites, 16-color palette) in Aseprite
- Built and integrated UI visuals and in-game object art
- Contributed to the visual presentation of game mechanics and flow

## Project Status

University prototype project (Hochschule Bonn-Rhein-Sieg, Advanced User Interfaces course). Full gameplay loop implemented: Main Menu → Enemy Preview → Recruitment → Battle → Results.

> ⚠️ Source code is managed via Unity Version Control (Plastic SCM) and is not hosted on GitHub.
