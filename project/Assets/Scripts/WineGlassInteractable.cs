using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WineGlassInteractable : MonoBehaviour
{
    public GameObject pianoUI;
    private XRGrabInteractable grabInteractable;

    private void Start()
    {
        // 初始时将琴键 UI 设置为隐藏状态
        pianoUI.SetActive(false);
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnWineGlassSelected);
        // 检查是否成功获取到组件
        // if (grabInteractable != null)
        // {
        //     // 为 selectEntered 事件添加监听器，当对象被选中时调用 OnWineGlassSelected 方法
        //     grabInteractable.selectEntered.AddListener(OnWineGlassSelected);
        //     Debug.LogError("已经获取到 XRGrabInteractable 组件。");
        // }
        // else
        // {
        //     Debug.LogError("未能获取到 XRGrabInteractable 组件，请检查 GameObject 配置。");
        // }
    }

    private void OnWineGlassSelected(SelectEnterEventArgs args)
    {
        //Debug.Log("OnWineGlassSelected 方法被调用");
        pianoUI.SetActive(true);
    }
}    