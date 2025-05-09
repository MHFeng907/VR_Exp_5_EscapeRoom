using UnityEngine;
using TMPro;
using System.Collections;

public class InteractFBX : MonoBehaviour
{
    [Header("交互设置")]
    public float interactionDistance = 5f;
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)]
    public string subtitleContent = "The book feels oddly warm... a hidden story awaits.";

    [Header("音频设置")]
    public AudioClip voiceOverClip;

    [Header("动画设置")]
    public float riseHeight = 0.2f; // 上升高度
    public float moveTowardPlayerDistance = 0.2f; // 稍微朝玩家移动一点
    public float rotationSpeed = 30f; // 每秒旋转度数
    public float animationDuration = 1.5f;
    public float returnDuration = 1f;

    private AudioSource audioSource;
    private Transform playerTransform;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isAnimating = false;
    private bool isReturning = false;
    private Coroutine currentAnimation;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
            Debug.LogError("[InteractableBook] 未找到标记为 'Player' 的对象！");

        if (subtitleText == null)
            Debug.LogError("[InteractableBook] 未设置字幕 UI 组件！");
        else
            subtitleText.gameObject.SetActive(false);

        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 鼠标左键
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider != null && hitInfo.collider.gameObject == gameObject)
                {
                    float distance = Vector3.Distance(playerTransform.position, transform.position);
                    Debug.Log($"[InteractableBook] 检测到点击，玩家与书的距离为 {distance}");

                    if (distance <= interactionDistance)
                    {
                        Debug.Log("[InteractableBook] 符合交互条件，开始播放字幕、音频和动画。");
                        StartCoroutine(AnimateObject(true));
                        ShowSubtitle();
                        PlayVoiceOver();
                    }
                    else
                    {
                        Debug.Log("[InteractableBook] 距离过远，无法交互。");
                    }
                }
            }
        }
    }

    IEnumerator AnimateObject(bool forward)
    {
        if (forward)
        {
            isAnimating = true;
            float timer = 0f;
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;

            Vector3 targetOffset = Vector3.up * riseHeight +
                                   (playerTransform.position - transform.position).normalized * moveTowardPlayerDistance;

            while (timer < animationDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / animationDuration;

                transform.position = Vector3.Lerp(startPosition, originalPosition + targetOffset, progress);
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            isAnimating = false;
        }
        else
        {
            isReturning = true;
            float timer = 0f;
            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;

            while (timer < returnDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / returnDuration;

                transform.position = Vector3.Lerp(currentPosition, originalPosition, progress);
                transform.rotation = Quaternion.Lerp(currentRotation, originalRotation, progress);
                yield return null;
            }

            transform.position = originalPosition;
            transform.rotation = originalRotation;
            isReturning = false;
        }
    }

    void ShowSubtitle()
    {
        if (subtitleText != null)
        {
            subtitleText.text = subtitleContent;
            subtitleText.gameObject.SetActive(true);

            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(HideAfterDelay(8f));
        }
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (subtitleText != null)
            subtitleText.gameObject.SetActive(false);

        StartCoroutine(AnimateObject(false)); // 动画回到原位
    }

    void PlayVoiceOver()
    {
        if (voiceOverClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(voiceOverClip);
        }
        else
        {
            Debug.LogWarning("[InteractableBook] 音频文件未分配或 AudioSource 未初始化！");
        }
    }
}
