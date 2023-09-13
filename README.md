# Bachelor thesis: Modeling and Predicting Routes of Internally Displaced Persons: An Agent-Based Approach Using the MARS Framework in the Context of the Syrian Refugee Crisis
Welcome to the repository of the project I developed for my bachelor's thesis in computer science at the University of Applied Sciences Hamburg.
The intent of the project is to replicate and extend the research presented in the paper titled "Scalable Agent-Based Modelling of Forced Migration."


## Table of Contents

1. [Introduction](#introduction)
2. [Project Structure](#project-structure)
3. [Installation](#installation)
4. [Usage](#usage)
5. [Simulation Output](#simulation-output)


## Introduction

This repository contains the code for the software developed as part of my bachelor thesis in computer science. revolves around the study and analysis of forced migration, particularly within the context of the Syrian Refugee Crisis.

The core of this project revolves around the study and analysis of forced migration, particularly within the context of the Syrian Refugee Crisis. 
The paper I base my work on introduces an agent-based model that predicts the distribution of refugees across a host country, taking social networks into account. 
In my thesis, I take this model and reimplement it in C# using the MARS Framework. Then apply it to the prediction of routes of Internally Displaced Persons (IDPs).

## Project Structure

The repository is organized as follows:

1. Project Solution File: This is the entry point for exploring and working with the codebase. You can open the solution file to access the entire project in your preferred Integrated Development Environment (IDE).

2. Refugee Simulation Project: This directory contains all the necessary files and code to run the agent-based simulation. It's the heart of the project, where you'll find the code responsible for modeling and predicting the routes of internally displaced persons (IDPs).

3. Tests Project: The Tests Project contains the test class and test resources used during the verification process.

4. Notebooks Directory: This section contains Jupyter Notebook files used for data preprocessing and analysis of the simulation outputs. Additionally, you will find the data sources used in these notebooks.

## Installation

To run this project, follow these steps:

1. Clone this repository: `git clone [repository URL]`
2. Open the project solution file in your preferred C# development environment
3. Download the [.NET SDK](https://dotnet.microsoft.com/en-us/download)
4. Install the newest Mars.Life.Simulations NuGet-package


## Usage

To run the simulation, follow these steps:

1. Navigate to the RefugeeSimulation project and open the Program.cs file
2. The first line of code in the Program file contains the string simulationMode. Initialize it with "Turkey" if you wish to simulation the movement of Syrian refugees in Turkey. Initialize it with "Syria" if you wish to simulate the movement of IDPs in Syria.
3. ![image](https://github.com/VRibeiro01/BA_V_Ribeiro/assets/103310770/ee6f0127-3a2c-4d35-b91c-fbb67aac32c2)

4. Configure the simulation if necessary. Click here to learn about the configuration options:
6. The simulation outputs are created in the RefugeeSimulation/Model/Validation folder. You can analyse the output files as you wish or run them through the Jupyter Notebook designated for analysis.
   For more information on the available Jupyter Notebooks, click here:
   Details on simulation output files can be found in the following section.

## Simulation Output

As part of our simulation, the model generates three important output files in CSV format:

1. InitPop.csv: This file contains the Syrian Refugee or IDP population of each region of the country at the beginning of the simulation. It serves as the initial population distribution data.

2. RefPop.csv: This file contains the Syrian Refugee or IDP population of each region of the country after the simulation run. It provides insights into how the population distribution has changed due to the modeled processes.

3. Routes.csv: This file contains all the routes taken by the agents during the simulation, along with the number of agents who took each route. Each route is defined by an origin and a destination, allowing for a detailed analysis of migration patterns within the simulated environment.





