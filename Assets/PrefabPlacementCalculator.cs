using UnityEngine;

public class PrefabPlacementCalculator : MonoBehaviour
{
    public float innerRadius = 5.0f;
    public float outerRadius = 10.0f;
    public float centralObjectSize = 13.37289f; // Diameter of the central object
    public float prefabSize = 50.0f; // Diameter of the prefab

    public int CalculateMaxPrefabs()
    {
        // Calculate the minimum distance required between prefabs, accounting for both central object and prefab sizes.
        float minDistance = (centralObjectSize + prefabSize) * 0.5f; // Half the sum of central object and prefab diameters.

        // Calculate the area of the annular region (excluding the central object).
        float annularArea = Mathf.PI * ((outerRadius * outerRadius) - (innerRadius * innerRadius));

        // Calculate the estimated number of prefabs that can be placed.
        int maxPrefabs = Mathf.FloorToInt(annularArea / (Mathf.PI * prefabSize * prefabSize));

        return maxPrefabs;
    }

    private void Start()
    {
        int maxPrefabs = CalculateMaxPrefabs();
        Debug.Log("Maximum number of prefabs without overlap: " + maxPrefabs);
    }
}
