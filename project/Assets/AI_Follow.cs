using UnityEngine;

public class FollowXRHead : MonoBehaviour
{
    public Transform xrCamera;            // XR 头显相机
    public float distance = 2f;           // 距离相机多远
    public float verticalOffset = 0.3f;   // 向上偏移
    public float horizontalAngle = 30f;   // 向右偏移角度（正数右，负数左）
    public float smoothSpeed = 5f;        // 平滑度

    void LateUpdate()
    {
        if (xrCamera == null) return;

        // 获取方向向量（正前方）
        Vector3 forward = xrCamera.forward;
        forward.y = 0;
        forward.Normalize();

        // 绕头顶垂直轴偏转角度
        Vector3 rotatedDirection = Quaternion.Euler(0, horizontalAngle, 0) * forward;

        // 计算目标位置
        Vector3 targetPos = xrCamera.position + rotatedDirection * distance;
        targetPos.y += verticalOffset;

        // 平滑移动
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        // 始终面向头显（可略微调整角度以更自然）
        Vector3 lookDir = transform.position - xrCamera.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
