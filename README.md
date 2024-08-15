# HumanActivityReconstruction
Human Activity Recognition (HAR) has been extensively researched over the past few decades due to its significant potential in applications such as healthcare, sports, smart homes, and security. The ubiquitous nature of smart devices which generate rich streams of motion data has accelerated advancements in this field. HAR research often requires extensive datasets, which are costly and time-consuming to produce. Generative networks can be used to synthetically generate data to address this, but currently, there is no method to visually reconstruct sensor data to validate its consistency with activity labels, ensuring trust in the synthetically generated data. Visual reconstruction helps identify faulty training data and provides a metric to assess the reliability of synthetic data. This study uses motion tracking data to visually reconstruct human movement in three dimensions, with six degrees of freedom, in Unity. We discuss the necessary data preprocessing steps to enhance the quality of training data for generative networks. Gyroscope data is used to derive orientation, while acceleration data is used to derive displacement information. This data was then emulated to visually reconstruct movements on a cube object within Unity game engine, post which manual classification was attempted to validate the reconstructed data against ground truth labels. The validation process showed that our proposed system was comprehensible by 24 survey participants with 63% accuracy. Our approach aids in manually classifying human activities and validating them against a set of ground truth labels, providing a system to improve the consistency and reliability of generative networks and their training data.

# Reproducibility
To run the project as is, download the Project from Unity folder in the repo and add the project to your Unity Hub.
The CSV files are already included inside the project in the "Assets/WISDM Datasets/" path segregated by the actvity type.
Alternatively they can be accessed from the Data folder in the repo.

To view the visual reconstruction of any file, copy the path of the data file that needs to be reconstructed.
Then follow the steps in the video:

https://github.com/user-attachments/assets/326623c9-11a9-4d51-aa58-04b9e6dce4ec

 
