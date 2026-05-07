# Game Design Document (GDD)

## 1. Introduction

### 1.1. Scope of the Document
This document is intended for the development team, stakeholders, and anyone involved in the production of Hexal Miners. It serves as the comprehensive reference for all design decisions, mechanics, systems, and narrative elements implemented in the Unity prototype.

### 1.2. Elevator Pitch
Hexal Miners is a competitive boardgame-style experience where six miners, trapped underground after a catastrophic cave-in, must navigate a hexagonal tile map using dice rolls and card draws. Only one miner can escape through a hidden exit tile buried somewhere in the unstable cave. Players must strategically break tiles with pickaxe cards while managing limited resources all wrapped in a theme of existential dread where every step could be your last.

## 2. Game Overview

### 2.1. Game Concept
Hexal Miners takes place entirely on a grid of hexagonal tiles representing the debris from a devastating mining accident. Up to three players take turns controlling one of three miner types—Blaster (Bomb), Hewer (Pickaxe), or Lamproom Man (Lamp)—rolling a six-sided dice to move across the map and drawing cards from a deck to gain temporary abilities. The primary objective is to locate and destroy a hidden black win-state tile using accumulated pickaxe power. Once that tile is broken, the current player escapes to the surface and wins the game. The core feeling is one of tension and dread—players must balance risk and reward while knowing only one miner can ultimately survive.

### 2.2. Audience
Hexal Miners targets an "everyone" demographic, from young children to elderly adults. The game's simplified premise roll dice, move, draw cards, break tiles—makes it accessible to casual gamers while the strategic depth of resource management appeals to more competitive players. The game is designed with inclusivity in mind: characters feature diverse skin tones, events are communicated visually as well as textually, and localization support is planned for multiple languages

### 2.3. Genre
Boardgame / Turn-Based Strategy / Party Game

### 2.4. Setting
The game takes place in the mid-20th century Nevada Black Rock Desert at an active underground mining site. Technology is limited to traditional labor intensive mining tools pickaxes, dynamite, and basic lamps. The cave environment is dusky, unstable, and filled with debris from a catastrophic explosion that caused the cave-in trapping all miners underground.

### 2.5. World Structure
Players navigate a single 8x8 hexagonal tile grid representing the collapsed mine floor. Movement is non-linear—players can move freely within their dice roll range to any available tile. Destroyed tiles become impassable, gradually shrinking the playable area.

### 2.6. Player
Blaster (Bomb token)

Hewer (Pickaxe token)

Lamproom Man (Lamp token) 

### 2.7. Core Loop
The core gameplay loop per turn is: Roll Dice → Move Token → Draw Card → Choose Action (Use/Store/Do Nothing) → End Turn. Players repeat this loop, accumulating card effects and breaking tiles until one player destroys the hidden win-state tile.

### 2.8. Look & Feel
The visual style is a pixelated 2D boardgame aesthetic with PNG sprite-based player tokens overlaid on a hexagonal tile grid. Tiles darken as they take damage, destroyed tiles turn red and become unavailable, and the win-state tile can be revealed as pure black. The interface includes a dice UI, card deck display, action buttons (Use/Store/Do Nothing), an inventory panel, and dialogue text feedback for all game events.
## 3. Gameplay

### 3.1. Objectives
Primary Objective: Locate and destroy the hidden black win state tile to escape the cave and win the game.
Secondary Objectives: Accumulate stat bonuses (Hit Power and Defense) through card draws, store cards in inventory for strategic use, and block opponents by destroying tiles in their path.
### 3.2. Progression
Progression is measured through accumulated stat bonuses:
Hit Power increases from Pickaxe and TNT cards, allowing tiles to be broken faster. Defense increases from Helmet cards, providing protection against stuns. Inventory stores unused cards for later tactical deployment
### 3.3. Play Flow
gameplay session flows as follows:
Player rolls the dice (1-6). Green highlights appear on all valid tiles within the roll distance. Player clicks a highlighted tile to move their token there. Player clicks the DRAW button on the card deck. A random card is drawn and displayed. The Action Panel appears with three choices:, Use (B key): Apply card's stat bonus and attack the current tile, Store (N key): Save the card to inventory for later use, Do Nothing (M key): End turn without using the card. Turn passes to the next player, Players can press K to open their inventory and select a stored card to use on their turn, Press R to reveal/hide the win-state tile location (cheat peek),Game ends when a player destroys the win-state tile
## 4. Mechanics

### 4.1. Rules
-Each player gets one turn per round in fixed order (Player 1 → Player 2 → Player 3 → repeat)

-Players can only move to tiles within their dice roll value (1-6 steps)

-Destroyed tiles (defense reduced to 0) are impassable

-Players may occupy the same tile as another player

-Only one card can be drawn per turn

-Win-state tile is randomly selected at game start and hidden from players

### 4.2. Game Universe
-Tile System: Every tile has a type (Bronze/Silver/Gold) that determines its defense value

-Card Deck: Contains 10 card types with weighted draw chances

-Win Tile: A randomly selected tile marked as the escape route; breaking it ends the game

-Stun System: If a player's defense reaches 0, they are stunned for one turn and recover with 1 defense
### 4.3. Character Movement
Players use a virtual D6 dice roll to determine movement range. Valid tiles are highlighted in green. Players can move freely within the roll distance—players can move to any valid, non-destroyed tile on the map. Destroyed tiles are unavailable and be landed on.

### 4.4. Player Interaction
-Tiles: Click to move, attack with pickaxe cards to break them

-Cards: Draw, use immediately, or store in inventory

-Other Players: Share tiles

-Inventory: Press K to open/close, select stored cards to use

