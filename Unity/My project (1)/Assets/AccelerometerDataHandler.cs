using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DisplacementDataHandler : MonoBehaviour
{
    public string fileName = "Assets/jogging_inst.csv";
    public string elapsedTimeColumnName = "elapsed_time";
    public string dxColumnName = "dx";
    public string dyColumnName = "dy";
    public string dzColumnName = "dz";
    public string xAccelColumnName = "x";
    public string yAccelColumnName = "y";
    public string zAccelColumnName = "z";
    public string rollColumnName = "roll";
    public string pitchColumnName = "pitch";
    public string yawColumnName = "yaw";
    public float speed = 1.0f;

    private List<DisplacementEntry> displacementData;
    private int currentIndex = 0;
    public Transform modelTransform;
    public bool loop = true; //true if continuous animation is desired
    private Vector3 cumulativeOriginShift = Vector3.zero;

    [System.Serializable]
    public class DisplacementEntry
    {
        public float ElapsedTime;
        public Vector3 Displacement;
        public Vector3 Acceleration;
        public float Roll;
        public float Pitch;
        public float Yaw;

        public DisplacementEntry(float elapsedTime, Vector3 displacement, Vector3 acceleration, float roll, float pitch, float yaw)
        {
            ElapsedTime = elapsedTime;
            Displacement = displacement;
            Acceleration = acceleration;
            Roll = roll;
            Pitch = pitch;
            Yaw = yaw;
        }
    }

    void Start()
    {
        displacementData = new List<DisplacementEntry>();
        LoadCSV();
        StartCoroutine(PlaybackData());
    }

    void LoadCSV()
    {
        using (var reader = new StreamReader(fileName))
        {
            var header = reader.ReadLine().Split(',');
            int elapsedTimeIndex = Array.IndexOf(header, elapsedTimeColumnName);
            int dxIndex = Array.IndexOf(header, dxColumnName);
            int dyIndex = Array.IndexOf(header, dyColumnName);
            int dzIndex = Array.IndexOf(header, dzColumnName);
            int xAccelIndex = Array.IndexOf(header, xAccelColumnName);
            int yAccelIndex = Array.IndexOf(header, yAccelColumnName);
            int zAccelIndex = Array.IndexOf(header, zAccelColumnName);
            int rollIndex = Array.IndexOf(header, rollColumnName);
            int pitchIndex = Array.IndexOf(header, pitchColumnName);
            int yawIndex = Array.IndexOf(header, yawColumnName);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                try
                {
                    float elapsedTime = float.Parse(values[elapsedTimeIndex]);
                    float dx = float.Parse(values[dxIndex]);
                    float dy = float.Parse(values[dyIndex]);
                    float dz = float.Parse(values[dzIndex]);
                    float xAccel = float.Parse(values[xAccelIndex]);
                    float yAccel = float.Parse(values[yAccelIndex]);
                    float zAccel = float.Parse(values[zAccelIndex]);
                    float roll = float.Parse(values[rollIndex]);
                    float pitch = float.Parse(values[pitchIndex]);
                    float yaw = float.Parse(values[yawIndex]);

                    Vector3 displacement = new Vector3(dx, dy, dz);
                    Vector3 acceleration = new Vector3(xAccel, yAccel, zAccel);

                    displacementData.Add(new DisplacementEntry(elapsedTime, displacement, acceleration, roll, pitch, yaw));
                }
                catch (FormatException e)
                {
                    Debug.LogError("Error parsing CSV line: " + line + " - " + e.Message);
                }
            }
        }
    }

    IEnumerator PlaybackData()
    {
        while (true)
        {
            if (currentIndex < displacementData.Count - 1)
            {
                var currentData = displacementData[currentIndex];
                var nextData = displacementData[currentIndex + 1];

                // Update position
                //cumulativeOriginShift += new Vector3(0, 0, 1); // Shift origin in the Z direction
                modelTransform.position += currentData.Displacement;

                // Update rotation using roll, pitch, and yaw
                Quaternion rotation = Quaternion.Euler(currentData.Pitch, currentData.Yaw, currentData.Roll);
                modelTransform.rotation = rotation;

                // Calculate delay based on elapsed time
                var timeDifference = nextData.ElapsedTime - currentData.ElapsedTime;
                float waitTime = timeDifference / speed;

                currentIndex++;
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                if (loop)
                {
                    currentIndex = 0; // Loop back to start for continuous playback
                }
                else
                {
                    break;
                }
            }
        }
    }
}
