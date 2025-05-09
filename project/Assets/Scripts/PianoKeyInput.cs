using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果使用 TextMeshPro，需要引入该命名空间

public class PianoKeyInput : MonoBehaviour
{
    public string correctPassword = "11556654433221";
    public string currentInput = "";
    public AudioSource keySound;
    public AudioSource errorSound;
    public TMP_Text successText; // 如果使用 TextMeshPro
    public TMP_Text wrongText;
    // public Text successText; // 如果使用 Unity 自带的 UI 文本
    public GameObject pianoUI; // 新增：琴键 UI 的引用
    public AudioSource backgroundMusic; // 新增：背景音乐的 AudioSource


    private void Start()
    {
        // 初始时隐藏成功提示文本
        if (successText != null)
        {
            successText.gameObject.SetActive(false);
        }
        if (wrongText != null)
        {
            wrongText.gameObject.SetActive(false);
        }
        // 初始时停止背景音乐
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }
    }

    public void OnKeyPress(string keyValue)
    {
        currentInput += keyValue;

        if (currentInput.Length == correctPassword.Length)
        {
            if (currentInput == correctPassword)
            {
                // 密码正确
                keySound.Play();
                currentInput = "";

                // 显示成功提示文本
                if (successText != null)
                {
                    //successText.text = "success";
                    successText.gameObject.SetActive(true);
                    Invoke("HiderightText", 10f);
                }
                if (pianoUI != null)
                {
                    Invoke("HidePianoUI", 10f);
                }
            }
            else
            {
                // 密码错误
                errorSound.Play();
                currentInput = "";
                // 显示失败提示文本
                if (wrongText != null)
                {
                    //wrongText.text = "wrong,input again";
                    wrongText.gameObject.SetActive(true);
                    Invoke("HideWrongText", 3f);
                    backgroundMusic.Play();
                }
            }
        }
    }
    private void HideWrongText()
    {
        if (wrongText != null)
        {
            wrongText.gameObject.SetActive(false);
        }
    }
    private void HiderightText()
    {
        if (successText != null)
        {
            successText.gameObject.SetActive(false);
        }
    }
    private void HidePianoUI()
    {
        if (pianoUI != null)
        {
            pianoUI.SetActive(false);
            // 隐藏琴键 UI 时停止背景音乐
            if (backgroundMusic != null)
            {
                backgroundMusic.Stop();
            }
        }
    }
    
}    