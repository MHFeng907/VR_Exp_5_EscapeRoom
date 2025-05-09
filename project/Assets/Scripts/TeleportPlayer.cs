using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeleportPlayer : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)"); // 你的XR Rig名字要对上
        if (xrOrigin != null && spawnPoint != null)
        {
            xrOrigin.transform.position = spawnPoint.position;
            xrOrigin.transform.rotation = spawnPoint.rotation;
        }
    }
}
