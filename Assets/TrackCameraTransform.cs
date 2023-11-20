using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TrackCameraTransform : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public Camera Camera;
    
    // Start is called before the first frame update
    void Start()
    {
        Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = "Camera position is: x: " + Camera.transform.position.x + "y: " + Camera.transform.position.y + "z: " + Camera.transform.position.z;
    }
}
