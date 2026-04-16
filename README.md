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
