using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Management;

public class XRHeadPoseLimiter : MonoBehaviour
{
    [Header("头显设置")]
    public Transform xrCamera;             // XR Rig 下的 Main Camera
    public float maxRadius = 0.4f;         // 最大探头范围半径（单位：米）

    [Header("起始位置（世界坐标）")]
    public Vector3 initialRigPosition = new Vector3(12.4f, 9.103f, -11f); // XR Rig 起始位置

    private bool initialized = false;

    void Start()
    {
        StartCoroutine(WaitForXRInitThenAlign());
    }

    IEnumerator WaitForXRInitThenAlign()
    {
        // 获取 XRDisplaySubsystem
        List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(displays);

        XRDisplaySubsystem display = null;
        foreach (var d in displays)
        {
            if (d.running)
            {
                display = d;
                break;
            }
        }

        // 等待 XR 系统完成启动
        while (display == null || !display.running)
        {
            SubsystemManager.GetInstances(displays);
            foreach (var d in displays)
            {
                if (d.running)
                {
                    display = d;
                    break;
                }
            }

            yield return null;
        }

        // 读取 HMD 初始位置
        InputDevices.GetDeviceAtXRNode(XRNode.CenterEye).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headWorldPos);

        // 抵消 HMD 起始偏移，使 XR Rig 放在目标初始位置
        transform.position = initialRigPosition - headWorldPos;
        initialized = true;
    }

    void LateUpdate()
    {
        // 获取 XR 头显位置和旋转
        InputDevices.GetDeviceAtXRNode(XRNode.CenterEye).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headWorldPos);
        InputDevices.GetDeviceAtXRNode(XRNode.CenterEye).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation);

        // 限制头部偏移
        Vector3 localHeadPos = transform.InverseTransformPoint(headWorldPos);
        Vector2 flatOffset = new Vector2(localHeadPos.x, localHeadPos.z);

        if (flatOffset.magnitude > maxRadius)
        {
            flatOffset = flatOffset.normalized * maxRadius;
            localHeadPos = new Vector3(flatOffset.x, localHeadPos.y, flatOffset.y);
        }

        // 应用限制后的位置和旋转
        xrCamera.localPosition = localHeadPos;
        xrCamera.localRotation = headRotation;
    }
}
