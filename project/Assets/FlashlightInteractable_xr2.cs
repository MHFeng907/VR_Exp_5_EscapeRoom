using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))] // 确保挂载了 XR 抓取组件
public class FlashlightInteractable_xr2 : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 10f; // 与玩家交互的最大距离
    public TextMeshProUGUI subtitleText; // 字幕 UI
    [TextArea(2, 5)]
    public string subtitleContent = "An old flashlight... maybe it'll light the way."; // 默认字幕内容

    [Header("Audio Settings")]
    public AudioClip voiceOverClip; // 拾取时播放的音频
    //public AudioClip keyAppearClip; // 谜题触发时播放的音频

    [Header("Animation Settings")]
    public float returnSmoothSpeed = 5f; // 放下后返回原位的平滑速度

    [Header("Flashlight Settings")]
    public Light flashlightLight; // 手电筒灯光组件

    //[Header("Puzzle Settings")]
    //public Transform paintingTarget; // 谜题目标位置（如画）
    //public float activationDistance = 5f; // 手电照射触发谜题的距离
    //public GameObject hiddenKey; // 被激活的隐藏物体（如钥匙）

    //[TextArea(2, 5)]
    //public string keyAppearSubtitle = "Something sparkles nearby..."; // 谜题触发后的字幕
    //public float keyAppearSubtitleDuration = 5f; // 字幕持续时间

    private AudioSource audioSource; // 音频播放器
    private Transform playerTransform; // 玩家头部 Transform
    private Vector3 originalPosition; // 原始位置
    private Quaternion originalRotation; // 原始旋转
    private Coroutine currentAnimation; // 当前字幕协程
    private Coroutine returnCoroutine; // 返回原位协程
    //private bool hasActivatedKey = false; // 是否已经激活钥匙

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // 添加 AudioSource 组件
        audioSource.playOnAwake = false;

        playerTransform = Camera.main?.transform; // 获取 XR 摄像头（主摄像机）
        if (playerTransform == null)
            Debug.LogError("Main Camera (player head) not found!");

        if (subtitleText == null)
            Debug.LogError("Subtitle Text not assigned!");
        else
            subtitleText.gameObject.SetActive(false);

        if (flashlightLight != null)
            flashlightLight.enabled = false; // 默认关闭灯光

        //if (hiddenKey != null)
            //hiddenKey.SetActive(false); // 默认隐藏钥匙

        originalPosition = transform.position; // 记录初始位置
        originalRotation = transform.rotation; // 记录初始旋转

        var grab = GetComponent<XRGrabInteractable>(); // 获取 XR 抓取组件
        grab.selectEntered.AddListener(OnGrabbed); // 注册抓取事件
        grab.selectExited.AddListener(OnReleased); // 注册放下事件
    }

    void OnDestroy()
    {
        var grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.RemoveListener(OnGrabbed);
        grab.selectExited.RemoveListener(OnReleased);
    }

    void Update()
    {
        //CheckFlashlightProximity(); // 实时检查是否触发谜题
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (flashlightLight != null)
            flashlightLight.enabled = true; // 拾起时开启手电灯光

        ShowSubtitle(subtitleContent, 8f); // 显示字幕
        PlayVoiceOver(voiceOverClip); // 播放音频

        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine); // 如果在回位中则停止
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (flashlightLight != null)
            flashlightLight.enabled = false; // 放下时关闭灯光

        returnCoroutine = StartCoroutine(ReturnToOriginalPosition()); // 执行返回原位动画
    }

    IEnumerator ReturnToOriginalPosition()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * returnSmoothSpeed;
            transform.position = Vector3.Lerp(startPos, originalPosition, t); // 插值回位
            transform.rotation = Quaternion.Lerp(startRot, originalRotation, t); // 插值回旋转
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    void ShowSubtitle(string content, float duration)
    {
        if (subtitleText == null) return;

        subtitleText.text = content;
        subtitleText.gameObject.SetActive(true);

        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(HideAfterDelay(duration)); // 延时隐藏字幕
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (subtitleText != null)
            subtitleText.gameObject.SetActive(false);
    }

    void PlayVoiceOver(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
