import imufusion
import matplotlib.pyplot as plt
import numpy as np
import sys
import seaborn as sns
import os
import csv
import pandas as pd
import glob


### Function to plot activities for each subject

def plot_activity_per_subject(df):

    # Get unique subjects
    unique_subjects = df['Subject-id_accel'].unique()

    # Define number of rows and columns
    if (len(unique_subjects)%5 == 0):
        n_rows = len(unique_subjects)//5
        n_cols = 5
    else:
        n_rows = len(unique_subjects)//5 + 1
        n_cols = 5

    # Create a figure with subplots
    fig, axes = plt.subplots(n_rows, n_cols, figsize=(20, 30), sharex=False, sharey=False)
    axes = axes.flatten()

    # Plot data for each subject
    for i, subject in enumerate(unique_subjects):
        if i >= n_rows * n_cols:
            break       # Only plot up to n_rows * n_cols subjects
        
        subject_data = df[df['Subject-id_accel'] == subject]
        ax = axes[i]
        ax.plot(subject_data['Timestamp_accel'], subject_data['accel_x'], label='accel_x')
        ax.plot(subject_data['Timestamp_accel'], subject_data['accel_y'], label='accel_y')
        ax.plot(subject_data['Timestamp_accel'], subject_data['accel_z'], label='accel_z')
        ax.set_title(f'Subject {subject}')
        ax.set_xlabel('DateTime')
        ax.set_ylabel('Acceleration')
        ax.legend(loc = 'lower right')

    df_1600 = df[df['Subject-id_accel'] == '1600']

    # Adjust layout
    plt.tight_layout()
    plt.show()

    return df_1600      # Returning final df for further plots
####################################################################################################

### Function to plot boxplots of accelerations in the 3 axes for each subject 

def plot_accel(df, df_name):
    # Set up the figure with subplots
    fig, axs = plt.subplots(3, 1, figsize=(18, 18), sharex=True)

    # Add a main title to the entire figure using the DataFrame name
    fig.suptitle(f'{df_name}', fontsize=16)

    # Plot for accel_x
    sns.boxplot(x='Subject-id_accel', y='accel_x', data=df, ax=axs[0])
    axs[0].set_xlabel('')
    axs[0].set_ylabel('accel_x')

    # Plot for accel_y
    sns.boxplot(x='Subject-id_accel', y='accel_y', data=df, ax=axs[1])
    axs[1].set_xlabel('')
    axs[1].set_ylabel('accel_y')

    # Plot for accel_z
    sns.boxplot(x='Subject-id_accel', y='accel_z', data=df, ax=axs[2])
    axs[2].set_xlabel('Subject-id_accel')
    axs[2].set_ylabel('accel_z')

    # Adjust layout to make room for the main title
    plt.tight_layout(rect=[0, 0, 1, 0.98])

    # Show the plots
    plt.show()
###################################################################################################

### Function to derive velocity and displacement from acceleration for each observation

