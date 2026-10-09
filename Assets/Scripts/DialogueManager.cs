using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private Image captainAvatar;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private RectTransform dialogueContainer;
        [SerializeField] private Button repeatSpeechButton;

        [Header("Sprites")]
        [SerializeField] private Sprite captainIdleSprite;
        [SerializeField] private Sprite captainCheerSprite;

        [Header("Voiceover Audio")]
        [SerializeField] private AudioSource voiceSource;

        private Tween textPunchTween;
        private Tween avatarBobTween;
        private string currentDialogue = "";
        private string currentVoClipName = "";
        private float speechEndTime = 0f;

        private Dictionary<string, AudioClip> voCache = new Dictionary<string, AudioClip>();

        public bool IsSpeaking
        {
            get
            {
                if (voiceSource != null && voiceSource.isPlaying) return true;
                return Time.time < speechEndTime;
            }
        }

        public IEnumerator WaitForDialogueCompletion(float extraPadding = 0.35f)
        {
            // Give audio playback a frame to register
            yield return null;
            while (IsSpeaking)
            {
                yield return null;
            }
            if (extraPadding > 0f)
            {
                yield return new WaitForSeconds(extraPadding);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (voiceSource == null)
            {
                voiceSource = gameObject.AddComponent<AudioSource>();
                voiceSource.playOnAwake = false;
                voiceSource.loop = false;
                voiceSource.volume = 1.0f;
            }

            if (repeatSpeechButton != null)
            {
                repeatSpeechButton.onClick.AddListener(RepeatCurrentDialogue);
            }
        }

        private void Start()
        {
            StartAvatarBobbing();
        }

        private void StartAvatarBobbing()
        {
            if (captainAvatar != null)
            {
                avatarBobTween?.Kill();
                avatarBobTween = captainAvatar.rectTransform.DOAnchorPosY(captainAvatar.rectTransform.anchoredPosition.y + 6f, 1.4f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void SetAvatarMood(bool isCheering)
        {
            if (captainAvatar == null) return;
            if (isCheering && captainCheerSprite != null)
            {
                captainAvatar.sprite = captainCheerSprite;
                captainAvatar.transform.DOPunchScale(Vector3.one * 0.12f, 0.4f, 6, 0.5f);
            }
            else if (captainIdleSprite != null)
            {
                captainAvatar.sprite = captainIdleSprite;
            }
        }

        public void ShowDialogue(string text, string voClipName = null, bool isCheerful = false, float punchScale = 1.05f)
        {
            currentDialogue = text;
            currentVoClipName = voClipName;
            SetAvatarMood(isCheerful);

            if (dialogueText != null)
            {
                dialogueText.text = text;
                textPunchTween?.Kill();
                dialogueText.transform.localScale = Vector3.one * 0.95f;
                textPunchTween = dialogueText.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
            }

            if (dialogueContainer != null)
            {
                dialogueContainer.DOPunchScale(new Vector3(0.04f, 0.04f, 0f), 0.3f, 5, 0.5f);
            }

            // Fallback reading duration in case audio is missing
            float fallbackReading = Mathf.Max(3.2f, text.Length * 0.065f);
            speechEndTime = Time.time + fallbackReading;

            // Play voiceover audio clip
            PlayVoiceClip(voClipName);
        }

        // Backward compatibility overload
        public void ShowDialogue(string text, bool isCheerful)
        {
            ShowDialogue(text, null, isCheerful);
        }

        private void PlayVoiceClip(string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
            {
                if (SoundManager.Instance != null && !SoundManager.Instance.IsMuted)
                {
                    SoundManager.Instance.PlayBubble();
                }
                return;
            }

            AudioClip clip = null;
            if (voCache.ContainsKey(clipName))
            {
                clip = voCache[clipName];
            }
            else
            {
                clip = Resources.Load<AudioClip>($"Audio/{clipName}");
                if (clip != null)
                {
                    voCache[clipName] = clip;
                }
            }

            if (clip != null)
            {
                speechEndTime = Time.time + clip.length;
                if (voiceSource != null && (SoundManager.Instance == null || !SoundManager.Instance.IsMuted))
                {
                    voiceSource.Stop();
                    voiceSource.clip = clip;
                    voiceSource.Play();
                }
            }
            else if (SoundManager.Instance != null && !SoundManager.Instance.IsMuted)
            {
                SoundManager.Instance.PlayBubble();
            }
        }

        public void RepeatCurrentDialogue()
        {
            if (!string.IsNullOrEmpty(currentDialogue))
            {
                ShowDialogue(currentDialogue, currentVoClipName);
            }
        }
    }
}
