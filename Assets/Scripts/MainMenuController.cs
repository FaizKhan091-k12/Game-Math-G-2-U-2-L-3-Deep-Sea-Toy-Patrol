using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Main Play Button")]
        [SerializeField] private Button playButton;
        [SerializeField] private RectTransform playButtonTransform;

        [Header("Title Logo Animation")]
        [SerializeField] private RectTransform titleLogoTransform;

        [Header("Submarine Animation")]
        [SerializeField] private RectTransform yellowSubmarineTransform;

        private void Start()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
            }

            InitMenuAnimations();
        }

        private void OnEnable()
        {
            DialogueManager.Instance?.ShowDialogue(
                "Welcome to the deep sea! The toys are lost in the sand. Let's sort them out. Press Play to start your submarine patrol!",
                "vo_menu",
                true
            );
        }

        private void InitMenuAnimations()
        {
            if (titleLogoTransform != null)
            {
                titleLogoTransform.DOAnchorPosY(titleLogoTransform.anchoredPosition.y + 10f, 2.0f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            if (playButtonTransform != null)
            {
                playButtonTransform.DOScale(Vector3.one * 1.07f, 1.0f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            if (yellowSubmarineTransform != null)
            {
                yellowSubmarineTransform.DOAnchorPosY(yellowSubmarineTransform.anchoredPosition.y + 18f, 1.8f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);

                yellowSubmarineTransform.DORotate(new Vector3(0f, 0f, 3f), 2.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void OnPlayClicked()
        {
            SoundManager.Instance?.PlayStarPress();
            if (playButtonTransform != null)
            {
                playButtonTransform.DOPunchScale(Vector3.one * 0.22f, 0.4f, 8, 0.5f);
            }
            DOVirtual.DelayedCall(0.35f, () =>
            {
                DeepSeaGameManager.Instance.GoToLevel(1);
            });
        }
    }
}
