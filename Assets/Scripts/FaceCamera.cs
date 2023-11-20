using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera Camera;

    private void Start()
    {
        Camera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        transform.LookAt(Camera.transform);
        transform.Rotate(0, 180, 0);
    }
}
