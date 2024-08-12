using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GraphRenderer : MonoBehaviour
{
    public GameObject cube; // Assign the cube in the inspector
    public LineRenderer lineRenderer; // Assign the LineRenderer in the inspector
    public RectTransform graphContainer; // Assign the RectTransform of the panel

    private List<Vector3> positions = new List<Vector3>();
    private float graphWidth;
    private float graphHeight;

    void Start()
    {
        graphWidth = graphContainer.rect.width;
        graphHeight = graphContainer.rect.height;
    }

    void Update()
    {
        if (cube != null)
        {
            // Add the current cube position to the list
            Vector3 cubePosition = cube.transform.position;
            positions.Add(cubePosition);

            // Update the LineRenderer with new positions
            lineRenderer.positionCount = positions.Count;

            // Normalize positions for the graph
            for (int i = 0; i < positions.Count; i++)
            {
                float normalizedX = (float)i / positions.Count * graphWidth;
                float normalizedY = (positions[i].y - cube.transform.position.y) / 10.0f * graphHeight;

                lineRenderer.SetPosition(i, new Vector3(normalizedX, normalizedY, 0));
            }
        }
    }
}
