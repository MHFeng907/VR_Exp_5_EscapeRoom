using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EscapeDoorTrigger : MonoBehaviour
{
    [Header("Escape Settings")]
    public Transform player;
    public float escapeDistance = 1.5f;
    public string targetSceneName = "NextRoomScene";  // 你要切换到的场景名
    public float delayBeforeLoad = 1f;

    [Header("音效与字幕")]
    public AudioSource audioSource;
    public AudioClip escapeSound;
    public TextMeshProUGUI subtitleText;
    [TextArea(2, 5)]
    public string escapeSubtitle = "You escaped the room!";

    private bool hasTriggered = false;

    void Update()
    {
        if (!hasTriggered && player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= escapeDistance)
            {
                StartCoroutine(HandleEscapeSequence());
                hasTriggered = true;
            }
        }
    }

    IEnumerator HandleEscapeSequence()
    {
        // 播放音效
        if (audioSource != null && escapeSound != null)
        {
            audioSource.PlayOneShot(escapeSound);
        }

        // 显示字幕
        if (subtitleText != null)
        {
            subtitleText.text = escapeSubtitle;
            subtitleText.gameObject.SetActive(true);
        }

        // 缓冲等待
        yield return new WaitForSeconds(delayBeforeLoad);

        // 切换场景
        SceneManager.LoadScene(targetSceneName);
    }
}
