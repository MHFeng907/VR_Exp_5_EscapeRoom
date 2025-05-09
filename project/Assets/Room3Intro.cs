using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class Room3Intro : MonoBehaviour
{
    [Header("字幕设置")]
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)]
    public string introText = "Welcome to Taoke'ai's home. Find the keys to the second floor—Clara might be up there.";
    public float subtitleDuration = 6f;

    [Header("语音音频")]
    public AudioSource voiceSource;
    public AudioClip voiceClip;

    [Header("背景音乐")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;

    [Header("视频检测")]
    public VideoPlayer videoPlayer;
    public VideoClip closeClip;
    public VideoClip runClip;

    [Header("结局场景")]
    public string closeSceneName = "Ending_Close";
    public string runSceneName = "Ending_Run";
    public float bufferSeconds = 5f;

    private bool videoStarted = false;

    void Start()
    {
        Debug.Log("[Room3Intro] 脚本启动");

        // ✅ 确保仅在 Room3 场景下运行
        if (!SceneManager.GetActiveScene().name.Equals("Room3", System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogWarning("[Room3Intro] 当前不是 Room3 场景，禁用音频播放与跳转逻辑");
            if (bgmSource != null) bgmSource.enabled = false;
            if (voiceSource != null) voiceSource.enabled = false;
            this.enabled = false;
            return;
        }

        // 字幕
        if (subtitleText != null)
        {
            subtitleText.color = Color.white;
            subtitleText.fontSize = 70;
            subtitleText.text = introText;
            StartCoroutine(HideSubtitleAfterDelay());
        }

        // 语音
        if (voiceSource != null && voiceClip != null)
        {
            voiceSource.PlayOneShot(voiceClip);
        }

        // BGM
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        // 视频监听
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            StartCoroutine(WatchVideoStart());
        }
    }

    IEnumerator HideSubtitleAfterDelay()
    {
        yield return new WaitForSeconds(subtitleDuration);
        if (subtitleText != null)
        {
            subtitleText.text = "";
        }
    }

    IEnumerator WatchVideoStart()
    {
        while (!videoPlayer.isPlaying)
        {
            yield return null;
        }

        Debug.Log("[Room3Intro] 视频开始播放，暂停BGM");
        videoStarted = true;

        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("[Room3Intro] 视频播放完毕，准备跳转");
        StartCoroutine(HandleSceneTransition());
    }

    IEnumerator HandleSceneTransition()
    {
        yield return new WaitForSeconds(bufferSeconds);

        var currentClip = ConsolePanelController.CurrentClipPlaying;

        if (currentClip == runClip)
        {
            Debug.Log("[Room3Intro] 跳转至运行结局: " + runSceneName);
            SceneManager.LoadScene(runSceneName);
        }
        else if (currentClip == closeClip)
        {
            Debug.Log("[Room3Intro] 跳转至关闭结局: " + closeSceneName);
            SceneManager.LoadScene(closeSceneName);
        }
        else
        {
            Debug.LogWarning("[Room3Intro] 当前片段无法识别，未跳转");
        }
    }
}
