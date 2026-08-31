# 🎮 2D Platformer Game — Bachelor's Thesis Project

## 📌 Project Overview

This repository contains the source code, game assets, and technical implementation for my **Bachelor's Qualification Thesis** at **Dnipro University of Technology** (Faculty of Information Technologies, Major 121: *Software Engineering*).

The project focuses on the design, architectural software patterns, performance optimization, and development of an action-oriented **2D Platformer Game** built using the **Unity Engine** and **C#**.

---

## ✨ Key Features & Mechanics

- **Player Dynamics & Movement:** Responsive player controls including smooth running, variable jump heights, wall jumping, dashing, and melee/ranged attack systems.
- **State Machine Architecture:** Finite State Machines (FSM) implemented for modular character control and clean animation transitions.
- **Boss & Enemy AI:** Dynamic AI behavior trees incorporating target pathfinding, multi-phase combat logic, and telegraphing attacks.
- **Interactive In-Game Shop & Inventory:** Real-time economy system enabling players to collect currency, purchase upgrades, and manage items.
- **Level Design & Tilemaps:** Detailed environment setup utilizing Unity Tilemaps, Rule Tiles, custom collision layers, and parallax background scrolling.
- **UI & HUD System:** Modular User Interface built using Unity UI, tracking health, stamina, coin count, and boss health bars.

---

## 🏗 Technical Architecture & Design Patterns

The codebase adheres to solid Object-Oriented Programming (OOP) principles and software design patterns to ensure scalability and maintainability:

* **Finite State Machine (FSM):** Handles complex player states (*Idle, Run, Jump, Dash, Attack*) and enemy AI combat states.
* **Singleton Pattern:** Utilized for global managers (e.g., `GameManager`, `UIManager`).
* **Observer Pattern / C# Events:** Used to decouple UI updates and audio triggers from gameplay event triggers (e.g., player health changes, enemy death).
* **Scriptable Objects:** Implemented for modular data storage of item attributes, enemy stats, and player upgrade configurations.

---

## 🛠 Tech Stack & Tools

* **Game Engine:** Unity
* **Language:** C# (.NET Framework)
* **IDE:** Visual Studio
* **Version Control:** Git & Git LFS
* **Graphics & Animation:** 2D Tilemaps, Sprite Atlases, Animator Components
