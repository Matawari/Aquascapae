using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int year = 1;
    private int week = 1;
    private int day = 1;

    private void Start()
    {
        StartCoroutine(IncrementDay());
    }

    private System.Collections.IEnumerator IncrementDay()
    {
        while (true)
        {
            day++;
            yield return new WaitForSeconds(1f);
        }
    }

    public float CurrentTime
    {
        get { return year * 365 * 86400 + week * 7 * 86400 + day * 86400 + Time.timeSinceLevelLoad; }
    }
}
