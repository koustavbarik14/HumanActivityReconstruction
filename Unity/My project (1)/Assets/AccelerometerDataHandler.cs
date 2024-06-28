using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;

public class DisplacementDataHandler : MonoBehaviour
{
    public string fileName = "Assets/jogging_inst.csv";
    public string xColumnName = "x";
    public string yColumnName = "y";
    public string zColumnName = "z";

    private List<DisplacementEntry> displacementData;
    private int currentIndex = 0;
    public Transform modelTransform; // Assign your model's transform in the inspector
    public bool loop = true;
    public Rigidbody rigid;

    private Vector3 cumulativePosition = Vector3.zero;

    public float speed = 1;

    [System.Serializable]
    public class DisplacementEntry
    {
        public DateTime Timestamp;
        public Vector3 Displacement;

        public DisplacementEntry(DateTime timestamp, Vector3 displacement)
        {
            Timestamp = timestamp;
            Displacement = displacement;
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
            int xIndex = Array.IndexOf(header, xColumnName);
            int yIndex = Array.IndexOf(header, yColumnName);
            int zIndex = Array.IndexOf(header, zColumnName);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                try
                {
                    // Adjust the format string to match your timestamp format in the CSV
                    DateTime timestamp = DateTime.ParseExact(values[0], "yyyy-MM-dd HH:mm:ss.ffffff", System.Globalization.CultureInfo.InvariantCulture);
                    float x = float.Parse(values[xIndex]);
                    float y = float.Parse(values[yIndex]);
                    float z = float.Parse(values[zIndex]);

                    displacementData.Add(new DisplacementEntry(timestamp, new Vector3(x, y, z)));
                }
                catch (FormatException e)
                {
                    Debug.LogError("Error parsing timestamp: " + values[0] + " - " + e.Message);
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
                // Get current and next data points
                var currentData = displacementData[currentIndex];
                var nextData = displacementData[currentIndex + 1];

                // Update the position using displacement values
                modelTransform.localPosition = currentData.Displacement * speed;
                //modelTransform.Translate(Vector3.forward * speed * Time.deltaTime);
                //modelTransform.Translate(nextData.Displacement - currentData.Displacement);
                //modelTransform.Translate(currentData.Displacement * Time.deltaTime);
                //modelTransform.Translate(currentData.Displacement);

                //cumulativePosition += currentData.Displacement;
                //modelTransform.position = cumulativePosition;

                /*Vector3 newPosition = rigid.position + modelTransform.localPosition;
                print(modelTransform.localPosition);
                rigid.MovePosition(newPosition);*/

                // Calculate the delay based on timestamps
                var timeDifference = nextData.Timestamp - currentData.Timestamp;
                float waitTime = (float)timeDifference.TotalSeconds;

                currentIndex++;
                yield return new WaitForSeconds(waitTime);
                // yield return new WaitForSeconds(0.5f * Time.deltaTime);
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
