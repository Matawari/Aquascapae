using UnityEngine;

public class LightIntensityManager : MonoBehaviour
{
    public float currentLightIntensity;

    public void SimulateLightIntensity()
    {
        // Simple sinusoidal model for day-night light intensity cycle
        float amplitude = 1.0f;
        float frequency = 0.1f;
        currentLightIntensity = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * Time.time);

        // Clamp light intensity to [0, 1]
        currentLightIntensity = Mathf.Clamp(currentLightIntensity, 0, 1);
    }
}
