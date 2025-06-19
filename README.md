# 🧠 MonoBehaviour FSM for Unity

This codebase provides a **modular and extensible Finite State Machine (FSM)** framework for character control in Unity, using **MonoBehaviour-based states**. It's designed for clean architecture, easy debugging, and flexible control for both player and AI-driven units.

## 🔧 Core Architecture

The system is centered around a `UnitStateManager` that handles:

- State transitions
- Input or AI behavior
- Shared context data

Each state (e.g., `IdleState`, `RunState`) is a separate `MonoBehaviour` inheriting from an abstract base `UnitStateBase`, and encapsulates its own logic and movement behavior.

## ✨ Key Features

- **State as Component**  
  Each state is a MonoBehaviour, allowing use of Unity’s serialization, events, and inspector tools.

- **Separation of Concerns**  
  Clean separation of logic for state, movement, animation, and collision.

- **Extensibility**  
  Add new states by implementing `UnitStateBase`.

- **Inspector-Friendly**  
  Manage transitions, forbidden states, and movement settings in the Unity Inspector.

- **Animation Integration**  
  Supports animation events and queued parameter changes.

- **Context Awareness**  
  States respond to the environment (grounded, platform presence, surface material) and input.

- **Modular & Maintainable**  
  Self-contained states enable easy updates and testing.

- **Unity-Native**  
  Built around Unity’s component and event systems.

- **Validated Transitions**  
  Transition rules can define forbidden or restricted paths.

- **Testable**  
  Individual states can be tested in isolation.

## 🏃 Movement Strategies

FSM uses the **Strategy Pattern** to assign specific movement behaviors per state through the `IMovementStrategy` interface.

### Built-in Strategies

- `DefaultMovementStrategy`  
  Ground movement with acceleration, deceleration, and clamping.

- `JumpMovementStrategy`  
  Vertical and horizontal movement control for jumping.

- `ConstantSpeedMovementStrategy`  
  Applies fixed velocity, ignoring input – useful for scripted movement.

### Extending Movement

- Easy to add custom strategies
- Each state can have its own movement logic, improving clarity and reusability.

## 🎮 Input Handling & AI Controllers

Input is abstracted via the `IUnitController` interface, allowing for swappable control logic between players and AI.

### Components

- `IUnitController`  
  Interface for any controller that interacts with the FSM.

- `InputHandler`  
  Implements player input via Unity's Input System.

- `EnemyAIController`  
  Implement AI-driven input logic like patrolling or chasing.

### Benefits

- **Swappable Controllers**  
  Assign different input handlers in the editor or at runtime.

- **Shared FSM**  
  Use the same FSM and movement logic for both players and enemies.

- **Unified Input Simulation**  
  AI can simulate player input events like jump or attack.
