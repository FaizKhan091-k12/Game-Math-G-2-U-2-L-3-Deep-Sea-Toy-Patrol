using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class SummaryController : MonoBehaviour
    {
        [Header("Badge & Animations")]
        [SerializeField] private RectTransform goldBadgeTransform;
        [SerializeField] private RectTransform badgeDropStartPos;
        [SerializeField] private RectTransform badgeRestPos;

        [Header("Happy Swimming Animals")]
        [SerializeField] private List<RectTransform> celebrationAnimals = new List<RectTransform>();

        [Header("Buttons")]
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] GameObject[] fishesLevel2;

        private void Start()
        {
            if (playAgainButton != null)
            {
                playAgainButton.onClick.AddListener(() =>
                {
                    DeepSeaGameManager.Instance.GoToLevel(1);
                    foreach (GameObject item in fishesLevel2)
                    {
                        item.SetActive(false);
                        fishesLevel2[0].SetActive(true);
                        fishesLevel2[1].SetActive(true);
                        fishesLevel2[2].SetActive(true);
                        fishesLevel2[3].SetActive(true);
                    }
                });
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(() =>
                {
                    DeepSeaGameManager.Instance.GoToMainMenu();
                    foreach (GameObject item in fishesLevel2)
                    {
                        item.SetActive(false);
                        fishesLevel2[0].SetActive(true);
                        fishesLevel2[1].SetActive(true);
                        fishesLevel2[2].SetActive(true);
                        fishesLevel2[3].SetActive(true);
                    }
                });
            }
        }

        private void OnEnable()
        {
            StartCoroutine(ShowSummarySequence());
        }

        private IEnumerator ShowSummarySequence()
        {
            if (playAgainButton != null) playAgainButton.gameObject.SetActive(false);
            if (mainMenuButton != null) mainMenuButton.gameObject.SetActive(false);

            if (goldBadgeTransform != null)
            {
                goldBadgeTransform.anchoredPosition = new Vector2(0f, 600f);
                goldBadgeTransform.localScale = Vector3.one * 0.4f;
            }

            // Animate happy animals swimming
            for (int i = 0; i < celebrationAnimals.Count; i++)
            {
                if (celebrationAnimals[i] != null)
                {
                    celebrationAnimals[i].DOKill();
                    celebrationAnimals[i].DOAnchorPosY(celebrationAnimals[i].anchoredPosition.y + 18f, 1.3f + (i * 0.1f))
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                }
            }

            yield return new WaitForSeconds(0.4f);

            // Drop badge
            if (goldBadgeTransform != null)
            {
                goldBadgeTransform.DOAnchorPosY(45f, 0.9f).SetEase(Ease.OutBounce);
                goldBadgeTransform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack);
                goldBadgeTransform.DORotate(new Vector3(0f, 0f, 5f), 1.2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }

            SoundManager.Instance?.PlayBadgeDrop();

            DialogueManager.Instance?.ShowDialogue(
                "Patrol complete! You asked a good question, drew neat tally marks, and read the total chart like a pro. The sea toys are safe because of you!",
                "vo_summary",
                true
            );

            yield return new WaitForSeconds(1.2f);

            if (playAgainButton != null)
            {
                playAgainButton.gameObject.SetActive(true);
                playAgainButton.transform.localScale = Vector3.zero;
                playAgainButton.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.gameObject.SetActive(true);
                mainMenuButton.transform.localScale = Vector3.zero;
                mainMenuButton.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
            }
        }
    }
}
