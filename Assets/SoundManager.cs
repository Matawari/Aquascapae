using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource audioSource; // Reference to the AudioSource component attached to SoundManager
    public AudioClip kachingSoundClip;
    public AudioClip insufficientFundsSoundClip;

    private float originalPitch;

    private void Awake()
    {
        // Singleton pattern to ensure there is only one instance of the SoundManager.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        originalPitch = audioSource.pitch;
    }

    public void PlayKachingSound()
    {
        if (!TimeController.IsGameFastForwarded())
        {
            audioSource.pitch = originalPitch;
        }
        else
        {
            // Set a lower pitch when in fast-forward mode to prevent high-pitched sounds.
            audioSource.pitch = originalPitch * 0.5f; // You can adjust the value as needed.
        }

        audioSource.PlayOneShot(kachingSoundClip);
    }

    public void PlayInsufficientFundsSound()
    {
        if (!TimeController.IsGameFastForwarded())
        {
            audioSource.pitch = originalPitch;
        }
        else
        {
            // Set a lower pitch when in fast-forward mode to prevent high-pitched sounds.
            audioSource.pitch = originalPitch * 0.5f; // You can adjust the value as needed.
        }

        audioSource.PlayOneShot(insufficientFundsSoundClip);
    }
}
