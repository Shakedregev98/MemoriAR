using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using JetBrains.Annotations;

public class InstantiatePrefabsAround : MonoBehaviour
{
    [SerializeField]
    public GameObject centralObject;
    public List<GameObject> prefabsToInstantiate = new List<GameObject>();
    public List<Button> buttons = new List<Button>();
    public int maxInstancesPerPrefab = 1;
    public float innerRadius = 16f; // Radius of the central object
    public float outerRadius = 24f; // Maximum radius for prefab instantiation

    public TextMeshProUGUI debugText;
    public GameObject addYourNamePanel;
    


    private List<GameObject> instantiatedPrefabs = new List<GameObject>();

    public GameObject imageOldedr;
    private bool isWaitingForImageOrVideo = false;
    private int maxSize = -1;
    public string finalPath;

    /*private void Awake()
    {
        Transform panelTransform = prefabsToInstantiate[0].transform.Find("Canvas/AddyournamePanel"); // Replace "PanelName" with the actual name of your panel

        if (panelTransform != null)
        {
            addYourNamePanel = panelTransform.gameObject;

            // Debugging information
            if (addYourNamePanel != null)
            {
                Debug.Log("Found the UI panel: " + addYourNamePanel.name);
            }
            else
            {
                Debug.LogError("UI panel is null. Make sure the panel name is correct.");
            }

            // Now you can use addYourNamePanel as a reference to your UI panel
        }
        else
        {
            Debug.LogError("Panel transform is null. Make sure the panel name is correct.");
        }
    }*/

    private void Start()
    {
        if (buttons.Count != prefabsToInstantiate.Count)
        {
            Debug.LogError("Button and Prefab lists must have the same number of elements.");
            return;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            int prefabIndex = i;
            buttons[i].onClick.AddListener(() => HandleButtonClick(prefabIndex));
        }

        
        imageOldedr = prefabsToInstantiate[3];
        

        Debug.Log("The Real world coordinets of the central object are " + "latitude: " + centralObject.GetComponent<ARGeospatialCreatorAnchor>().Latitude + " longtitude: " + centralObject.GetComponent<ARGeospatialCreatorAnchor>().Longitude + " altitude: " + centralObject.GetComponent<ARGeospatialCreatorAnchor>().Altitude);
        
    }

    private void HandleButtonClick(int prefabIndex)
    {
        {
            if (centralObject == null)
            {
                Debug.LogError("Central Object is not set.");
                return;
            }

            if (prefabIndex < 0 || prefabIndex >= prefabsToInstantiate.Count)
            {
                Debug.LogError("Invalid prefab index.");
                return;
            }

            if (isWaitingForImageOrVideo)
            {
                Debug.Log("Already waiting for image or video selection. Cannot proceed.");
                return;
            }

           
            if (prefabIndex == 3)
            {
                // If the selected prefab is the fourth one, initiate image or video picking
                StartCoroutine(WaitForImageOrVideoSelection());
            }
            else
            {
                // For other prefabs, proceed with instantiation
                InstantiatePrefab(prefabIndex);
            }

        }
    }

    private IEnumerator WaitForImageOrVideoSelection()
    {
        isWaitingForImageOrVideo = true;

        // Call the method to pick image or video
        PickImageOrVideo();

        // Wait until the image or video selection is completed
        while (finalPath == null)
        {
            yield return null;
        }

        // Once image or video selection is completed, proceed with instantiation
        InstantiatePrefab(3);

        isWaitingForImageOrVideo = false;
    }

