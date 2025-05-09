using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PasswordSystem.cs
public class PasswordSystem : MonoBehaviour
{
    public CanvasGroup hintScreen;  // Õœ»Î∆¡ƒªCanvasGroup
    public AudioClip successSound;

    private string inputCode = "";

    public void OnKeyPressed(int keyNumber)
    {
        inputCode += keyNumber.ToString();
        if (inputCode.Length == 8) ValidateCode();
    }

    private void ValidateCode()
    {
        if (inputCode == "20130512")
        {
            StartCoroutine(FadeInHint());
            GetComponent<AudioSource>().PlayOneShot(successSound);
        }
    }

    IEnumerator FadeInHint()
    {
        while (hintScreen.alpha < 1)
        {
            hintScreen.alpha += Time.deltaTime * 0.5f;
            yield return null;
        }
    }
}