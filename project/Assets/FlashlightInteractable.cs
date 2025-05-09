using UnityEngine;
using TMPro;
using System.Collections;

public class FlashlightInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 10f;
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)]
    public string subtitleContent = "An old flashlight... maybe it'll light the way.";

    [Header("Audio Settings")]
    public AudioClip voiceOverClip;
    public AudioClip keyAppearClip; // 【新增】钥匙出现时播放的音频

    [Header("Animation Settings")]
    public float riseHeight = 0.1f;
    public float moveTowardPlayerDistance = 0.1f;
    public float rotationSpeed = 0f;
    public float animationDuration = 1.5f;
    public float returnDuration = 1f;

    [Header("Flashlight Settings")]
    public Light flashlightLight;
    public float longPressTime = 0.5f;
    public Vector3 carryOffset = new Vector3(0.2f, -0.2f, 0.5f);
    public float carrySmoothSpeed = 10f;
    public float returnSmoothSpeed = 5f;
    public Vector3 carryRotation = new Vector3(10f, 0f, 0f);

    [Header("Puzzle Settings")]
    public Transform paintingTarget;
    public float activationDistance = 5f;
    public GameObject hiddenKey;
    [TextArea(2, 5)]
    public string keyAppearSubtitle = "Something sparkles nearby..."; // 【新增】钥匙出现时的字幕
    public float keyAppearSubtitleDuration = 5f; // 出现字幕持续时间

    // Private variables
    private AudioSource audioSource;
    private Transform playerTransform;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isAnimating = false;
    private bool isReturning = false;
    private bool isCarrying = false;
    private bool isNear = false;
    private float mouseDownTime = 0f;
    private Coroutine currentAnimation;
    private Coroutine returnCoroutine;
    private bool hasActivatedKey = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
            Debug.LogError("Player not found!");

        if (subtitleText == null)
            Debug.LogError("Subtitle Text not assigned!");
        else
            subtitleText.gameObject.SetActive(false);

        if (flashlightLight == null)
            Debug.LogError("Flashlight Light component not found!");
        else
            flashlightLight.enabled = false;

        if (hiddenKey != null)
            hiddenKey.SetActive(false);

        if (paintingTarget == null)
            Debug.LogError("Painting Target not assigned!");

        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (playerTransform == null) return;

        isNear = Vector3.Distance(playerTransform.position, transform.position) <= interactionDistance;

        HandleInput();

        if (isCarrying && Input.GetMouseButton(0))
        {
            UpdateFlashlightPosition();
        }

        CheckFlashlightProximity();
    }

    void HandleInput()
    {
        if (!isNear) return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownTime = Time.time;
        }

        if (Input.GetMouseButton(0) && !isCarrying)
        {
            if (Time.time - mouseDownTime >= longPressTime)
            {
                PickUpFlashlight();
            }
        }

        if (Input.GetMouseButtonUp(0) && isCarrying)
        {
            DropFlashlight();
        }

        if (Input.GetMouseButtonUp(0) && !isCarrying &&
            (Time.time - mouseDownTime < longPressTime))
        {
            if (!isAnimating && !isReturning)
            {
                StartCoroutine(AnimateObject(true));
                ShowSubtitle(subtitleContent, 8f);
                PlayVoiceOver(voiceOverClip);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            ToggleFlashlight();
        }
    }

    void UpdateFlashlightPosition()
    {
        Vector3 targetPosition = playerTransform.position +
                               playerTransform.forward * carryOffset.z +
                               playerTransform.right * carryOffset.x +
                               playerTransform.up * carryOffset.y;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * carrySmoothSpeed);

        Quaternion targetRotation = playerTransform.rotation * Quaternion.Euler(carryRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * carrySmoothSpeed);
    }

    void PickUpFlashlight()
    {
        isCarrying = true;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            isReturning = false;
        }

        flashlightLight.enabled = true;
    }

    void DropFlashlight()
    {
        isCarrying = false;
        returnCoroutine = StartCoroutine(ReturnToOriginalPosition());
    }

    IEnumerator ReturnToOriginalPosition()
    {
        isReturning = true;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * returnSmoothSpeed;
            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            transform.rotation = Quaternion.Lerp(startRot, originalRotation, t);
            yield return null;
        }

        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isReturning = false;
    }

    void ToggleFlashlight()
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = !flashlightLight.enabled;
        }
    }

    IEnumerator AnimateObject(bool forward)
    {
        if (forward)
        {
            isAnimating = true;
            float timer = 0f;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            Vector3 targetOffset = Vector3.up * riseHeight +
                                 (playerTransform.position - transform.position).normalized * moveTowardPlayerDistance;

            while (timer < animationDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / animationDuration;

                transform.position = Vector3.Lerp(startPos, originalPosition + targetOffset, progress);
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            isAnimating = false;
        }
        else
        {
            isReturning = true;
            float timer = 0f;
            Vector3 currentPos = transform.position;
            Quaternion currentRot = transform.rotation;

            while (timer < returnDuration)
            {
                timer += Time.deltaTime;
                float progress = timer / returnDuration;

                transform.position = Vector3.Lerp(currentPos, originalPosition, progress);
                transform.rotation = Quaternion.Lerp(currentRot, originalRotation, progress);
                yield return null;
            }

            transform.position = originalPosition;
            transform.rotation = originalRotation;
            isReturning = false;
        }
    }

    void ShowSubtitle(string content, float duration)
    {
        if (subtitleText != null)
        {
            subtitleText.text = content;
            subtitleText.gameObject.SetActive(true);

            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(HideAfterDelay(duration));
        }
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (subtitleText != null)
            subtitleText.gameObject.SetActive(false);

        if (!isCarrying)
            StartCoroutine(AnimateObject(false));
    }

    void PlayVoiceOver(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void CheckFlashlightProximity()
    {
        if (hasActivatedKey || paintingTarget == null || flashlightLight == null)
            return;

        float distance = Vector3.Distance(transform.position, paintingTarget.position);
        if (flashlightLight.enabled && distance <= activationDistance)
        {
            if (hiddenKey != null)
            {
                hiddenKey.SetActive(true);
                hasActivatedKey = true;

                // 播放钥匙出现时的提示音和字幕
                ShowSubtitle(keyAppearSubtitle, keyAppearSubtitleDuration);
                PlayVoiceOver(keyAppearClip);
            }
        }
    }
}
