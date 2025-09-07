# BoonOfDoom

A Unity-based action RPG game featuring deep combat mechanics, weapon progression systems, and puzzle-solving elements.

## 🎮 Game Overview

**BoonOfDoom** is a third-person action RPG built in Unity 6.0, featuring Souls-like combat mechanics with unique weapon progression and puzzle-solving elements. Players explore a dark fantasy world, battle enemies, solve environmental puzzles, and unlock powerful weapon abilities.

## 🏗️ Project Architecture

### Core Systems

#### **Character Management**
- **Player System**: Comprehensive player controller with locomotion, combat, inventory, and equipment management
- **AI System**: Advanced AI with patrol paths, boss fights, and character spawning
- **Character Stats**: Health, stamina, runes (currency), and focus management

#### **Combat System**
- **Weapon Classes**: Straight Sword, Great Axe, Bow
- **Attack Types**: Light attacks, heavy attacks, charged attacks, running attacks, rolling attacks
- **Combat Mechanics**: Poise system, blocking, dodging, weapon abilities
- **Projectile System**: Arrows and magical projectiles

#### **Weapon Progression**
- **Skill Trees**: Unique progression trees for each weapon type
- **Weapon Levels**: Experience-based weapon leveling system
- **Special Abilities**: Unlockable weapon abilities (Flame, Smash)
- **Elemental Effects**: Fire, Lightning, and Heavy Impact damage types

#### **Puzzle System**
- **Environmental Puzzles**: Timed torches, pressure plates, breakable objects
- **Weapon-Based Puzzles**: Puzzles requiring specific weapon abilities
- **Sequence Puzzles**: Multi-step puzzle sequences

#### **Save System**
- **Multiple Save Slots**: 3 character save slots
- **Persistent Data**: Character stats, equipment, world state, boss progress
- **Death System**: Runes drop at death location for retrieval

## 🎯 Key Features

### **Combat Mechanics**
- **Souls-like Combat**: Precision-based combat with stamina management
- **Weapon Variety**: Multiple weapon types with unique movesets
- **Lock-On System**: Target locking for precise combat
- **Combo System**: Chain attacks with timing-based combos

### **Weapon Progression**
- **Skill Trees**: Visual skill trees for each weapon type
- **Rune System**: Currency-based skill unlocking
- **Weapon Abilities**: Special abilities like Flame Slash, Charged Slam
- **Elemental Imbuement**: Infuse weapons with fire, lightning, or heavy impact

### **Puzzle Elements**
- **Timed Torches**: Light torches in specific sequences
- **Pressure Plates**: Environmental triggers
- **Breakable Objects**: Destructible environment elements
- **Weapon-Specific Puzzles**: Puzzles requiring specific weapon abilities

### **World Management**
- **Boss Fights**: Unique boss encounters with special mechanics
- **Character Spawning**: Dynamic AI character management
- **Patrol Paths**: AI navigation systems
- **Brazier System**: Checkpoint/rest system

## 🛠️ Technical Implementation

### **Unity Version**
- **Engine**: Unity 6.0.43f1
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Input System**: New Input System for cross-platform support

### **Key Packages**
- **Input System**: `com.unity.inputsystem` (1.13.1)
- **AI Navigation**: `com.unity.ai.navigation` (2.0.7)
- **Netcode**: `com.unity.netcode.gameobjects` (2.2.0)
- **Visual Effects**: `com.unity.visualeffectgraph` (17.0.4)
- **Timeline**: `com.unity.timeline` (1.8.8)

### **Core Scripts Architecture**

#### **Player System**
```
Assets/Scripts/Character/Player/
├── PlayerManager.cs              # Main player controller
├── PlayerInputManager.cs         # Input handling
├── PlayerCombatManager.cs        # Combat logic
├── PlayerLocomotionManager.cs    # Movement system
├── PlayerEquipmentManager.cs     # Equipment management
├── PlayerInventoryManager.cs     # Inventory system
├── PlayerStatManager.cs          # Stats and resources
├── PlayerCamera.cs               # Camera control
└── PlayerUI/                     # UI management
```

#### **World Management**
```
Assets/Scripts/WorldManager/
├── WorldAIManager.cs             # AI character management
├── WorldSaveGameManager.cs       # Save/load system
├── WorldItemDatabase.cs          # Item data management
├── WorldSoundFXManager.cs        # Audio management
└── WorldUtilityManager.cs        # Utility functions
```

