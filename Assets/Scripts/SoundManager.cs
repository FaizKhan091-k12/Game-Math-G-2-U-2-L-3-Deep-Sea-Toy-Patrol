using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepSeaPatrol
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        public AudioClip sfxClick;
        public AudioClip sfxStarPress;
        public AudioClip sfxCorrect;
        public AudioClip sfxWrong;
        public AudioClip sfxLaser;
        public AudioClip sfxTallyHit;
        public AudioClip sfxBundleSlash;
        public AudioClip sfxWheelTick;
        public AudioClip sfxBadgeDrop;
        public AudioClip sfxBubble;
        public AudioClip bgmUnderwater;

        private bool isMuted = false;
        public bool IsMuted => isMuted;

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

            SetupAudioSources();
        }

        private void SetupAudioSources()
        {
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
                bgmSource.volume = 0.45f;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = 0.85f;
            }

            LoadClipsIfMissing();

            if (bgmUnderwater != null)
            {
                bgmSource.clip = bgmUnderwater;
                bgmSource.Play();
            }
        }

        private void LoadClipsIfMissing()
        {
            // Resources or direct assignment fallback
            if (sfxClick == null) sfxClick = Resources.Load<AudioClip>("Audio/sfx_click");
            if (sfxStarPress == null) sfxStarPress = Resources.Load<AudioClip>("Audio/sfx_star_press");
            if (sfxCorrect == null) sfxCorrect = Resources.Load<AudioClip>("Audio/sfx_correct");
            if (sfxWrong == null) sfxWrong = Resources.Load<AudioClip>("Audio/sfx_wrong");
            if (sfxLaser == null) sfxLaser = Resources.Load<AudioClip>("Audio/sfx_laser");
            if (sfxTallyHit == null) sfxTallyHit = Resources.Load<AudioClip>("Audio/sfx_tally_hit");
            if (sfxBundleSlash == null) sfxBundleSlash = Resources.Load<AudioClip>("Audio/sfx_bundle_slash");
            if (sfxWheelTick == null) sfxWheelTick = Resources.Load<AudioClip>("Audio/sfx_wheel_tick");
            if (sfxBadgeDrop == null) sfxBadgeDrop = Resources.Load<AudioClip>("Audio/sfx_badge_drop");
            if (sfxBubble == null) sfxBubble = Resources.Load<AudioClip>("Audio/sfx_bubble");
            if (bgmUnderwater == null) bgmUnderwater = Resources.Load<AudioClip>("Audio/bgm_underwater");
        }

        public void PlaySound(AudioClip clip, float volumeScale = 1.0f)
        {
            if (isMuted || clip == null) return;
            sfxSource.PlayOneShot(clip, volumeScale);
        }

        public void PlayClick() => PlaySound(sfxClick, 0.7f);
        public void PlayStarPress() => PlaySound(sfxStarPress, 0.9f);
        public void PlayCorrect() => PlaySound(sfxCorrect, 1.0f);
        public void PlayWrong() => PlaySound(sfxWrong, 0.8f);
        public void PlayLaser() => PlaySound(sfxLaser, 0.9f);
        public void PlayTallyHit() => PlaySound(sfxTallyHit, 0.85f);
        public void PlayBundleSlash() => PlaySound(sfxBundleSlash, 0.95f);
        public void PlayWheelTick() => PlaySound(sfxWheelTick, 0.6f);
        public void PlayBadgeDrop() => PlaySound(sfxBadgeDrop, 1.0f);
        public void PlayBubble() => PlaySound(sfxBubble, 0.7f);

        public void ToggleSound()
        {
            isMuted = !isMuted;
            if (bgmSource != null)
            {
                bgmSource.mute = isMuted;
            }
            if (sfxSource != null)
            {
                sfxSource.mute = isMuted;
            }
            if (!isMuted)
            {
                PlayClick();
            }
        }
    }
}
