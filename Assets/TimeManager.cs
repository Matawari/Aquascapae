using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    private int year = 1;
    private int week = 1;
    private int day = 1;
    public float timeScaleFactor = 6.0f; // 1 hour in game = 10 minutes in real life
    private float timeIncrement;

    private void Start()
    {
        timeIncrement = (86400.0f / timeScaleFactor) / Time.timeScale; // 1 day in game
        StartCoroutine(IncrementDay());
    }

    private System.Collections.IEnumerator IncrementDay()
    {
        while (true)
        {
            day++;
            UpdateCalendarText(); // Update calendar text
            yield return new WaitForSeconds(timeIncrement);
        }
    }


    private void UpdateCalendarText()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.yearText.text = "Year: " + year;
            TimeController.Instance.weekText.text = "Week: " + week;
            TimeController.Instance.dayText.text = "Day: " + day;
        }
    }

    public float CurrentTime
    {
        get { return year * 365 * 86400 + week * 7 * 86400 + day * 86400 + Time.timeSinceLevelLoad; }
    }
}