#### **Weapon System**
```
Assets/Scripts/Items/Weapons/
├── WeaponItem.cs                 # Base weapon class
├── WeaponManager.cs              # Weapon management
├── MeleeWeaponItem.cs            # Melee weapon class
├── RangedWeaponItem.cs           # Ranged weapon class
└── WeaponSkillTree.cs            # Skill tree system
```

#### **Puzzle System**
```
Assets/Scripts/PuzzleSystem/
├── Manager/
│   ├── WeaponSkillManager.cs     # Skill management
│   └── WeaponSkillUnlockManager.cs # Skill unlocking
└── EnvObjects/
    ├── TimedTorch.cs             # Torch puzzle logic
    ├── PressurePlateObject.cs    # Pressure plate system
    ├── BreakableObject.cs        # Destructible objects
    └── FlammableObject.cs        # Fire-reactive objects
```

## 🎨 Asset Structure

### **Art Assets**
```
Assets/Art/
├── Models/                       # 3D models
├── Textures/                     # Material textures
├── Materials/                    # Material definitions
├── Animations/                   # Animation clips
├── Audio/                        # Sound effects and music
├── Fonts/                        # UI fonts
└── UI_Sprites/                   # UI elements
```

### **Data Assets**
```
Assets/Data/
├── Items/
│   ├── Weapons/                  # Weapon definitions
│   ├── Flasks/                   # Consumable items
│   └── Armor/                    # Equipment items
├── Weapon Skills/                # Skill tree definitions
├── Weapon Actions/               # Combat action definitions
├── Effects/                      # Visual effects
└── AIState/                      # AI behavior definitions
```

## 🎮 Game Mechanics

### **Combat System**
- **Stamina Management**: All actions consume stamina
- **Blocking**: Weapons can block incoming damage
- **Dodging**: Evasive maneuvers with invincibility frames

### **Weapon Progression**
- **Experience System**: Weapons gain experience through combat
- **Skill Trees**: Visual progression trees with prerequisites
- **Rune Cost**: Skills require souls (currency) to unlock
- **Special Abilities**: Unique weapon abilities for puzzle solving

### **Puzzle Mechanics**
- **Elemental Puzzles**: Use fire abilities to light torches
- **Timing Puzzles**: Complete sequences within time limits
- **Weapon-Specific**: Puzzles requiring specific weapon abilities
- **Environmental**: Interact with world objects to progress

### **Save System**
- **Multiple Slots**: 3 independent character save slots
- **Persistent World**: Boss states, item collection, puzzle progress
- **Death Recovery**: Retrieve lost runes from death location
- **Checkpoint System**: Brazier rest points for progression

## 🚀 Getting Started

### **Prerequisites**
- Unity 6.0.43f1 or later
- Visual Studio 2022 or Rider (recommended)

### **Installation**
1. Clone the repository **(IMPORTANT: make sure you have Git LFS installed or assets and files will not import correctly!)**
2. Open the project in Unity 6.0
3. Wait for package installation to complete
4. Open `Assets/Scenes/Scene_Main_Menu.unity` to start

### **Controls**
- **Movement**: WASD or Left Stick
- **Camera**: Mouse or Right Stick
- **Attack**: Left Click or RB
- **Heavy Attack**: Right Click or RT
- **Block**: Q or LB
- **Dodge**: Space or B
- **Interact**: E or Y
- **Lock-On**: Tab or Right Stick Click

## 📁 Project Structure

```
BoonOfDoom/
├── Assets/
│   ├── Scripts/                  # All C# scripts
│   ├── Scenes/                   # Unity scenes
│   ├── Art/                      # Visual assets
│   ├── Data/                     # Game data assets
│   ├── SimpleNaturePack/         # Environment assets
│   ├── Frank_RPG_2Handed/        # Character animations
│   └── TextMesh Pro/             # Text rendering
├── ProjectSettings/               # Unity project settings
├── Packages/                      # Unity packages
└── README.md                      # This file
```

## 🎯 Development Status

This project is actively developed with the following systems implemented:

✅ **Core Systems**
- Player movement and combat
- Weapon system with progression
- Save/load functionality
- AI character management
- Puzzle system framework

## 🤝 Contributing

This is a personal project, but suggestions and feedback are welcome. The codebase is well-structured and documented for easy understanding and modification.

## 📄 License

This project is for educational and personal use. All assets and code are property of the original creators.

---

**BoonOfDoom** - A dark fantasy action RPG with deep combat mechanics and puzzle-solving elements. 
