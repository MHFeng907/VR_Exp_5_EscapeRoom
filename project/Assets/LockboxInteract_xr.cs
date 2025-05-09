using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections;

public class LockboxInteract_xr : MonoBehaviour
{
    [Header("交互组件")]
    public XRBaseInteractable interactable;
    public float interactionDistance = 3f;
    public Transform playerTransform;

    [Header("UI与字幕")]
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)]
    public string subtitleContent = "The lockbox seems important...";
    public TMP_InputField passwordInputField;
    public GameObject xrKeyboard; // XR键盘 GameObject（必须在Inspector拖入）

    [Header("密码设置")]
    public string correctPassword = "FFA500";

    [Header("音频")]
    public AudioClip voiceOverClip;
    public AudioClip correctPasswordClip;
    public AudioClip wrongPasswordClip;

    [Header("成功奖励")]
    public GameObject keyPrefab;

    [Header("动画参数")]
    public float riseHeight = 0.1f;
    public float moveTowardPlayerDistance = 0.1f;
    public float animationDuration = 1.5f;
    public float returnDuration = 1f;

    private AudioSource audioSource;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isAnimating = false;
    private bool isReturning = false;
    private bool isInputting = false;
    private bool interactionLocked = false;

    private Vector3 originalScale;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        if (keyPrefab != null) keyPrefab.SetActive(false);
        if (subtitleText != null) subtitleText.gameObject.SetActive(false);

        if (passwordInputField != null)
        {
            passwordInputField.gameObject.SetActive(false);
            passwordInputField.interactable = false;
        }

        if (xrKeyboard != null)
        {
            xrKeyboard.SetActive(false);
        }

        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnXRSelected);
        }
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnXRSelected);
        }
    }

    private void OnXRSelected(SelectEnterEventArgs args)
    {
        if (interactionLocked || isAnimating || isReturning) return;

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance <= interactionDistance)
        {
            interactionLocked = true;
            StartCoroutine(HandleInteraction());
        }
    }

    IEnumerator HandleInteraction()
    {
        yield return AnimateObject(true);
        ShowSubtitle();
        PlayAudio(voiceOverClip);
        yield return new WaitForSeconds(2f);
        ShowPasswordInput();
    }

    void ShowSubtitle()
    {
        subtitleText.text = subtitleContent;
        subtitleText.gameObject.SetActive(true);
    }

    void ShowPasswordInput()
    {
        subtitleText.gameObject.SetActive(false);

        if (passwordInputField != null)
        {
            passwordInputField.gameObject.SetActive(true);
            passwordInputField.text = "";
            passwordInputField.interactable = true;
            passwordInputField.ActivateInputField(); // 激活后键盘会弹出
        }

        if (xrKeyboard != null)
        {
            xrKeyboard.SetActive(true);
        }

        isInputting = true;
    }

    public void SubmitPassword() // 应挂在 TMP_InputField 的 OnEndEdit
    {
        if (!isInputting) return;
        isInputting = false;

        string input = passwordInputField.text.Trim();
        HidePasswordInput();

        if (input.Equals(correctPassword))
        {
            subtitleText.text = "Congratulations! You've found all the keys.\n Now head to the exit quickly!";
            subtitleText.gameObject.SetActive(true);
            PlayAudio(correctPasswordClip);
            if (keyPrefab != null) keyPrefab.SetActive(true);
            StartCoroutine(EndInteraction(true));
        }
        else
        {
            subtitleText.text = "Incorrect code. Try again.";
            subtitleText.gameObject.SetActive(true);
            PlayAudio(wrongPasswordClip);
            StartCoroutine(EndInteraction(false));
        }
    }

    void HidePasswordInput()
    {
        if (passwordInputField != null)
        {
            passwordInputField.DeactivateInputField();
            passwordInputField.interactable = false;
            passwordInputField.gameObject.SetActive(false);
        }

        if (xrKeyboard != null)
        {
            xrKeyboard.SetActive(false);
        }
    }

    IEnumerator EndInteraction(bool success)
    {
        yield return new WaitForSeconds(3f);
        subtitleText.gameObject.SetActive(false);

        if (!success)
        {
            yield return AnimateObject(false);
        }

        ResetInteraction();
    }

    IEnumerator AnimateObject(bool forward)
{
    float timer = 0f;
    float duration = forward ? animationDuration : returnDuration;

    Vector3 startScale = transform.localScale;
    Vector3 targetScale = forward ? startScale * 1.2f : originalScale; // 放大20%

    if (forward) isAnimating = true;
    else isReturning = true;

    while (timer < duration)
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        yield return null;
    }

    transform.localScale = targetScale;
    isAnimating = isReturning = false;
}


    void PlayAudio(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void ResetInteraction()
    {
        interactionLocked = false;
        isInputting = false;
        HidePasswordInput();
    }
}
