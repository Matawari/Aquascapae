using UnityEngine;

public class DecompositionManager : MonoBehaviour
{
    // Singleton instance
    private static DecompositionManager _instance;
    public static DecompositionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DecompositionManager>();
                if (_instance == null)
                {
                    Debug.LogError("No instance of DecompositionManager found!");
                }
            }
            return _instance;
        }
    }

    public WaterQualityParameters waterQuality; // Reference to the water quality parameters to adjust nutrient levels.
    public float decompositionRate = 0.05f; // Rate at which organic matter decomposes.
    public float nutrientReleaseRate = 0.1f; // Amount of nutrients released per unit of decomposed matter.
    [SerializeField] private WaterQualityParameters waterQualityParameters;

    private float totalOrganicMatter; // Total amount of organic matter available for decomposition.

    private void Awake()
    {
        // Ensure there's only one instance of DecompositionManager
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        totalOrganicMatter = 0.0f; // Initially, no organic matter.
    }

    private void Update()
    {
        DecomposeOrganicMatter();
    }

    // Function to add organic matter (e.g., when a fish dies).
    public void AddOrganicMatter(float amount)
    {
        totalOrganicMatter += amount;
    }

    // Function to handle decomposition and nutrient release.
    private void DecomposeOrganicMatter()
    {
        if (totalOrganicMatter > 0)
        {
            float decomposedMatter = totalOrganicMatter * decompositionRate * Time.deltaTime; // Calculate the amount of matter decomposed in this frame.
            totalOrganicMatter -= decomposedMatter; // Reduce the total organic matter.

            float nutrientsReleased = decomposedMatter * nutrientReleaseRate; // Calculate the amount of nutrients released.
            waterQuality.AdjustNutrientLevels(nutrientsReleased); // Add the released nutrients to the water.
        }
    }

    public void HandleDecomposition(float amount)
    {
        // Logic to handle decomposition and adjust nutrient levels
        if (waterQualityParameters != null)
        {
            waterQualityParameters.AdjustNutrientLevels(amount);
        }
        else
        {
            Debug.LogError("WaterQualityParameters reference is not set in DecompositionManager.");
        }
    }
}
