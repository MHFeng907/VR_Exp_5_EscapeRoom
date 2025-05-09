using UnityEngine;
using TMPro;
using System.Collections;

public class Radio : MonoBehaviour
{
    public float interactionDistance = 10f; // 交互距离
    public string subtitleText = "这是一个字幕示例"; // 字幕内容
    public AudioClip audioClip; // 需要播放的音频剪辑
    public TextMeshProUGUI subtitleUI; // 引用到字幕的 TMP Text 组件

    private Transform playerTransform;
    private AudioSource audioSource;

    void Start()
    {
        // 查找玩家对象
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("未找到标记为 'Player' 的对象。");
        }

        // 获取 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("未找到 AudioSource 组件。");
        }

        // 确保字幕 UI 被禁用
        if (subtitleUI != null)
        {
            subtitleUI.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 创建从摄像机到鼠标位置的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 如果射线击中了物体
            if (Physics.Raycast(ray, out hit))
            {
                // 检查是否点击的是当前物体
                if (hit.transform == transform)
                {
                    // 检查玩家是否在交互距离内
                    if (playerTransform != null && Vector3.Distance(playerTransform.position, transform.position) <= interactionDistance)
                    {
                        // 显示字幕并播放音频
                        StartCoroutine(ShowSubtitleAndPlayAudio());
                    }
                }
            }
        }
    }

    IEnumerator ShowSubtitleAndPlayAudio()
    {
        // 显示字幕
        if (subtitleUI != null)
        {
            subtitleUI.text = subtitleText;
            subtitleUI.gameObject.SetActive(true);
        }

        // 播放音频
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        // 等待音频播放完毕
        yield return new WaitForSeconds(audioClip.length);

        // 隐藏字幕
        if (subtitleUI != null)
        {
            subtitleUI.gameObject.SetActive(false);
        }
    }
}
