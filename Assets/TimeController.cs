using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }
    public float fastForwardMultiplier = 2f;
    private bool isPaused;
    public delegate void TimeScaleChanged(float newTimeScale);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public static bool IsGamePausedOrFastForwarded()
    {
        return Time.timeScale == 0f || Time.timeScale > 1f;
    }

    public static bool IsGameFastForwarded()
    {
        return Time.timeScale > 1f;
    }

    public static void FastForward(float multiplier)
    {
        Time.timeScale = multiplier;
    }
}
