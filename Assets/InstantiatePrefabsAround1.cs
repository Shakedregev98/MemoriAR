using UnityEngine;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;

public class InstantiatePrefabsAround1 : MonoBehaviour
{
    public GameObject centralObject;
    public GameObject prefabToInstantiate;
    [SerializeField]
    private int maxInstances = 50;
    public float innerRadius = 20f;
    public float outerRadius = 25f;

    public List<Vector3> potentialPositions = new List<Vector3>();
    public List<ARGeospatialCreatorAnchor> geoAnccors = new List<ARGeospatialCreatorAnchor>();
    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private float minDistanceBetweenPrefabs = 2.0f; // Minimum distance between prefabs

    private void Start()
    {
        CalculatePotentialPositions();
        
    }



    private void CalculatePotentialPositions()
    {
        for (int i = 0; i < maxInstances; i++)
        {
            Vector3 randomPosition = Vector3.zero; // Initialize randomPosition
            bool isValidPosition = false;

            // Try to find a valid position that doesn't overlap with existing prefabs.
            while (!isValidPosition)
            {
                float randomRadius = Random.Range(innerRadius, outerRadius);
                float randomAngle = Random.Range(0f, 360f);

                float randomX = randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
                float randomZ = randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

                randomPosition = centralObject.transform.position + new Vector3(randomX, 0f, randomZ);

                // Check for overlap with existing prefabs.
                isValidPosition = IsPositionValid(randomPosition);
            }

            potentialPositions.Add(randomPosition);
        }
    }




    private bool IsPositionValid(Vector3 position)
    {
        foreach (var prefab in instantiatedPrefabs)
        {
            if (Vector3.Distance(prefab.transform.position, position) < minDistanceBetweenPrefabs)
            {
                return false; // Position is not valid, there's an overlap.
            }
        }
        return true; // Position is valid, no overlap.
    }
}
