using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphPlotter : MonoBehaviour
{
    public GameObject cube; // The cube whose position we're tracking
    public LineRenderer xLineRenderer; // Line renderer for x position
    public LineRenderer yLineRenderer; // Line renderer for y position
    public LineRenderer zLineRenderer; // Line renderer for z position
    public int maxPoints = 100; // Max number of points in the graph

    private List<Vector3> xPositions = new List<Vector3>();
    private List<Vector3> yPositions = new List<Vector3>();
    private List<Vector3> zPositions = new List<Vector3>();
    private float timeCounter = 0f;

    void Update()
    {
        // Record the current position
        Vector3 position = cube.transform.position;

        // Update the time counter
        timeCounter += Time.deltaTime;

        // Add the new position to the lists
        xPositions.Add(new Vector3(timeCounter, position.x, 0));
        yPositions.Add(new Vector3(timeCounter, position.y, 0));
        zPositions.Add(new Vector3(timeCounter, position.z, 0));

        // Keep the lists within the maxPoints limit
        if (xPositions.Count > maxPoints)
        {
            xPositions.RemoveAt(0);
            yPositions.RemoveAt(0);
            zPositions.RemoveAt(0);
        }

        // Update the line renderers
        UpdateLineRenderer(xLineRenderer, xPositions);
        UpdateLineRenderer(yLineRenderer, yPositions);
        UpdateLineRenderer(zLineRenderer, zPositions);
    }

    void UpdateLineRenderer(LineRenderer lineRenderer, List<Vector3> positions)
    {
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}
