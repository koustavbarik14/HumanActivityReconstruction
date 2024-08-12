using UnityEngine;
using UnityEngine.UI;

public class DisplayPosition : MonoBehaviour
{
    public GameObject cube; // Assign the cube in the inspector
    public Text positionText; // Assign the Text element in the inspector

    void Update()
    {
        if (cube != null && positionText != null)
        {
            Vector3 position = cube.transform.position;
            float time = Time.time;
            positionText.text = $"Position: (X: {position.x:F4}, Y: {position.y:F4}, Z: {position.z:F4}) \nTime: {time:F4}";
        }
    }
}
