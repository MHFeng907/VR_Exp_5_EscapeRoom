using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Animator fadeAnimator;
    public string nextSceneName = "room_1";
    public float transitionDelay = 1f;

    [Header("音效设置")]
    public AudioSource sfxSource;       // 拖入一个专门播放音效的 AudioSource
    public AudioClip clickSfx;          // 拖入按钮点击音效

    public void StartGame()
    {
        if (clickSfx != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clickSfx); // 播放按钮点击音效
        }

        fadeAnimator.SetBool("out", true);
        StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(nextSceneName);
    }
}