def modify_df(df):
    time_differences = []

    for i in range(len(df)):
        difference = (df['DateTime'].iloc[i] - df['DateTime'].iloc[0]).total_seconds() * 1_000_000
        time_differences.append(difference)

    # Add the time differences as a new column in the DataFrame
    df['Time_Difference'] = time_differences

    df['Difference_Microseconds'] = df['Time_Difference'].diff()

    df = df.reset_index(drop = True)

    df['vx'] = 0
    df['vy'] = 0
    df['vz'] = 0

    # Calculate time difference in seconds
    dt = df['Difference_Microseconds'] / 1_000_000

    df['prev_x'] = df['accel_x'].shift(1, fill_value=0)
    df['prev_y'] = df['accel_y'].shift(1, fill_value=0)
    df['prev_z'] = df['accel_z'].shift(1, fill_value=0)

    df['vx'] = 0.5 * (df['prev_x'] + df['accel_x']) * dt
    df['vy'] = 0.5 * (df['prev_y'] - 9.8 + df['accel_y'] - 9.8) * dt
    df['vz'] = 0.5 * (df['prev_z'] + df['accel_z']) * dt

    df = df.fillna(0)

    df.drop(columns=['prev_x', 'prev_y', 'prev_z'], inplace=True)


    # Calculate distance by integrating velocity
    # Initialize distance
    df['dx'] = 0
    df['dy'] = 0
    df['dz'] = 0

    # Calculate time difference in seconds
    dt = df['Difference_Microseconds'] / 1_000_000

    df['prev_dx'] = df['vx'].shift(1, fill_value=0)
    df['prev_dy'] = df['vy'].shift(1, fill_value=0)
    df['prev_dz'] = df['vz'].shift(1, fill_value=0)

    df['dx'] = 0.5 * (df['prev_dx'] + df['vx']) * dt
    df['dy'] = 0.5 * (df['prev_dy'] + df['vy']) * dt
    df['dz'] = 0.5 * (df['prev_dz'] + df['vz']) * dt

    df = df.fillna(0)

    df['Time_secs'] = df['Timestamp_accel']*1e-9
    df['Time_diff'] = df['Time_secs'] - df['Time_secs'].iloc[0]

    df = df[df['Time_diff'] > 15]

    df.drop(columns=['prev_dx', 'prev_dy', 'prev_dz', 'Time_secs'], inplace=True)

    return df
#################################################################################################

# Function to store a separate dataframe for each subject and modify the acceleration value to calculate
# velocity and displacement and save them as separate csv files in a activity specific folder

def process_subjects(df, activity):
    subject_ids = df['Subject-id_accel'].unique()

    # Create a dictionary to store individual subject dataframes
    subject_dfs = {}

    # Make a separate folder for each activity
    if not os.path.exists(f'wisdm_{activity}'):
        os.makedirs(f'wisdm_{activity}')

    for subject_id in subject_ids:
        subject_df = df[df['Subject-id_accel'] == subject_id].copy()
        subject_df = modify_df(subject_df)
        subject_dfs[subject_id] = subject_df
        # Save the dataframe to a CSV file
        csv_path = os.path.join(f'wisdm_{activity}', f'{activity}_{subject_id}.csv')
        subject_df.to_csv(csv_path, index=False)

    return subject_dfs

#####################################################################################################################

# If sitting activity is used this needs to be applied to sitting since the gravity acts on a different axis (z-axis)
# for sitting when compared with other activities where it acts on the y-axis

def modify_df_sitting(df):
    time_differences = []

    for i in range(len(df)):
        difference = (df['DateTime'].iloc[i] - df['DateTime'].iloc[0]).total_seconds() * 1_000_000
        time_differences.append(difference)

    # Add the time differences as a new column in the DataFrame
    df['Time_Difference'] = time_differences

    df['Difference_Microseconds'] = df['Time_Difference'].diff()

    df = df.reset_index(drop = True)

    df['vx'] = 0
    df['vy'] = 0
    df['vz'] = 0

    # Calculate time difference in seconds
    dt = df['Difference_Microseconds'] / 1_000_000

    df['prev_x'] = df['accel_x'].shift(1, fill_value=0)
    df['prev_y'] = df['accel_y'].shift(1, fill_value=0)
    df['prev_z'] = df['accel_z'].shift(1, fill_value=0)

    df['vx'] = 0.5 * (df['prev_x'] + df['accel_x']) * dt
    df['vy'] = 0.5 * (df['prev_y'] + df['accel_y']) * dt
    df['vz'] = 0.5 * (df['prev_z'] - 9.8 + df['accel_z'] - 9.8) * dt

    df = df.fillna(0)

    df.drop(columns=['prev_x', 'prev_y', 'prev_z'], inplace=True)


    # Calculate distance by integrating velocity
    # Initialize distance
    df['dx'] = 0
    df['dy'] = 0
    df['dz'] = 0

    # Calculate time difference in seconds
    dt = df['Difference_Microseconds'] / 1_000_000

    df['prev_dx'] = df['vx'].shift(1, fill_value=0)
    df['prev_dy'] = df['vy'].shift(1, fill_value=0)
    df['prev_dz'] = df['vz'].shift(1, fill_value=0)

    df['dx'] = 0.5 * (df['prev_dx'] + df['vx']) * dt
    df['dy'] = 0.5 * (df['prev_dy'] + df['vy']) * dt
    df['dz'] = 0.5 * (df['prev_dz'] + df['vz']) * dt

    df = df.fillna(0)

    df['Time_secs'] = df['Timestamp_accel']*1e-9
    df['Time_diff'] = df['Time_secs'] - df['Time_secs'].iloc[0]

    df = df[df['Time_diff'] > 15]

    df.drop(columns=['prev_dx', 'prev_dy', 'prev_dz', 'Time_secs'], inplace=True)

    return df
