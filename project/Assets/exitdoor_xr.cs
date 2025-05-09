using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

namespace SojaExiles
{
    public class exitdoor_xr : MonoBehaviour
    {
        [Header("XR交互")]
        public XRBaseInteractable interactable;

        [Header("钥匙条件")]
        public GameObject key1;
        public GameObject key2;

        [Header("门动画")]
        public Animator openandclose;

        [Header("音频与字幕")]
        public AudioClip doorOpenClip;
        public AudioClip successClip;
        public TextMeshProUGUI subtitleText;
        [TextArea(2, 5)] public string doorOpenSubtitle = "The door unlocks with a click...";
        [TextArea(2, 5)] public string successSubtitle = "You escaped the room!";

        [Header("场景设置")]
        public string nextSceneName = "blackScene";
        public float delayBeforeSceneLoad = 1f;

        private AudioSource audioSource;
        private bool hasEscaped = false;

        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            if (interactable != null)
            {
                interactable.selectEntered.AddListener(OnDoorSelected);
            }
        }

        void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnDoorSelected);
            }
        }

        private void OnDoorSelected(SelectEnterEventArgs args)
        {
            if (hasEscaped) return;

            // 检查钥匙条件
            if (key1 != null && key2 != null && key1.activeSelf && key2.activeSelf)
            {
                StartCoroutine(EscapeSequence());
                hasEscaped = true;
            }
            else
            {
                if (subtitleText != null)
                {
                    subtitleText.text = "The door is locked...";
                    subtitleText.gameObject.SetActive(true);
                    StartCoroutine(HideSubtitleAfterDelay(2f));
                }
            }
        }

        IEnumerator EscapeSequence()
        {
            // 播放开门动画和音效
            if (openandclose != null) openandclose.Play("Opening");
            if (doorOpenClip != null) audioSource.PlayOneShot(doorOpenClip);
            if (subtitleText != null)
            {
                subtitleText.text = doorOpenSubtitle;
                subtitleText.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(1f); // 开门展示时间

            // 播放成功音效和字幕
            if (successClip != null) audioSource.PlayOneShot(successClip);
            if (subtitleText != null)
            {
                subtitleText.text = successSubtitle;
                subtitleText.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(delayBeforeSceneLoad);

            SceneManager.LoadScene(nextSceneName);
        }

        IEnumerator HideSubtitleAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (subtitleText != null)
            {
                subtitleText.gameObject.SetActive(false);
            }
        }
    }
}