    private void PickImageOrVideo()
    {
        if (NativeGallery.CanSelectMultipleMediaTypesFromGallery())
        {
            NativeGallery.Permission permission = NativeGallery.GetMixedMediaFromGallery((path) =>
            {
                Debug.Log("Media path: " + path);
                if (path != null)
                {
                    finalPath = path;

                    // Determine if the user has picked an image, video, or neither
                    switch (NativeGallery.GetMediaTypeOfFile(path))
                    {
                        case NativeGallery.MediaType.Image:
                            Debug.Log("Picked image");
                            Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                            if (texture == null)
                            {
                                Debug.Log("Couldn't load texture from " + path);
                                return;
                            }

                            // Assign texture to a quad 
                            GameObject quad = imageOldedr;
                            quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                            quad.transform.forward = Camera.main.transform.forward;
                            quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                            Material material = quad.GetComponent<Renderer>().material;
                            if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                                material.shader = Shader.Find("Legacy Shaders/Diffuse");

                            material.mainTexture = texture;
                            break;
                        case NativeGallery.MediaType.Video:
                            Debug.Log("Picked video");
                            // Play the selected video
                            Handheld.PlayFullScreenMovie("file://" + path);
                            break;
                        default:
                            Debug.Log("Probably picked something else");
                            break;
                    }
                }
            }, NativeGallery.MediaType.Image | NativeGallery.MediaType.Video, "Select an image or video");

            Debug.Log("Permission result: " + permission);
        }
    }

    private void InstantiatePrefab(int prefabIndex)
    {
        if (instantiatedPrefabs.Count < maxInstancesPerPrefab)
        {
            Vector3 randomPosition = GetRandomPositionInFrontOfCamera();
            randomPosition = AvoidOverlap(randomPosition);

            Quaternion prefabRotation = prefabsToInstantiate[prefabIndex].transform.rotation;

            GameObject newPrefab = Instantiate(prefabsToInstantiate[prefabIndex], randomPosition, prefabRotation);

            ARGeospatialCreatorAnchor geoAnchorComponent = newPrefab.GetComponent<ARGeospatialCreatorAnchor>();

            if (geoAnchorComponent != null)
            {
                StartCoroutine(WaitForInitialization(geoAnchorComponent, newPrefab, randomPosition));
                instantiatedPrefabs.Add(newPrefab);
            }
            else
            {
                Debug.LogWarning("ARGeospatialCreatorAnchor component not found on the prefab.");
            }
        }
        else
        {
            Debug.LogWarning("Maximum instances have been reached.");
        }

        // Coroutine to wait for initialization and display coordinates.
        IEnumerator WaitForInitialization(ARGeospatialCreatorAnchor geoAnchorComponent, GameObject prefab, Vector3 position)
        {
            yield return new WaitForSeconds(0.09f); // Adjust the delay as needed.

            double latitude = geoAnchorComponent.Latitude;
            double longitude = geoAnchorComponent.Longitude;
            double altitude = geoAnchorComponent.Altitude;

            // Update the UI text component.
            debugText.text = $"Real-World Coordinates: Latitude = {latitude}, Longitude = {longitude}, Altitude = {altitude}";

            Debug.Log($"Real-World Coordinates: Latitude = {latitude}, Longitude = {longitude}, Altitude = {altitude}");

            prefab.transform.position = position;
        }
    }


    private Vector3 GetRandomPositionInFrontOfCamera()
    {
        float angle = Random.Range(210f, 330f); // Adjust the angle range as needed (for example, 3 to 9 hours).

        float randomRadius = Random.Range(innerRadius, outerRadius);

        float randomX = randomRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float randomZ = randomRadius * Mathf.Sin(angle * Mathf.Deg2Rad);

        Vector3 randomPosition = centralObject.transform.position + new Vector3(randomX, 0, randomZ);

        return randomPosition;
    }


    private Vector3 AvoidOverlap(Vector3 position)
    {
        while (IsOverlapping(position))
        {
            position = GetRandomPositionInFrontOfCamera();
        }
        return position;
    }

    // Check if the position overlaps with any other prefabs.
    private bool IsOverlapping(Vector3 position)
    {
        foreach (var obj in instantiatedPrefabs)
        {
            if (Vector3.Distance(obj.transform.position, position) < 2.0f) // Adjust the threshold as needed
            {
                return true;
            }
        }
        return false;

    }
}