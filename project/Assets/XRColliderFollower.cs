using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class XRColliderFollower : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float maxHeadDistance = 0.4f;         // 限制头部偏移范围
    public Transform xrCamera;                  // XR Rig 内的 Main Camera
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- 1. 处理移动 ---
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 forward = new Vector3(xrCamera.forward.x, 0, xrCamera.forward.z).normalized;
        Vector3 right = new Vector3(xrCamera.right.x, 0, xrCamera.right.z).normalized;
        Vector3 direction = forward * input.y + right * input.x;

        characterController.Move(direction * moveSpeed * Time.deltaTime);

        // --- 2. 限制摄像头位置 ---
        Vector3 localPos = xrCamera.localPosition;
        Vector2 flatOffset = new Vector2(localPos.x, localPos.z);

        if (flatOffset.magnitude > maxHeadDistance)
        {
            Vector2 clamped = flatOffset.normalized * maxHeadDistance;
            xrCamera.localPosition = new Vector3(clamped.x, localPos.y, clamped.y);
        }
    }
}
