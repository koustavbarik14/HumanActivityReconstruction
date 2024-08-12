using UnityEngine;
using System.Collections.Generic;

public class CubePathRenderer : MonoBehaviour
{
    public GameObject cube; // Reference to the cube GameObject
    public float updateInterval = 0.05f; // Interval to update the line renderer

    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();
    private float timeSinceLastUpdate = 0f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.03f; // Set the width of the line
        lineRenderer.endWidth = 0.03f;
        lineRenderer.useWorldSpace = true; // Ensure the line renderer uses world space
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            // Add the cube's current position to the positions list
            positions.Add(cube.transform.position);
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());

            timeSinceLastUpdate = 0f;
        }
    }
}
