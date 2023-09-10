using UnityEngine;

public class PlantTraits : MonoBehaviour
{
    [Header("Health and Stress")]
    public float initialHealth = 100.0f;
    public float health;
    public float initialStress = 0.0f;
    public float stress;

    [Header("Environmental Requirements")]
    [SerializeField] private float pHLevel;
    [SerializeField] private float ammoniaLevel;
    [SerializeField] private float nitriteLevel;
    [SerializeField] private float nitrateLevel;
    [SerializeField] private float o2Production;
    [SerializeField] private float co2Level;

    [Header("Growth and Metabolism")]
    [SerializeField] private float growthRate;
    [SerializeField] private float plantSize;
    private float nutrientUptakeRate;

    public float NutrientUptakeRate
    {
        get { return nutrientUptakeRate; }
    }

    [Header("Physical Characteristics")]
    [SerializeField] private float phosphorusLevel;
    [SerializeField] private float potassiumLevel;

    public float AmmoniaEffect { get; set; }
    public float NitrateEffect { get; set; }
    public float pHEffect { get; set; }

    private void Start()
    {
        health = initialHealth;
        stress = initialStress;
    }

    private void Update()
    {
        Debug.Log("pH Level: " + pHLevel);
        Debug.Log("Ammonia Level: " + ammoniaLevel);
        Debug.Log("Nitrite Level: " + nitriteLevel);
        Debug.Log("Nitrate Level: " + nitrateLevel);
        Debug.Log("O2 Production: " + o2Production);
        Debug.Log("CO2 Level: " + co2Level);
        Debug.Log("Growth Rate: " + growthRate);
        Debug.Log("Plant Size: " + plantSize);
        Debug.Log("Nutrient Uptake Rate: " + nutrientUptakeRate);
        Debug.Log("Ammonia Effect: " + AmmoniaEffect);
        Debug.Log("Nitrate Effect: " + NitrateEffect);
        Debug.Log("pH Effect: " + pHEffect);
        Debug.Log("Phosphorus Level: " + phosphorusLevel);
        Debug.Log("Potassium Level: " + potassiumLevel);

        float ammoniaEffect = AmmoniaEffect * Time.deltaTime;
        health += ammoniaEffect;
        stress += ammoniaEffect;

        float nitrateEffect = NitrateEffect * Time.deltaTime;
        health += nitrateEffect;
        stress += nitrateEffect;

        float pHChange = pHEffect * Time.deltaTime;
        health += pHChange;
        stress += pHChange;

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
        health = Mathf.Clamp(health, 0.0f, 100.0f);
    }

    public void UpdatePlantStress(float stressChange)
    {
        stress += stressChange;
        stress = Mathf.Clamp(stress, 0.0f, 100.0f);
    }
}
