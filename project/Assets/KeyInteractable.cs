using UnityEngine;
using System.Collections;

public class KeyInteractable : MonoBehaviour
{
    [Header("Light Requirements")]
    public GameObject flashlight; // 手电筒物体（GameObject）
    public float activationDistance = 2f;

    [Header("Key Settings")]
    public GameObject keyVisual;
    public Collider keyCollider;

    [Header("Effects")]
    public ParticleSystem revealEffect;
    public AudioClip revealSound;
    public Material glowMaterial;

    private Renderer keyRenderer;
    private Material originalMaterial;
    private AudioSource audioSource;
    private Light flashlightLight; // ⚡ 新增：保存手电筒的Light组件

    private bool hasBeenRevealed = false; // 一旦显现，不再隐藏

    void Start()
    {
        if (keyVisual != null)
        {
            if (keyVisual.activeSelf)
            {
                keyVisual.SetActive(false);
                Debug.Log("[KeyInteractable] 已在Start阶段隐藏钥匙。");
            }

            keyRenderer = keyVisual.GetComponent<Renderer>();
            if (keyRenderer != null)
            {
                originalMaterial = keyRenderer.material;
            }
        }
        else
        {
            Debug.LogError("[KeyInteractable] keyVisual未绑定！");
        }

        if (keyCollider != null)
        {
            keyCollider.enabled = false;
        }

        if (flashlight == null)
        {
            Debug.LogError("[KeyInteractable] 没有绑定手电筒GameObject，请检查！");
        }
        else
        {
            flashlightLight = flashlight.GetComponentInChildren<Light>();
            if (flashlightLight == null)
            {
                Debug.LogError("[KeyInteractable] 在手电筒GameObject中找不到Light组件！");
            }
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (hasBeenRevealed || flashlightLight == null)
            return; // 如果已经显现或者找不到Light就不检测

        CheckFlashlightProximity();
    }

    void CheckFlashlightProximity()
    {
        if (!flashlightLight.enabled)
        {
            Debug.Log("[KeyInteractable] 手电筒光目前是关闭状态，不检测。");
            return;
        }

        float distance = Vector3.Distance(transform.position, flashlight.transform.position);
        Debug.Log($"[KeyInteractable] 当前手电到钥匙的距离：{distance}");

        if (distance <= activationDistance)
        {
            Debug.Log("[KeyInteractable] 手电筒开启且接近，钥匙显现！");
            RevealKey();
        }
    }

    void RevealKey()
    {
        hasBeenRevealed = true;

        if (keyVisual != null)
        {
            keyVisual.SetActive(true);
            if (revealEffect != null) revealEffect.Play();
            if (revealSound != null) audioSource.PlayOneShot(revealSound);

            if (keyRenderer != null && glowMaterial != null)
            {
                keyRenderer.material = glowMaterial;
            }
        }

        if (keyCollider != null)
        {
            keyCollider.enabled = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}
