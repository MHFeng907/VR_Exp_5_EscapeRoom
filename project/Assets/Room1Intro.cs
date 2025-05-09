using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Room1Intro : MonoBehaviour
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

    void Start()
    {
        Debug.Log("[Room1Intro] === 脚本启动 ===");
        Debug.Log($"[Room1Intro] 当前场景: {SceneManager.GetActiveScene().name}");

        // 检查场景名称
        if (!SceneManager.GetActiveScene().name.Equals("Room1", System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogWarning("[Room1Intro] 当前不是 Room1 场景，禁用音频播放");
            if (bgmSource != null) bgmSource.enabled = false;
            if (voiceSource != null) voiceSource.enabled = false;
            this.enabled = false;
            return;
        }
        // 临时测试：用超大红色字体确保可见性
        subtitleText.color = Color.white;
        subtitleText.fontSize = 70;
        subtitleText.text = "=== 测试字幕 ===";
        Debug.Log($"字幕位置: {subtitleText.rectTransform.position}");

        // 检查组件引用
        if (subtitleText == null) Debug.LogError("[Room1Intro] 字幕文本组件未分配!");
        if (voiceSource == null) Debug.LogError("[Room1Intro] 语音源组件未分配!");
        if (voiceClip == null) Debug.LogError("[Room1Intro] 语音片段未分配!");
        if (bgmSource == null) Debug.LogError("[Room1Intro] BGM源组件未分配!");
        if (bgmClip == null) Debug.LogError("[Room1Intro] BGM片段未分配!");

        // 检查音频监听器
        if (FindObjectOfType<AudioListener>() == null)
            Debug.LogError("[Room1Intro] 场景中缺少音频监听器(AudioListener)!");

        // 播放字幕
        if (subtitleText != null)
        {
            Debug.Log("[Room1Intro] 显示字幕: " + introText);
            subtitleText.text = introText;
            StartCoroutine(HideSubtitleAfterDelay());
        }

        // 播放语音
        if (voiceSource != null && voiceClip != null)
        {
            Debug.Log("[Room1Intro] 尝试播放语音");
            voiceSource.PlayOneShot(voiceClip);
            StartCoroutine(CheckAudioPlaying(voiceSource, "语音"));
        }
        else
        {
            Debug.LogWarning("[Room1Intro] 语音组件缺失，无法播放");
        }

        // 播放背景音乐
        if (bgmSource != null && bgmClip != null)
        {
            Debug.Log("[Room1Intro] 尝试播放BGM");
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
            StartCoroutine(CheckAudioPlaying(bgmSource, "BGM"));
        }
        else
        {
            Debug.LogWarning("[Room1Intro] BGM组件缺失，无法播放");
        }

        // 额外检查音频设置
        StartCoroutine(CheckAudioSettings());
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

    IEnumerator CheckAudioPlaying(AudioSource source, string audioName)
    {
        yield return new WaitForSeconds(0.5f); // 等待半秒让音频有机会开始
        
        if (source == null)
        {
            Debug.LogError($"[Room1Intro] {audioName}源为空!");
            yield break;
        }

        Debug.Log($"[Room1Intro] {audioName}状态 - 播放中: {source.isPlaying}, 音量: {source.volume}, 静音: {source.mute}");

        if (!source.isPlaying)
        {
            Debug.LogError($"[Room1Intro] {audioName}没有播放! 可能原因:");
            Debug.LogError($"1. 音频片段未正确加载");
            Debug.LogError($"2. 音频源被禁用");
            Debug.LogError($"3. 音量设置为0");
            Debug.LogError($"4. 音频监听器缺失");
        }
    }

    IEnumerator CheckAudioSettings()
    {
        yield return new WaitForSeconds(1f);
        
        // 检查全局音频设置
        Debug.Log($"[Room1Intro] 全局音频设置 - 音量: {AudioListener.volume}, 暂停: {AudioListener.pause}");

        // 检查音频源详细设置
        if (voiceSource != null)
        {
            Debug.Log($"[Room1Intro] 语音源设置 - 音量: {voiceSource.volume}, " +
                     $"优先级: {voiceSource.priority}, " +
                     $"空间混合: {voiceSource.spatialBlend}, " +
                     $"循环: {voiceSource.loop}");
        }

        if (bgmSource != null)
        {
            Debug.Log($"[Room1Intro] BGM源设置 - 音量: {bgmSource.volume}, " +
                     $"优先级: {bgmSource.priority}, " +
                     $"空间混合: {bgmSource.spatialBlend}, " +
                     $"循环: {bgmSource.loop}");
        }
    }

    void OnDisable()
    {
        Debug.Log("[Room1Intro] 脚本被禁用");
    }
}