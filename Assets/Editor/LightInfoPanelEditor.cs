using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightInfoPanel))]
public class LightInfoPanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LightInfoPanel lightInfoPanel = (LightInfoPanel)target;

        // Draw the default inspector
        base.OnInspectorGUI();

        // Custom slider for light intensity
        if (lightInfoPanel.lightGameObject != null)
        {
            float intensity = EditorGUILayout.Slider("Light Intensity", lightInfoPanel.lightGameObject.intensity, 0, 8);
            lightInfoPanel.lightGameObject.intensity = intensity;
            lightInfoPanel.intensitySlider.value = intensity;
        }

        // Custom slider for light temperature
        float temperature = EditorGUILayout.Slider("Light Temperature", lightInfoPanel.temperatureSlider.value, 1000, 10000);
        lightInfoPanel.temperatureSlider.value = temperature;
    }
}
