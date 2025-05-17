using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RevealCanvasOnGrab : MonoBehaviour
{
    public GameObject birthdayTipCanvas;
    private bool hasShown = false;

    private void Start()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrab);
        }

        // 默认隐藏
        if (birthdayTipCanvas != null)
        {
            birthdayTipCanvas.SetActive(false);
        }
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (!hasShown && birthdayTipCanvas != null)
        {
            birthdayTipCanvas.SetActive(true);
            hasShown = true; // 只显示一次
        }
    }
}
