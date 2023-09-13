# Jupyter Notebooks Guide

This guide provides an overview of the Jupyter Notebooks included in this repository. These notebooks are essential for data preprocessing and analyzing the model outputs.

## Prerequisites

Before running the notebooks on your local machine, ensure you have installed the required Python dependencies using the `requirements.txt` file in this directory.

## Data Preprocessing Notebooks

### 1. `cleanup_turkey_refpop_data.ipynb`
- This notebook is used for cleaning the data source containing information on the existing Syrian refugee population in Turkey per province.

### 2. `cleanup_camp_data.ipynb`
- This notebook is used to remove inactive camps from the data source containing the coordinates of refugee camps.

### 3. `cleanup_conflict_data.ipynb`
- This notebook preprocesses the conflict data to make it easily integrable into the simulation.

### 4. `cleanup_Syr_idpPop_data.ipynb`
- This notebook is used to extract Internally Displaced Person's (IDP) population numbers in Syrian governorates across multiple months from multiple data sources. It is also used to extract data on the most common IDP routes.

All the data sources required for these notebooks are available in the 'data' directory.

## Model Output Analysis Notebooks

### 1. `analyse_validation_output.ipynb`
- This notebook takes in the model output CSV files from the simulation run in Turkey mode and presents the data on a map of Turkey.
- It includes maps showing the Syrian refugee population across Turkish provinces at the beginning of the simulation and at the end of simulation runs of different lengths.
- Additionally, it displays a map illustrating the most common routes taken by Syrian refugees in Turkey.

### 2. `analyse_IDP_validation_output.ipynb`
- This notebook processes the model output CSV files from the simulation run in Syria mode and presents the data on both bar charts and a map of Syria.
- It includes a map showing the IDP population across Syrian governorates at the beginning of the simulation and at the end of simulation runs of different lengths.
- Additionally, it provides visualizations illustrating the most common routes taken by IDPs in Syria.

  - Simulation output files are included in the 'data' folder, enabling these notebooks to be run without issues.
  If different output files are to be analysed, move them to the 'data' folder and ensure they have the same names as the ones already contained
  in the 'data' folder. Alternatively, you can directly modify the file paths in the notebooks' source code.



## Running the Notebooks

To execute these notebooks and analyze the data, follow these steps:

1. Install the required Python dependencies using the `requirements.txt` file.
2. Open the desired notebook in your Jupyter environment.
3. Execute the cells in the notebook sequentially.


