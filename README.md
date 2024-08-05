# 2D Physics Engine

## Overview

This project is a 2D physics engine implemented in C#. The engine simulates physical phenomena such as gravity, acceleration, collisions, rotations, and friction using Object-Oriented Programming (OOP) principles.

## Features

- **Gravity**: Objects experience a constant acceleration due to gravity.
- **Acceleration**: Objects can accelerate based on applied forces.
- **Collisions**: Collisions between objects are detected and resolved using impulse-based methods.
- **Rotations**: Objects can rotate and their rotations are resolved using inertia.
- **Friction**: Friction is implemented to simulate the resistance that objects experience when moving against each other or a surface.

## Implementation Details

### Gravity

Gravity is implemented as a constant downward force applied to all objects in the simulation. This force causes objects to accelerate downwards, simulating the effect of gravity.

### Acceleration

Acceleration is calculated based on the forces applied to an object. Newton's second law of motion (F = ma) is used to determine the acceleration of an object given its mass and the net force acting on it.

### Collision Resolution

Collisions between objects are detected and resolved using impulses. When a collision is detected, an impulse is applied to each object to separate them and to simulate the effect of the collision. This method ensures that the momentum and energy are conserved in the collision.

### Rotations and Inertia

Rotations are handled by calculating the moment of inertia for each object and applying torques. The rotation of an object is updated based on the applied torques and its moment of inertia, allowing for realistic rotational dynamics.

### Friction

Friction is implemented to simulate the resistance that objects experience when moving against each other or a surface. Both static and dynamic friction are modeled, allowing for realistic interactions between objects and surfaces.

## Code Structure

The project is organized into several classes, each representing a different aspect of the physics engine:

- `PhysicsEngine`: The main class that manages the simulation.
- `RigidBody`: Represents a physical object in the simulation, with properties like mass, velocity, and position.
- `CollisionResolver`: Handles collision detection and resolution between objects.
- `Force`: Represents forces applied to objects, including gravity and friction.
- `Vector2D`: A utility class for vector operations in 2D space.

## Getting Started

1. **Clone the Repository:**
   ```sh
   git clone https://github.com/charbelch1/2dPhysicsEngine.git
