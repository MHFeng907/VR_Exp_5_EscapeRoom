using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class PianoKeyInteractable : MonoBehaviour
{
    public string keyValue;
    public UnityEvent<string> onKeyPressed;

    private XRSimpleInteractable simpleInteractable;
    public AudioSource keyAudioSource; // 新增：琴键音效的 AudioSource
    public AudioClip[] keySounds; // 新增：存储每个琴键对应的音效

    private void Start()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.AddListener(OnKeySelected);
            Debug.Log("事件绑定成功");
        }
        else
        {
            Debug.LogError("未找到 XRSimpleInteractable 组件");
        }
        // 确保有 AudioSource 组件
        if (keyAudioSource == null)
        {
            keyAudioSource = gameObject.AddComponent<AudioSource>();
        }
        keyAudioSource.playOnAwake = false; // 确保不会自动播放

        

    }

    private void OnKeySelected(SelectEnterEventArgs args)
    {
        Debug.Log("Key " + keyValue + " is selected.");
        onKeyPressed.Invoke(keyValue);
        // 根据琴键值播放对应的音效
        int keyIndex = int.Parse(keyValue) - 1;
        if (keyIndex >= 0 && keyIndex < keySounds.Length)
        {
            keyAudioSource.clip = keySounds[keyIndex];
            keyAudioSource.Play();
        }
    }
}    