using UnityEngine;

public class PlantTraits : MonoBehaviour
{
    public float initialHealth = 100.0f;
    public float initialStress = 0.0f;

    public float health;
    public float stress;

    // Properties for the effects of ammonia, nitrate, and pH on the plant
    public float AmmoniaEffect { get; set; }
    public float NitrateEffect { get; set; }
    public float pHEffect { get; set; }

    private void Start()
    {
        // Initialize health and stress
        health = initialHealth;
        stress = initialStress;
    }

    private void Update()
    {
        // Apply the effects of ammonia, nitrate, and pH on health and stress here
        // You can use the AmmoniaEffect, NitrateEffect, and pHEffect properties 

        // Calculate the effect of ammonia on health and stress
        float ammoniaEffect = AmmoniaEffect * Time.deltaTime;
        health += ammoniaEffect;
        stress += ammoniaEffect;

        // Calculate the effect of nitrate on health and stress
        float nitrateEffect = NitrateEffect * Time.deltaTime;
        health += nitrateEffect;
        stress += nitrateEffect;

        // Calculate the effect of pH on health and stress
        float pHChange = pHEffect * Time.deltaTime;
        health += pHChange;
        stress += pHChange;

        // Ensure health and stress stay within a desired range, e.g., [0, 100]
        health = Mathf.Clamp(health, 0.0f, 100.0f);
        stress = Mathf.Clamp(stress, 0.0f, 100.0f);
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetStress()
    {
        return stress;
    }

    public void UpdatePlantHealth(float healthChange)
    {
        health += healthChange;
        health = Mathf.Clamp(health, 0.0f, 100.0f); // Ensure health stays within [0, 100] range
    }

    public void UpdatePlantStress(float stressChange)
    {
        stress += stressChange;
        stress = Mathf.Clamp(stress, 0.0f, 100.0f); // Ensure stress stays within [0, 100] range
    }
}
