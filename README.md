# Reinforcement Learning Pellet Collector

This project demonstrates a reinforcement learning (RL) agent trained using Unity's ML-Agents toolkit to navigate an environment and collect randomly spawned yellow pellets. The agent is rewarded for successfully collecting pellets and penalized for colliding with walls or taking too long to complete its task. This project serves as an excellent introduction to RL concepts while showcasing a simple yet effective training scenario.

---

## 🎯 **Project Overview**

The goal of the agent is to collect all pellets within the environment as quickly as possible while avoiding penalties. At the beginning of each episode:

- The agent is placed at a random position within a bounded environment.
- A set number of yellow pellets is spawned at random positions throughout the environment.
- The agent receives continuous movement and rotation inputs to navigate.

The episode ends when:
1. The agent collects all the pellets (success).
2. The agent collides with a wall (failure).
3. Time runs out (failure).

[![Watch the Agent in Action](https://img.youtube.com/vi/7iIXe4NusvQ/0.jpg)](https://www.youtube.com/watch?v=7iIXe4NusvQ)

*Click on the thumbnail above to watch the trained agent in action!*

---

## 🚀 **Features**

### 1. **Reinforcement Learning Environment**
- **Dynamic Spawning**: Pellets are spawned at random positions at the start of every episode. The algorithm ensures pellets are placed at least a minimum distance from the agent and other pellets.
- **Agent Reward System**:
  - **+10 Reward**: For each pellet collected.
  - **+5 Bonus Reward**: For completing the collection of all pellets.
  - **-15 Penalty**: For colliding with walls or exceeding the episode's time limit.

### 2. **Agent Behavior**
- The agent uses continuous actions for:
  - Forward/backward movement.
  - Left/right rotation.
- Trained using Unity ML-Agents' PPO (Proximal Policy Optimization) algorithm.

### 3. **Real-Time Environment Feedback**
- The environment's color changes based on the agent's performance:
  - **Green**: Successful completion (all pellets collected).
  - **Red**: Wall collision (failure).
  - **Black**: Time expiration (failure).

### 4. **Customizable Parameters**
- Easily modify settings such as:
  - The number of pellets to spawn (`pelletCount`).
  - The agent's movement speed (`moveSpeed`).
  - The maximum time per episode (`timeForEpisode`).
