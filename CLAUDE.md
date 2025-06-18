# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6000.1.7f1 project implementing "Crapette Party" - a multiplayer card game for educational purposes demonstrating Photon networking. The game supports both local (same-screen) and online multiplayer modes.

## Architecture

### Core Components
- **MainDeck**: Singleton managing the 52-card deck with shuffling/dealing logic
- **Card**: Individual card representation with CardData struct (sprite, value) and card distance calculations
- **CardStack**: Stack-based card container with visual management and card serving functionality
- **CardManager**: Main game logic controller handling turn phases, win conditions, and game flow
- **PlayerLocal**: Local player input handling (mouse for P1, keyboard for P2 when local)

### Networking Layer
- **NetGameManager**: Photon PUN2 integration managing room state, player synchronization, and timing
- **NetCardLobby**: Lobby scene controller handling room creation/joining and player matching
- **NetPlayer**: Network player representation and state management

### Game Flow
1. **Preparation**: Cards dealt to player boards (5 stacks of 1-5 cards each)
2. **Service**: Both players draw initial cards to center stacks
3. **Play Phase**: Players place cards on center stacks (±1 value rule)
4. **New Service**: When no moves available, new cards drawn
5. **Victory**: First player to empty their board wins

### Key Systems
- **Deck Management**: Uses Unity's Random.InitState() for synchronized shuffling across network
- **Turn Management**: Coroutine-based phase system with network synchronization
- **Card Movement**: DOTween integration for smooth card animations
- **Input Handling**: Dual input support (mouse/keyboard) for local co-op

## Dependencies

### Required Asset Store Packages (Not Included)
- **Photon PUN2**: Network multiplayer framework
- **DOTween**: Animation library for card movement

## Common Development Commands

### Unity Editor
- Open Unity Hub and add this project folder
- Ensure Unity 6000.1.7f1 is installed
- Import required dependencies from Asset Store before first run

### Scenes
- **CardGame-Lobby**: Main menu and multiplayer lobby
- **CardGame-Board**: Game board where matches are played

### Key Scripts Locations
- Core game logic: `Assets/Scripts/CardGame/`
- Networking components: `Assets/Scripts/CardGame/Network/`
- Card assets: `Assets/2D/Cards/`

## Network Architecture

The game uses Photon PUN2 with:
- **Room Properties**: Deck randomization seed, game timing
- **Player Properties**: Ready states, level loading status
- **Master Client**: Handles game state transitions and scene loading
- **Automatic Scene Sync**: Enabled for seamless multiplayer transitions

## Known Issues
- Game may fail on restart attempts
- No conflict resolution for simultaneous card plays
- Limited to 2 players maximum per room