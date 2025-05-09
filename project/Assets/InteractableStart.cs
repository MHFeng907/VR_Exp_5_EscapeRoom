using UnityEngine;
using TMPro;

public class InteractableStart : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip voiceOverClip;
    
    [Header("Subtitle Settings")]
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)]
    public string subtitleContent = "Blue... her beloved color.\nFollow its glow, and it may lead you home.";
    
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = true;

        if (subtitleText != null)
        {
            subtitleText.gameObject.SetActive(false);
        }
    }

    // Call this method to start the interaction
    public void StartInteract()
    {
        ShowSubtitle();
        PlayVoiceOver();
    }

    void ShowSubtitle()
    {
        if (subtitleText != null)
        {
            subtitleText.text = subtitleContent;
            subtitleText.gameObject.SetActive(true);
            Invoke("HideSubtitle", 8f); // Hide after 8 seconds
        }
    }

    void HideSubtitle()
    {
        if (subtitleText != null)
        {
            subtitleText.gameObject.SetActive(false);
        }
    }

    void PlayVoiceOver()
    {
        if (voiceOverClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(voiceOverClip);
        }
        else
        {
            Debug.LogWarning("Audio file not assigned or AudioSource not initialized!");
        }
    }
}