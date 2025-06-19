# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6000.1.7f1 project implementing "Chemistry Party" - a multiplayer educational card game demonstrating chemistry concepts through element cards and compound creation. The project evolved from a traditional Crapette card game into a chemistry-themed game with Photon networking support.

## Architecture

### Game Manager Hierarchy
The project uses a dual-manager system:
- **ChemistryGameManager**: Single-player focused chemistry game logic
- **SimpleChemistryManager**: Multiplayer-enabled version with Photon PUN2 integration
- **ChemistryCardManager**: Legacy card game manager (original Crapette implementation)

### Core Chemistry Components
- **ChemistryDatabase**: ScriptableObject containing all elements, compounds, and reactions
- **ElementData**: Individual chemical element with atomic properties and visual data
- **ElementCard**: UI component for interactive element cards with selection states
- **CompoundData**: Chemical compound definitions with required elements and properties
- **CompoundCard**: UI component for displaying created compounds
- **ReactionData**: Defines chemical reactions between compounds

### UI System Architecture
The project features a modern UI system with fallback support:
- **ModernUITheme**: Centralized theming system with color palettes and animations
- **ModernUIComponents**: Enhanced UI components (ModernButton, ModernPanel, ModernText)
- **EnhancedLobbyUI/EnhancedGameBoardUI**: DOTween-powered modern interfaces
- **BuildSafeUI/SimpleCrashFix**: Fallback systems for builds without dependencies

### Networking Layer
- **NetCardLobby**: Photon lobby management with room creation/joining
- **NetGameManager**: Network synchronization for multiplayer chemistry games
- **SimpleChemistryManager**: Implements IOnEventCallback for custom Photon events

## Game Flow

### Chemistry Game Loop
1. **Initialization**: Players receive random element cards (periodic table elements)
2. **Selection Phase**: Players select compatible elements from their hand
3. **Compound Creation**: Valid combinations create compounds (H2O, CO2, etc.)
4. **Discard Option**: Players can discard unwanted cards and draw replacements
5. **Victory Condition**: First player to create 8 unique compounds wins

### Multiplayer Synchronization
- Turn-based gameplay with network event broadcasting
- Hand state synchronization across clients
- Compound creation validation on master client
- Automatic turn switching with timeout protection

## Dependencies

### Required Asset Store Packages (Not Included)
- **Photon PUN2**: Network multiplayer framework for lobby and game synchronization
- **DOTween**: Animation library for modern UI transitions and card movements

### Dependency Management
The project includes automatic dependency detection and graceful fallbacks:
- Missing DOTween: Enhanced UI components automatically disable
- Missing Photon: Network features disable, single-player mode remains functional
- Missing script references: Crash prevention systems detect and handle gracefully

## Build System and Diagnostics

### Build Tools (Assets/Scripts/CardGame/)
- **QuickBuildFix**: One-click build preparation and scene configuration
- **BuildFixer**: Comprehensive build diagnostics and automated fixes
- **BuildDiagnostics**: Scene validation and dependency checking
- **SimpleCrashFix**: Runtime crash prevention and component validation

### Common Build Issues
- Missing script references cause immediate crashes - use BuildSafeUI to detect
- DestroyImmediate calls in builds - all scripts properly wrapped with #if UNITY_EDITOR
- Scene configuration - build tools automatically add essential scenes

## Common Development Commands

### Build and Testing
```bash
# Use QuickBuildFix component context menu
Right-click QuickBuildFix → "🚀 Quick Build Fix & Test"
Right-click QuickBuildFix → "🎯 Create Safe Build Now"

# Alternative: Use Unity Build Settings
File → Build Settings → Add scenes manually → Build and Run
```

### Chemistry Database Setup
```bash
# Create chemistry database
Assets → Create → Chemistry → Chemistry Database
# Populate with elements via ChemistryDataInitializer
Right-click ChemistryDataInitializer → "Setup All Chemistry Data"
```

### Scene Management
- **CardGame-Lobby**: Chemistry-themed lobby with enhanced UI (DOTween dependent)
- **CardGame-Board**: Main chemistry game board with compound creation interface
- **DemoAsteroids-LobbyScene**: Photon demo scene (not used in actual game)

## Network Architecture

### Photon Integration
- **Custom Events**: Element selection, compound creation, turn switching
- **Room Properties**: Game state, turn information, chemistry database references  
- **Player Properties**: Hand contents, compound collections, ready states
- **Master Client Authority**: Validates compound creation and manages game state

### Event Synchronization
```csharp
// Custom event codes used in SimpleChemistryManager
const byte ELEMENT_SELECTED = 1;
const byte COMPOUND_CREATED = 2;
const byte TURN_ENDED = 3;
const byte HAND_UPDATED = 4;
```

## Key Script Locations
- Core chemistry logic: `Assets/Scripts/CardGame/ChemistryGameManager.cs`
- Multiplayer logic: `Assets/Scripts/CardGame/SimpleChemistryManager.cs`
- Database management: `Assets/Scripts/CardGame/ChemistryDatabase.cs`
- UI systems: `Assets/Scripts/CardGame/ModernUI*.cs`
- Build tools: `Assets/Scripts/CardGame/QuickBuildFix.cs`
- Network components: `Assets/Scripts/CardGame/Network/`

## Known Issues and Limitations
- DOTween dependency causes enhanced UI to fail in builds without proper asset store import
- Photon PUN2 required for multiplayer - game falls back to single-player without it
- Missing script references in scenes cause immediate runtime crashes
- Hand parent assignment critical for card display - checked in SimpleChemistryManager:219
- Win condition hardcoded to 8 compounds (configurable in inspector)
- No save/load functionality for game progress