using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;

public class ConsolePanelController : MonoBehaviour
{
    [Header("Buttons (XR Interactables)")]
    public XRBaseInteractable closeButtonInteractable;
    public XRBaseInteractable runButtonInteractable;

    [Header("Button Visuals")]
    public Image closeButtonImage;
    public Image runButtonImage;

    [Header("Canvases")]
    public Canvas choiceCanvas;
    public Canvas playerCanvas;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoScreen;
    public VideoClip closeClip;
    public VideoClip runClip;

    public static VideoClip CurrentClipPlaying;

    private void Start()
    {
        if (closeButtonInteractable != null)
            closeButtonInteractable.selectEntered.AddListener(OnClosePressed);

        if (runButtonInteractable != null)
            runButtonInteractable.selectEntered.AddListener(OnRunPressed);

        if (playerCanvas != null)
            playerCanvas.gameObject.SetActive(false);
    }

    private void OnClosePressed(SelectEnterEventArgs args)
    {
        if (closeButtonImage != null)
            closeButtonImage.color = Color.yellow;

        PlayVideo(closeClip);
    }

    private void OnRunPressed(SelectEnterEventArgs args)
    {
        if (runButtonImage != null)
            runButtonImage.color = Color.yellow;

        PlayVideo(runClip);
    }

    private void PlayVideo(VideoClip clip)
    {
        if (clip == null || videoPlayer == null || playerCanvas == null)
            return;

        if (choiceCanvas != null)
            choiceCanvas.gameObject.SetActive(false);

        playerCanvas.gameObject.SetActive(true);

        videoPlayer.clip = clip;
        CurrentClipPlaying = clip;
        videoPlayer.Play();

        SetRawImageFullScreen(videoScreen);
    }

    private void SetRawImageFullScreen(RawImage rawImage)
    {
        if (playerCanvas == null || rawImage == null) return;

        RectTransform rawRect = rawImage.GetComponent<RectTransform>();
        rawRect.SetParent(playerCanvas.transform, false);
        rawRect.anchorMin = Vector2.zero;
        rawRect.anchorMax = Vector2.one;
        rawRect.offsetMin = Vector2.zero;
        rawRect.offsetMax = Vector2.zero;
    }
}
