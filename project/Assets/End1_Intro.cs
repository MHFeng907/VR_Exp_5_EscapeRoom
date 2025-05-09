using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class End1_Intro : MonoBehaviour
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

    [Header("场景切换")]
    public string targetSceneName = "FinalRoom";  // 设置你想跳转的目标场景名称

    void Start()
    {
        Debug.Log("[Room1Intro] === 脚本启动 ===");
        Debug.Log($"[Room1Intro] 当前场景: {SceneManager.GetActiveScene().name}");

        // 检查场景名称
        if (!SceneManager.GetActiveScene().name.Equals("End1", System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogWarning("[Room1Intro] 当前不是 End1 场景，禁用音频播放");
            if (bgmSource != null) bgmSource.enabled = false;
            if (voiceSource != null) voiceSource.enabled = false;
            this.enabled = false;
            return;
        }

        // 初始化字幕样式
        subtitleText.color = Color.white;
        subtitleText.fontSize = 70;
        subtitleText.text = introText;

        // 播放字幕
        StartCoroutine(HideSubtitleAfterDelay());

        // 播放语音
        if (voiceSource != null && voiceClip != null)
        {
            Debug.Log("[Room1Intro] 播放语音...");
            voiceSource.clip = voiceClip;
            voiceSource.Play();
            StartCoroutine(WaitForVoiceAndLoadScene(voiceClip.length));
        }
        else
        {
            Debug.LogWarning("[Room1Intro] 语音播放失败：组件或音频未设置");
        }

        // 播放背景音乐
        if (bgmSource != null && bgmClip != null)
        {
            Debug.Log("[Room1Intro] 播放背景音乐...");
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("[Room1Intro] BGM 播放失败：组件或音频未设置");
        }
    }

    IEnumerator HideSubtitleAfterDelay()
    {
        yield return new WaitForSeconds(subtitleDuration);
        if (subtitleText != null)
        {
            Debug.Log("[Room1Intro] 隐藏字幕");
            subtitleText.text = "";
        }
    }

    IEnumerator WaitForVoiceAndLoadScene(float voiceDuration)
    {
        yield return new WaitForSeconds(voiceDuration);
        Debug.Log("[Room1Intro] 语音播放完毕，等待额外 3 秒...");
        yield return new WaitForSeconds(3f);
        Debug.Log($"[Room1Intro] 切换场景至：{targetSceneName}");
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
    }

    void OnDisable()
    {
        Debug.Log("[Room1Intro] 脚本被禁用");
    }
}
