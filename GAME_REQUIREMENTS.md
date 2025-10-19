# Game Requirements Document

## Core Features Required

### 1. Main Character
- ✅ Basic character with WASD movement (PlayerMovement.cs exists)
- ✅ Camera controls (implemented in PlayerMovement.cs)
- ❌ Health system (needs implementation)

### 2. Health System
- ❌ Player health component
- ❌ Health UI display
- ❌ Health reduction on damage
- ❌ Death/respawn mechanics

### 3. NPC System
- ❌ NPC prefab with movement (WASD-like)
- ❌ Multiple NPC spawning at different locations
- ❌ Spawn timing system (spawn within seconds)
- ❌ AI behavior to move toward and attack main character

### 4. Combat System
- ❌ NPC attack mechanics
- ❌ Weapon assignment to NPCs
- ❌ Attack animations/movements
- ❌ Damage dealing to player

### 5. Spawn System
- ❌ Fixed spawn points
- ❌ Timed spawning system
- ❌ Spawn management (limit, cleanup)

### 6. Game Orchestration
- ❌ Game start sequence
- ❌ Wave management
- ❌ Overall game flow control

## Technical Implementation Plan

### Phase 1: Health System
1. Create PlayerHealth script
2. Create Health UI
3. Integrate with existing PlayerMovement

### Phase 2: NPC System
1. Create NPC prefab
2. Create NPC movement script
3. Create NPC AI script for targeting player

### Phase 3: Spawn System
1. Create spawn points
2. Create spawn manager
3. Integrate with NPC system

### Phase 4: Combat System
1. Create attack scripts
2. Implement damage system
3. Add weapon mechanics

### Phase 5: Game Management
1. Create game manager
2. Implement wave system
3. Add UI for game state

## Current Status
- ✅ Player movement and camera (PlayerMovement.cs)
- ✅ Basic project structure
- ❌ Everything else needs implementation

## Notes
- Focus on core functionality first
- Make systems modular for easy expansion
- Ensure proper Unity component architecture
- Test each system individually before integration
