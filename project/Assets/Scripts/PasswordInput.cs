using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PasswordInput : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    private string enteredPassword = "";
    public string correctPassword = "20130512";

    [Header("数字按钮")]
    public List<DigitButton> numberButtons;

    public Color flashColor = Color.yellow;
    public float flashDuration = 0.2f;
    public float bounceScale = 0.9f;
    public float bounceDuration = 0.1f;

    [Header("成功时")]
    public GameObject consolePanel;
    public AudioSource successAudioSource;

    [Header("控制台按钮")]
    public Button closeButton;
    public Button runButton;

    [Header("全屏动画")]
    public Animator fullScreenAnimator;
    public string closeAnimationTrigger = "PlayClose";
    public string runAnimationTrigger = "PlayRun";

    [Header("键盘Canvas")]
    public Canvas keyboardCanvas;

    [Header("提示Canvas")]
    public Canvas tipCanvas;

    [Header("注意Canvas")] // ? 新增字段
    public Canvas attentionCanvas;

    private Dictionary<Button, Color> originalButtonColors = new Dictionary<Button, Color>();

    private void Start()
    {
        if (keyboardCanvas == null && numberButtons.Count > 0)
        {
            keyboardCanvas = numberButtons[0].button.GetComponentInParent<Canvas>();
        }

        if (consolePanel != null)
            consolePanel.SetActive(false);

        if (tipCanvas != null)
            tipCanvas.enabled = false;

        if (attentionCanvas != null)
            attentionCanvas.enabled = false; // ? 初始隐藏

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        if (runButton != null)
            runButton.onClick.AddListener(OnRunClicked);

        foreach (var digitBtn in numberButtons)
        {
            if (digitBtn.button != null)
            {
                var img = digitBtn.button.GetComponent<Image>();
                if (img != null)
                {
                    originalButtonColors[digitBtn.button] = img.color;
                }
            }

            if (digitBtn.interactable != null)
            {
                digitBtn.interactable.selectEntered.AddListener((args) => EnterDigit(digitBtn.digit));
            }
        }
    }

    public void EnterDigit(string digit)
    {
        enteredPassword += digit;
        displayText.text = enteredPassword;

        foreach (var btn in numberButtons)
        {
            if (btn.digit == digit)
            {
                StartCoroutine(FlashAndBounce(btn.button));
                break;
            }
        }

        if (enteredPassword.Length == correctPassword.Length)
        {
            CheckPassword();
        }
    }

    private void CheckPassword()
    {
        if (enteredPassword == correctPassword)
        {
            Debug.Log("密码正确，激活控制台！");
            StartCoroutine(HandleSuccessSequence());
        }
        else
        {
            Debug.Log("密码错误，开始闪红提示！");
            StartCoroutine(FlashRedAll());
            enteredPassword = "";
            displayText.text = "";
        }
    }

    private void OnCloseClicked()
    {
        if (consolePanel != null)
            consolePanel.SetActive(false);
        if (fullScreenAnimator != null)
            fullScreenAnimator.SetTrigger(closeAnimationTrigger);
    }

    private void OnRunClicked()
    {
        if (consolePanel != null)
            consolePanel.SetActive(false);
        if (fullScreenAnimator != null)
            fullScreenAnimator.SetTrigger(runAnimationTrigger);
    }

    private IEnumerator FlashAndBounce(Button btn)
    {
        Image img = btn.GetComponent<Image>();
        Vector3 originalScale = btn.transform.localScale;

        if (img != null)
        {
            Color originalColor = originalButtonColors.ContainsKey(btn) ? originalButtonColors[btn] : img.color;

            img.color = flashColor;
            btn.transform.localScale = originalScale * bounceScale;

            yield return new WaitForSeconds(flashDuration);

            img.color = originalColor;
            btn.transform.localScale = originalScale;
        }
    }

    private IEnumerator FlashRedAll()
    {
        Color errorColor = Color.red;

        foreach (var btn in numberButtons)
        {
            var img = btn.button.GetComponent<Image>();
            if (img != null)
            {
                img.color = errorColor;
            }
        }

        yield return new WaitForSeconds(0.3f);

        foreach (var btn in numberButtons)
        {
            var img = btn.button.GetComponent<Image>();
            if (img != null && originalButtonColors.TryGetValue(btn.button, out Color originalColor))
            {
                img.color = originalColor;
            }
        }
    }

    private IEnumerator FlashGreenAll()
    {
        Color successColor = Color.green;

        foreach (var btn in numberButtons)
        {
            var img = btn.button.GetComponent<Image>();
            if (img != null)
            {
                img.color = successColor;
            }
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var btn in numberButtons)
        {
            var img = btn.button.GetComponent<Image>();
            if (img != null && originalButtonColors.TryGetValue(btn.button, out Color originalColor))
            {
                img.color = originalColor;
            }
        }
    }

    private IEnumerator HandleSuccessSequence()
    {
        if (successAudioSource != null)
            successAudioSource.Play();

        yield return StartCoroutine(FlashGreenAll());

        if (keyboardCanvas != null)
            keyboardCanvas.enabled = false;

        if (consolePanel != null)
            consolePanel.SetActive(true);

        if (tipCanvas != null)
            tipCanvas.enabled = true;
            tipCanvas.gameObject.SetActive(true);


        if (attentionCanvas != null) // ? 新增逻辑
            attentionCanvas.enabled = true;
            attentionCanvas.gameObject.SetActive(true);

    }
}

[System.Serializable]
public class DigitButton
{
    public string digit;
    public Button button;
    public XRSimpleInteractable interactable;
}