using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }
    public float fastForwardMultiplier = 2f;
    private bool isPaused;
    private float timeScale = 1f;
    private float currentTime = 0f; // Current time in seconds

    public TextMeshProUGUI stateText;
    public TextMeshProUGUI clockText; // Reference to the TMP Text element for the clock

    public Button pauseButton;
    public Button playButton;
    public Button fastForward2xButton;
    public Button fastForward4xButton;

    public TextMeshProUGUI yearText;
    public TextMeshProUGUI weekText;
    public TextMeshProUGUI dayText;

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

        pauseButton.onClick.AddListener(TogglePause);
        playButton.onClick.AddListener(Resume);
        fastForward2xButton.onClick.AddListener(FastForward2x);
        fastForward4xButton.onClick.AddListener(FastForward4x);
    }

    private void Start()
    {
        StartCoroutine(UpdateTime());
    }

    private System.Collections.IEnumerator UpdateTime()
    {
        while (true)
        {
            if (!isPaused)
            {
                currentTime += Time.deltaTime * timeScale;
                UpdateClockText(currentTime);
            }
            yield return null;
        }
    }

    private void UpdateClockText(float timeInSeconds)
    {
        int hours = Mathf.FloorToInt(timeInSeconds / 3600);
        int minutes = Mathf.FloorToInt((timeInSeconds % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);

        // Format the time as HH:MM:SS
        string formattedTime = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        if (clockText != null)
        {
            clockText.text = "Time: " + formattedTime;
        }
    }

    public void TogglePause()
    {
        isPaused = true;
        timeScale = Time.timeScale;
        Time.timeScale = 0f;
        UpdateStateText("Paused");
    }

    public void Resume()
    {
        if (timeScale > 1f)
        {
            // If in FF mode, reset to normal speed
            timeScale = 1f;
            Time.timeScale = timeScale;
            UpdateStateText("Playing");
        }
        else
        {
            isPaused = false;
            Time.timeScale = timeScale;
            UpdateStateText("Playing");
        }
    }


    public void FastForward2x()
    {
        timeScale = 2f;
        Time.timeScale = timeScale;
        UpdateStateText("FF2x");
    }

    public void FastForward4x()
    {
        timeScale = 4f;
        Time.timeScale = timeScale;
        UpdateStateText("FF4x");
    }

    private void UpdateStateText(string newState)
    {
        if (stateText != null)
        {
            stateText.text = newState;
        }
    }

    public static bool IsGamePausedOrFastForwarded()
    {
        return Time.timeScale == 0f || Time.timeScale > 1f;
    }

    public static bool IsGameFastForwarded()
    {
        return Time.timeScale > 1f;
    }
}
