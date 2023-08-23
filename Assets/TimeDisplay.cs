using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    public Slider timeSlider;

    private void UpdateTimeScale(float newTimeScale)
    {
        timeSlider.value = newTimeScale;
    }

    public void OnSliderValueChanged(float newValue)
    {
        TimeController.FastForward(newValue);
    }
}
