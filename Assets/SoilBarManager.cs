using UnityEngine;
using UnityEngine.UI;

public class SoilBarManager : MonoBehaviour
{
    public Image soilBar;  // The UI Image acting as the soil bar
    public BagController bagController;  // Reference to the BagController script
    private Camera _cam;  // Reference to the main camera in the scene

    private void Start()
    {
        _cam = Camera.main;  // Get the reference to the main camera
    }

    private void LateUpdate()
    {
        // Update the fill amount of the soil bar based on the soilAmount in BagController
        float fillAmount = (bagController.soilAmount >= 30) ? 1.0f : bagController.soilAmount / 30.0f;
        soilBar.fillAmount = fillAmount;

        // Update the soilBar's position to follow the bag's position
        Vector3 bagPosition = bagController.transform.position;
        soilBar.transform.position = bagPosition + new Vector3(0, 0.15f, -0.1f);  // Smaller vertical offset


        Vector3 toCamera = _cam.transform.position - soilBar.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(-toCamera, _cam.transform.up);
        soilBar.transform.rotation = lookRotation;

    }
}