##########################################################################################################

# If sitting activity is used this needs to be applied to sitting since the gravity acts on a different axis (z-axis)
# for sitting when compared with other activities where it acts on the y-axis

def process_subjects_sitting(df, activity):
    subject_ids = df['Subject-id_accel'].unique()

    # Create a dictionary to store individual subject dataframes
    subject_dfs = {}

    if not os.path.exists(f'wisdm_{activity}'):
        os.makedirs(f'wisdm_{activity}')

    for subject_id in subject_ids:
        subject_df = df[df['Subject-id_accel'] == subject_id].copy()
        subject_df = modify_df_sitting(subject_df)
        subject_dfs[subject_id] = subject_df
        # Save the dataframe to a CSV file
        csv_path = os.path.join(f'wisdm_{activity}', f'{activity}_{subject_id}.csv')
        subject_df.to_csv(csv_path, index=False)

    return subject_dfs
#############################################################################################################

### Function to fix orientation

def fix_orientation(df):
    # Step 1: Shift signals where the mean of accel_* columns is negative
    def shift_signals(group):
        for col in ['accel_x', 'accel_y', 'accel_z']:
            mean_value = group[col].mean()
            if mean_value < 0:
                shift_value = 2 * abs(mean_value)
                group[col] += shift_value
        return group

    # Apply the shift_signals function to each group
    df_1 = df.groupby('Subject-id_accel').apply(shift_signals)
    df_1.reset_index(drop=True, inplace=True)

    # Step 2: Swap accel_x and accel_y where mean of accel_x is greater than mean of accel_y
    def swap_columns(group):
        if group['accel_x'].mean() > group['accel_y'].mean():
            group[['accel_x', 'accel_y']] = group[['accel_y', 'accel_x']]
        return group

    # Apply the swap_columns function to each group
    df_1 = df_1.groupby('Subject-id_accel').apply(swap_columns)
    df_1.reset_index(drop=True, inplace=True)

    return df_1
#############################################################################################################

# If sitting activity is used this needs to be applied to sitting since the gravity acts on a different axis (z-axis)
# for sitting when compared with other activities where it acts on the y-axis

def fix_orientation_sitting(df):
    # Step 1: Shift signals where the mean of accel_* columns is negative
    def shift_signals(group):
        for col in ['accel_x', 'accel_y', 'accel_z']:
            mean_value = group[col].mean()
            if mean_value < 0:
                shift_value = 2 * abs(mean_value)
                group[col] += shift_value
        return group

    # Apply the shift_signals function to each group
    df_1 = df.groupby('Subject-id_accel').apply(shift_signals)
    df_1.reset_index(drop=True, inplace=True)

    # Step 2: Swap accel_x and accel_y where mean of accel_x is greater than mean of accel_y
    def swap_columns(group):
        if group['accel_x'].mean() > group['accel_z'].mean():
            group[['accel_x', 'accel_z']] = group[['accel_z', 'accel_x']]
        return group

    # Apply the swap_columns function to each group
    df_1 = df_1.groupby('Subject-id_accel').apply(swap_columns)
    df_1.reset_index(drop=True, inplace=True)

    return df_1
########################################################################################################################