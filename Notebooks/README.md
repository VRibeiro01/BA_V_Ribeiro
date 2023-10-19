# Jupyter Notebooks Guide

This guide provides an overview of the Jupyter Notebooks included in this repository. These notebooks are essential for data preprocessing and analyzing the model outputs.

## Prerequisites

Before running the notebooks on your local machine, ensure you have installed the required Python dependencies using the `requirements.txt` file in this directory.

## Data Preprocessing Notebooks

### 1. `cleanup_conflict_data.ipynb`
- This notebook preprocesses the conflict data to make it easily integrable into the simulation.

### 2. `cleanup_Syr_idpPop_data.ipynb`
- This notebook is used to extract Internally Displaced Person's (IDP) population numbers in Syrian governorates across multiple months from multiple data sources. It is also used to extract data on the most common IDP routes. This data is used for model validation and to compare the output of scenarios run with the model.

### 3. 'cleanup_syr_resident_pop_data.ipynb'
- This notebook is used to identify the discrepancies in name spellings between the Syrian resident population data file and the Syria geographic data file.

All the data sources required for these notebooks are available in the 'data' directory.
The data directory has two sub-directories: 'turkey' and 'syria'.
The 'syria' directory contains the data sources which were prepared with using the above mentioned notebooks.
Data on IDP population estimations across Syria is found in the directory 'syria/idp_data'.


## Model Output Analysis Notebooks

### 1. `analysis_turkey_output.ipynb`
- This notebook takes in the model output CSV files from the simulation run in Turkey mode and presents the it on a map of Turkey.
- It displays a map illustrating the most common routes taken by Syrian refugees in Turkey. This map is then compared to the output of the reference model.

### 2. `analyse_IDP_validation_output.ipynb`
- This notebook processes the model output CSV files from the simulation run in Syria mode and presents the data on bar charts, a densitiy map of IDP populations across Syria and a map displaying common routes identified in the model.
- - This notebook is used to analyse model output for the scenarios.

### 3. 'Validation_Script.ipynb'
- This notebook is used to calculate the MAPE (mean absolute percentage error) of the model output of calibration experiments.



  - Simulation output files are included in the 'data' folder, enabling these notebooks to be run without issues.
  - Outputs of the Turkey simulation can be found under 'data/Turkey'.
  - Outputs of the Syria simulation can be found under 'data/Syria/scenario'.
  - Outputs of the calibration experiments can be found under data/Syria/calibration'.
    
  If different output files are to be analysed, move them to the 'data/syria/scenario' folder and ensure they have the same names as the ones already contained
  in the 'data' folder. Alternatively, you can directly modify the file paths in the notebooks' source code.



## Running the Notebooks

To execute these notebooks and analyze the data, follow these steps:

1. Install the required Python dependencies using the `requirements.txt` file.
2. Open the desired notebook in your Jupyter environment.
3. Execute the cells in the notebook sequentially.


