using UnityEngine;

public class DrawAxes : MonoBehaviour
{
    public float axisLength = 1.0f; // Length of the axis lines
    public Color xColor = Color.red;
    public Color yColor = Color.green;
    public Color zColor = Color.blue;

    void OnDrawGizmos()
    {
        Gizmos.color = xColor;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * axisLength);

        Gizmos.color = yColor;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * axisLength);

        Gizmos.color = zColor;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * axisLength);
    }
}
