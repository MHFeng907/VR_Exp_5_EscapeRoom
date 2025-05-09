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
    public List<DigitButton> numberButtons; // DigitButton自定义类

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

    [Header("键盘Canvas")]  // 新增
    public Canvas keyboardCanvas; // 需要手动拖上去，或者自动赋值

    private void Start()
    {
        // 键盘Canvas如果没手动拖，可以自动找（假设按钮都在同一个Canvas下）
        if (keyboardCanvas == null && numberButtons.Count > 0)
        {
            keyboardCanvas = numberButtons[0].button.GetComponentInParent<Canvas>();
        }

        if (consolePanel != null)
            consolePanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        if (runButton != null)
            runButton.onClick.AddListener(OnRunClicked);

        // 绑定数字按钮XR交互事件
        foreach (var digitBtn in numberButtons)
        {
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

            if (successAudioSource != null)
                successAudioSource.Play();

            // 先变绿
            StartCoroutine(FlashGreenAll());

            // 隐藏键盘
            if (keyboardCanvas != null)
            {
                keyboardCanvas.enabled = false;
            }

            // 显示控制台
            if (consolePanel != null)
                consolePanel.SetActive(true);
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
        Color originalColor = img.color;

        img.color = flashColor;
        btn.transform.localScale = originalScale * bounceScale;

        yield return new WaitForSeconds(flashDuration);

        img.color = originalColor;
        btn.transform.localScale = originalScale;
    }

    private IEnumerator FlashRedAll()
    {
        Color originalColor = Color.white;
        Color errorColor = Color.red;

        foreach (var btn in numberButtons)
        {
            if (btn.button != null)
            {
                var img = btn.button.GetComponent<Image>();
                if (img != null)
                    img.color = errorColor;
            }
        }

        yield return new WaitForSeconds(0.3f);

        foreach (var btn in numberButtons)
        {
            if (btn.button != null)
            {
                var img = btn.button.GetComponent<Image>();
                if (img != null)
                    img.color = originalColor;
            }
        }
    }

    private IEnumerator FlashGreenAll()
    {
        Color successColor = Color.green;

        foreach (var btn in numberButtons)
        {
            if (btn.button != null)
            {
                var img = btn.button.GetComponent<Image>();
                if (img != null)
                    img.color = successColor;
            }
        }

        yield return new WaitForSeconds(0.5f); // 稍微给玩家看0.5秒，然后键盘消失
    }
}

// 辅助类：DigitButton
[System.Serializable]
public class DigitButton
{
    public string digit;  // 比如"1","2"
    public Button button; // 真实UI按钮
    public XRSimpleInteractable interactable; // XR的交互器
}
