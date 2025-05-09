using UnityEngine;

public class ClampCameraWithinBounds : MonoBehaviour
{
    public Transform xrCamera;
    public float maxDistance = 0.4f;

    void LateUpdate()
    {
        Vector3 offset = xrCamera.localPosition;
        offset.y = 0; // 忽略上下探头
        if (offset.magnitude > maxDistance)
        {
            offset = offset.normalized * maxDistance;
            xrCamera.localPosition = new Vector3(offset.x, xrCamera.localPosition.y, offset.z);
        }
    }
}
