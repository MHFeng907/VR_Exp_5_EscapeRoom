using UnityEngine;
using TMPro;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("交互设置")]
    public float interactionDistance = 10f;
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)]
    public string subtitleContent = "Blue... her beloved color.\nFollow its glow, and it may lead you home.";
    
    [Header("音频设置")]
    public AudioClip voiceOverClip;
    
    [Header("动画设置")]
    public float riseHeight = 0.1f;
    public float moveTowardPlayerDistance = 0.1f;
    public float rotationSpeed = 0f;
    public float animationDuration = 1.5f;
    public float returnDuration = 1f;

    private AudioSource audioSource;
    private Transform playerTransform;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isAnimating = false;
    private bool isReturning = false;
    private float animationProgress = 0f;
    private Coroutine currentAnimation;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
            Debug.LogError("未找到标记为 'Player' 的对象。");

        if (subtitleText == null)
            Debug.LogError("未设置字幕 UI 组件。");
        else
            subtitleText.gameObject.SetActive(false);

        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void OnMouseOver()
    {
        //if (playerTransform == null || subtitleText == null || isAnimating || isReturning)
            //return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance <= interactionDistance && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(AnimateObject(true)); // 启动动画
            ShowSubtitle();
            PlayVoiceOver();
        }
    }

    IEnumerator AnimateObject(bool forward)
    {
        if (forward)
        {
            // 正向动画（上升和移动）
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
            // 返回动画
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
            
            // 8秒后隐藏字幕并返回原位
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
            
        StartCoroutine(AnimateObject(false)); // 启动返回动画
    }

    void PlayVoiceOver()
    {
        if (voiceOverClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(voiceOverClip);
        }
        else
        {
            Debug.LogWarning("音频文件未分配或 AudioSource 未初始化！");
        }
    }
}