#### 4.5. Game Menus
A brief mention on how the game menus work and what options are available to the player.


#### 4.6. Game Options
-Dice UI: Click the dice button to roll
-Card Deck UI: Shows the drawn card image, DRAW button
-Action Panel: Appears after drawing with Use/Store/Do Nothing buttons
-Inventory Panel: Press K to toggle, shows all stored cards as selectable images
-Win Panel: Appears when a player wins, displays "{PlayerName} WINS!!!"

### 4.7. Assets
A list of the main assets that the game will use, split by type: *Player Model, Player Texture, Enemy Model, Terrain Material, Enemy Death Sound, etc.*

-Player Tokens:Bomb (Blaster), Pickaxe (Hewer), Lamp (Lamproom Man)as PNG sprites
Tiles:Bronze, Silver, Gold hexagonal tiles with defense stats
Card Sprites:10 card images (Wooden/Iron/Golden Pickaxe, Regular/Superbomb/Purple/Colorful TNT, Wooden/Iron Helmet, Lamp)
Dice: 6-sided dice with roll animation
UI Panels: Card Deck, Action Panel, Inventory Panel, Win Panel

## 5. Graphics and Audio

### 5.1. Visual System
The game uses a 2D pixelated hexagonal tile board with PNG-based player token sprites. The camera is orthographic and auto-adjusts to fit the entire map. Tiles visually degrade (darken) as they take damage, destroyed tiles turn red, and the win tile appears black when revealed.
#### 5.1.1. Player Camera
A single orthographic camera centered on the map.

#### 5.1.2. Landscape
The game world consists entirely of a 8x8 hexagonal tile grid. There are no additional landscape elements the board itself represents the debris-filled mine floor.
### 5.2. Interface

-Dice UI: Positioned for easy clicking

-Card Deck: Shows card back, drawn card, and DRAW button

-Action Panel: Appears centered after drawing a card with three buttons

-Inventory Panel: Overlays with a grid of stored card images

-Dialogue Text: Provides feedback for all game events

-Turn Display: Shows current player's name

## 6. Story and Narrative

### 6.1. Backstory
On an evening in the Nevada Black Rock Desert, a mining site was in high-powered operation with various miner types Blasters, Hewers, and Lamproom Men working in tandem to complete their daily quota. In the midst of work frenzy, a Blaster unit primed and detonated explosives without proper authorization. The blast caused the ground to collapse, plunging all miners underground into a dusky, debris filled cave with no visible exit.

### 6.2. Main Plot
The surviving miners—Dyne and Powwow (Blasters), Hatchet and Brine (Hewers), and Ernie and Sal (Lamproom Men) must use scattered, damaged tools to break through the collapsed debris and find a way back to the surface. However, the unstable cave structure means that only one miner may ultimately escape. Every step is a gamble, and every broken tile brings one miner closer to freedom—or seals another's fate forever.
## 7. Characters

### 7.1. Main Characters
-7.1.1. Blasters (Dyne & Powwow)
    Backstory: Experienced explosives handlers who caused the cave-in through reckless detonation.

    Appearance: Bomb token icon.

    Abilities: Specialize in high-damage TNT cards; start with higher hit power potential.

7.1.2. Hewers (Hatchet & Brine)
    Backstory: Skilled diggers responsible for extracting minerals with pickaxes and shovels.

    Appearance: Pickaxe token icon.

    Abilities: Specialize in Pickaxe cards; efficient at breaking tiles.

7.1.3. Lamproom Men (Ernie & Sal)
    Backstory: Lamp maintenance workers who distributed light sources throughout the mine.

    Appearance: Lamp token icon.

    Abilities: Specialize in Lamp cards; can illuminate hidden paths and reveal clues.

## 8. Game World

### 8.1. Look & Feel of the World
The mine interior is represented by a pixelated hexagonal grid with three tile types: Bronze (brown, 1 defense), Silver (gray, 2 defense), and Gold (yellow, 3 defense). Damaged tiles darken, destroyed tiles turn red and become impassable. The hidden win tile appears black when revealed. The overall aesthetic is dark, claustrophobic, and tense.

### 8.2. Locations
The entire game takes place in a single location: the collapsed mine chamber represented by the 8x8 hexagonal tile grid.

#### 8.2.1. Connection to the Plot
The tile grid represents the debris that must be cleared to find the escape route. The win-state tile is the only path to the surface hidden among the wreckage.

### 8.3. Levels
The current prototype features a single 8x8 hexagonal grid level. The grid size can be adjusted in the Inspector (gridWidth/gridHeight). There are no multiple levels or stages.

## 9. Changelog

Looking back at where Hexal Miners started and where it ended up, a lot changed between the Design Treatment and the final Unity build. The name was locked in, and the six individual miners I originally planned got consolidated into just three tokens Bomb, Pickaxe, and Lamp, each still carrying the spirit of the Blaster, Hewer, and Lamproom Man roles. Movement opened up from diagonal only to letting players go anywhere within their dice roll, which just felt better during testing. The card system got trimmed down too; Pickaxes are the only ones that really do what they're supposed to, while TNT and Helmets give stat bonuses but don't have their special mechanics working yet, and Lamp cards just sit there looking pretty. I had to cut the event cards and creatures entirely—there just wasn't time. On the flip side, I built some things I hadn't even thought about in the treatment: the Use/Store/Do Nothing buttons that actually work now, an inventory you can pull up anytime, and a defense system where getting knocked out stuns you for a turn. The whole thing runs on a pixelated hex grid with PNG tokens, the camera adjusts itself to fit whatever size map I make, and it supports up to three people with both mouse and keyboard controls. It's not everything I dreamed up originally and the game itself is playable after all.

