# Model Configuration Guide

The model can be run with the default configuration but there are also various configuration options that can be interesting for analysis. These options are controlled through JSON configuration files, 
and there are separate configuration files for two simulation modes: Turkey mode and Syria mode. 
This guide will help you understand how to configure the model according to your needs.

## Simulation Modes

### Turkey Mode
- In Turkey mode, the model simulates the movement of Syrian refugees across Turkey.

### Syria Mode
- In Syria mode, the model simulates the movement of Syrian Internally Displaced Persons (IDPs) within Syria.

## Configuration Files

- The model is configured using JSON files:
  - `config_turkey.json` for Turkey mode.
  - `config_syria.json` for Syria mode.
  
- To change the configuration, open the corresponding JSON file based on the simulation mode you intend to run.

## Simulation Parameters

### 1. startTime and endTime
- These parameters control the duration of the simulation.
- You can change the start and end dates to specify the time period for the simulation.
- Note: It is recommended not to change the year, as data sources are aligned with the default start and end dates.

### 2. Decision Making Parameters of Agents
- These parameters can be adjusted to analyze their impact on the simulation
- Decision parameters include weights and probabilities, and their values must be between 0 and 1.

### 3. Validate Mode (RefugeeAgent Section)
- Set the `Validate` parameter to either `true` or `false`:
  - If set to `true`, the simulation will run 5 times, and the results will be averaged.
  - Statistics on the number of agents activated and the types of locations chosen in the decision-making process will be printed to the console.
  - If set to `false`, the simulation will run only once.

### 4. Administrative Level (NodeLayer Section)
- You can change the simulated administrative level by adjusting the `file` parameter in the NodeLayer section.
- Simply change the number contained in the file name:
  - For Syria, you can choose a number between 1-3.
  - For Turkey, you can choose 1 or 2.

### 5. Decision Making Parameters (NodeLayer Section)
- The following decision-making parameters in the NodeLayer section can be customized:
  - `PopulationWeight`: Affects the importance of an existing migrant population when choosing the next destination.
  - `AnchorLong` and `AnchorLat`: Contain the longitude and latitude of a location where a lot of migrants are moving to in real life.
  - `LocationWeight`: Affects the importance of the distance to this location.
  - Note: All weight parameters must have values between 0 and 1.



