using UnityEngine;
using TMPro;
using System.Collections;

public class LockboxInteract : MonoBehaviour
{
    [Header("交互设置")]
    public float interactionDistance = 10f;
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)]
    public string subtitleContent = "The lockbox seems important...";

    [Header("密码设置")]
    public TMP_InputField passwordInputField;
    public string correctPassword = "FFA500";
    public float delayBeforePasswordInput = 2f;

    [Header("音频设置")]
    public AudioClip voiceOverClip;
    public AudioClip correctPasswordClip;
    public AudioClip wrongPasswordClip;

    [Header("成功奖励")]
    public GameObject keyPrefab;

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
    private Coroutine currentAnimation;

    private bool waitingToStartInput = false; // 等待空格开启输入
    private bool isInputtingPassword = false; // 正在输入密码
    private bool interactionLocked = false;   // 【新增】是否正在交互中，防止多次点击

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

        if (passwordInputField == null)
            Debug.LogError("未设置密码输入框组件。");
        else
            passwordInputField.gameObject.SetActive(false);

        if (keyPrefab != null)
            keyPrefab.SetActive(false);

        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (waitingToStartInput && Input.GetKeyDown(KeyCode.Space))
        {
            StartPasswordInput();
        }
        else if (isInputtingPassword && Input.GetKeyDown(KeyCode.Return))
        {
            CheckPassword();
        }
    }

    void OnMouseOver()
    {
        if (playerTransform == null || subtitleText == null || isAnimating || isReturning || interactionLocked)
            return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance <= interactionDistance && Input.GetMouseButtonDown(0))
        {
            interactionLocked = true; // 锁定，不允许在一次交互未完成时再次触发
            StartCoroutine(AnimateObject(true));
            ShowSubtitle();
            PlayVoiceOver();
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
            currentAnimation = StartCoroutine(ShowPasswordInputAfterDelay(delayBeforePasswordInput));
        }
    }

    IEnumerator ShowPasswordInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (passwordInputField != null)
        {
            passwordInputField.gameObject.SetActive(true);
            passwordInputField.text = "";
            waitingToStartInput = true;
        }
    }

    void StartPasswordInput()
    {
        waitingToStartInput = false;
        isInputtingPassword = true;
        passwordInputField.ActivateInputField();
        subtitleText.text = "Enter the code and press Enter.";
    }

    void CheckPassword()
    {
        isInputtingPassword = false;
        passwordInputField.DeactivateInputField();
        passwordInputField.gameObject.SetActive(false);

        string input = passwordInputField.text.Trim();

        if (input.Equals(correctPassword))
        {
            // 正确
            if (subtitleText != null)
                subtitleText.text = "The lock clicks open!";
            if (correctPasswordClip != null)
                audioSource.PlayOneShot(correctPasswordClip);

            if (keyPrefab != null)
                keyPrefab.SetActive(true); // 显示钥匙

            StartCoroutine(EndInteraction(3f, true)); // 正确，稍后结束流程
        }
        else
        {
            // 错误
            if (subtitleText != null)
                subtitleText.text = "Incorrect code. Try again.";
            if (wrongPasswordClip != null)
                audioSource.PlayOneShot(wrongPasswordClip);

            StartCoroutine(EndInteraction(3f, false)); // 错误，也要回到初始状态
        }
    }

    IEnumerator EndInteraction(float delay, bool success)
    {
        yield return new WaitForSeconds(delay);

        if (subtitleText != null)
            subtitleText.gameObject.SetActive(false);

        // 如果失败，回到初始状态
        if (!success)
        {
            StartCoroutine(AnimateObject(false)); // 回到初始位置
        }

        ResetInteraction(); // 统一重置所有状态
    }

    void ResetInteraction()
    {
        interactionLocked = false; // 允许再次交互
        waitingToStartInput = false;
        isInputtingPassword = false;

        if (passwordInputField != null)
        {
            passwordInputField.gameObject.SetActive(false);
            passwordInputField.text = "";
        }
    }

    void PlayVoiceOver()
    {
        if (voiceOverClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(voiceOverClip);
        }
    }
}
