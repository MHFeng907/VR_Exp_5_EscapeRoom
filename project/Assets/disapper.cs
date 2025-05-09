using UnityEngine;
using TMPro;
using System.Collections;

public class PlayAudioWithSubtitle : MonoBehaviour
{
    [Header("字幕")]
    public float displayTime = 5f;
    private TextMeshProUGUI textComponent;

    [Header("语音音频")]
    public AudioSource voiceSource;
    public AudioClip voiceClip;

    [Header("背景音乐")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    void Start()
    {
        // 获取 TMP 组件
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.enabled = true;
        }

        // 播放语音
        if (voiceSource != null && voiceClip != null)
        {
            voiceSource.PlayOneShot(voiceClip);
        }

        // 播放 BGM
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        // 启动协程隐藏字幕
        StartCoroutine(HideSubtitleAfterDelay());
    }

    IEnumerator HideSubtitleAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);

        if (textComponent != null)
        {
            textComponent.enabled = false;
        }
    }
}
