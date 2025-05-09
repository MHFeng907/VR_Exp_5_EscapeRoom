using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections;

public class TVInteraction : MonoBehaviour
{
    [Header("电视 UI")]
    public GameObject televisionUI;

    [Header("字幕与配音")]
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)] public string subtitleLine = "You’ve arrived in Clara’s mother’s bedroom... but where is she?";
    public float subtitleDuration = 5f;
    public AudioClip narrationClip;
    public AudioSource audioSource;

    [Header("场景切换")]
    public string nextSceneName = "room_2";
    public float delayBeforeSceneLoad = 5f;

    private XRGrabInteractable grabInteractable;

    private void Start()
    {
        if (televisionUI != null)
            televisionUI.SetActive(false);

        if (subtitleText != null)
            subtitleText.gameObject.SetActive(false);

        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnTVSelected);
        }
        else
        {
            Debug.LogError("[TV] XRGrabInteractable 组件未找到！");
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnTVSelected);
        }
    }

    private void OnTVSelected(SelectEnterEventArgs args)
    {
        Debug.Log("[TV] XR交互触发，显示电视 UI + 播放字幕/音频");

        if (televisionUI != null)
            televisionUI.SetActive(true);

        if (subtitleText != null)
        {
            subtitleText.text = subtitleLine;
            subtitleText.gameObject.SetActive(true);
            StartCoroutine(HideSubtitleAfterDelay(subtitleDuration));
        }

        if (audioSource != null && narrationClip != null)
        {
            audioSource.PlayOneShot(narrationClip);
        }

        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator HideSubtitleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (subtitleText != null)
        {
            subtitleText.gameObject.SetActive(false);
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeSceneLoad);
        Debug.Log($"[Scene] 切换到场景：{nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}

