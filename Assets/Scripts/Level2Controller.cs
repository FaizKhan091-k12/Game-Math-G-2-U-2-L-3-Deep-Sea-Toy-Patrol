using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class Level2Controller : MonoBehaviour
    {
        [Header("Tally Marks on Stone")]
        [SerializeField] private List<Image> straightMarks = new List<Image>(); // 7 straight marks (4 in first group, 3 in second)
        [SerializeField] private Image diagonalSlashMark; // 5th mark diagonal crossing
        [SerializeField] private RectTransform stoneTablet;

        [Header("Fish Swimming Queue")]
        [SerializeField] private List<RectTransform> fishQueue = new List<RectTransform>();
        [SerializeField] private Transform fishSpawnPoint;
        [SerializeField] private Transform fishRestPoint;

        [Header("Laser Tool")]
        [SerializeField] private Button laserButton;
        [SerializeField] private Image laserBeamEffect;
        [SerializeField] private RectTransform laserEmitterTip;

        [Header("Chart Display")]
        [SerializeField] private TextMeshProUGUI chartCountText;
        [SerializeField] private RectTransform chartRow;

        [Header("Question Banner")]
        [SerializeField] private TextMeshProUGUI questionBannerText;

        [Header("Navigation")]
        [SerializeField] private Button nextLevelButton;

        private int currentTallyCount = 0;
        private int currentTask = 1; // 1: Marks 1-4, 2: Mark 5, 3: Marks 6-8
        private bool isProcessingTap = false;

        private void Start()
        {
            if (laserButton != null)
            {
                laserButton.onClick.AddListener(OnLaserTapped);
            }

            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.AddListener(() =>
                {
                    DeepSeaGameManager.Instance.GoToLevel(3);
                });
                nextLevelButton.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            ResetLevel();
        }

        public void ResetLevel()
        {
            currentTallyCount = 0;
            currentTask = 1;
            isProcessingTap = false;

            if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);
            if (laserButton != null) laserButton.interactable = true;

            // Hide all marks
            for (int i = 0; i < straightMarks.Count; i++)
            {
                if (straightMarks[i] != null)
                {
                    straightMarks[i].gameObject.SetActive(false);
                    straightMarks[i].transform.localScale = Vector3.zero;
                }
            }

            if (diagonalSlashMark != null)
            {
                diagonalSlashMark.gameObject.SetActive(false);
                diagonalSlashMark.transform.localScale = Vector3.zero;
            }

            if (laserBeamEffect != null)
            {
                laserBeamEffect.gameObject.SetActive(false);
            }

            if (chartCountText != null)
            {
                chartCountText.text = "?";
            }

            // Animate initial fish queue for Task 1
            StartCoroutine(SetupTask1());
        }

        private IEnumerator SetupTask1()
        {
            if (questionBannerText != null)
            {
                questionBannerText.text = "Tap your laser once for each fish to draw four tally marks!";
            }

            DialogueManager.Instance?.ShowDialogue(
                "More fish have joined the octopuses and voted for the Red Car! Four fish picked the Red Car. Tap your laser once for each fish to draw four tally marks.",
                "vo_lvl2_task1",
                false
            );

            // Animate 4 fish into position
            for (int i = 0; i < 4 && i < fishQueue.Count; i++)
            {
                if (fishQueue[i] != null)
                {
                    fishQueue[i].gameObject.SetActive(true);
                    fishQueue[i].localScale = Vector3.one;
                    fishQueue[i].DOPunchPosition(new Vector3(0f, 15f, 0f), 0.8f, 3, 0.5f).SetDelay(i * 0.15f);
                }
            }

            yield return null;
        }

        public void OnLaserTapped()
        {
            if (isProcessingTap) return;

            if (currentTask == 1)
            {
                // Task 1: Drawing marks 1 to 4
                if (currentTallyCount < 4)
                {
                    StartCoroutine(FireLaserForStraightMark(currentTallyCount));
                }
            }
            else if (currentTask == 2)
            {
                // Task 2: 5th diagonal closing mark
                if (currentTallyCount == 4)
                {
                    StartCoroutine(FireLaserForBundleSlash());
                }
            }
            else if (currentTask == 3)
            {
                // Task 3: Drawing marks 6 to 8
                if (currentTallyCount >= 5 && currentTallyCount < 8)
                {
                    StartCoroutine(FireLaserForStraightMark(currentTallyCount - 1)); // 5th straight mark array index
                }
            }
        }

        private IEnumerator FireLaserForStraightMark(int straightIndex)
        {
            isProcessingTap = true;
            currentTallyCount++;

            // Laser beam animation
            ShootLaserBeam();
            SoundManager.Instance?.PlayLaser();

            yield return new WaitForSeconds(0.12f);

            // Tally mark appears on stone with impact
            SoundManager.Instance?.PlayTallyHit();
            if (straightIndex >= 0 && straightIndex < straightMarks.Count && straightMarks[straightIndex] != null)
            {
                Image mark = straightMarks[straightIndex];
                mark.gameObject.SetActive(true);
                mark.transform.localScale = new Vector3(0.1f, 1.3f, 1f);
                mark.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
                stoneTablet?.DOShakePosition(0.2f, 8f, 10, 90, false, true);
            }

            // Fish bob reaction
            int fishIndex = currentTallyCount - 1;
            if (fishIndex >= 0 && fishIndex < fishQueue.Count && fishQueue[fishIndex] != null)
            {
                fishQueue[fishIndex].DOPunchScale(Vector3.one * 0.25f, 0.35f, 6, 0.5f);
            }

            yield return new WaitForSeconds(0.25f);
            isProcessingTap = false;

            // Check progression
            if (currentTask == 1 && currentTallyCount == 4)
            {
                StartCoroutine(CompleteTask1());
            }
            else if (currentTask == 3 && currentTallyCount == 8)
            {
                StartCoroutine(CompleteTask3());
            }
        }

        private IEnumerator CompleteTask1()
        {
            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance?.PlayCorrect();

            DialogueManager.Instance?.ShowDialogue(
                "Perfect! One tally mark means one vote. We have counted four votes for the Red Car so far.",
                "vo_lvl2_task1_correct",
                true
            );

            // Wait until dialogue is completely spoken before advancing gameplay
            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(DialogueManager.Instance.WaitForDialogueCompletion());
            }
            else
            {
                yield return new WaitForSeconds(3.5f);
            }

            // Advance to Task 2: 5th fish arrives
            currentTask = 2;
            if (questionBannerText != null)
            {
                questionBannerText.text = "Draw a line across the four marks to close the group of 5!";
                questionBannerText.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 4, 0.5f);
            }

            if (fishQueue.Count > 4 && fishQueue[4] != null)
            {
                fishQueue[4].gameObject.SetActive(true);
                fishQueue[4].localScale = Vector3.zero;
                fishQueue[4].DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
                SoundManager.Instance?.PlayBubble();
            }

            DialogueManager.Instance?.ShowDialogue(
                "One more fish picked the Red Car! That makes five. Tap once to draw a line across the first four marks and close the group of five.",
                "vo_lvl2_task2",
                false
            );
        }

        private IEnumerator FireLaserForBundleSlash()
        {
            isProcessingTap = true;
            currentTallyCount = 5;

            ShootLaserBeam();
            SoundManager.Instance?.PlayLaser();

            yield return new WaitForSeconds(0.12f);

            // Diagonal slash appears
            SoundManager.Instance?.PlayBundleSlash();
            if (diagonalSlashMark != null)
            {
                diagonalSlashMark.gameObject.SetActive(true);
                diagonalSlashMark.transform.localScale = Vector3.zero;
                diagonalSlashMark.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
                stoneTablet?.DOShakePosition(0.3f, 14f, 14, 90, false, true);
            }

            // 5th fish celebrates
            if (fishQueue.Count > 4 && fishQueue[4] != null)
            {
                fishQueue[4].DOPunchScale(Vector3.one * 0.35f, 0.4f, 8, 0.5f);
            }

            yield return new WaitForSeconds(0.35f);
            isProcessingTap = false;

            StartCoroutine(CompleteTask2());
        }

        private IEnumerator CompleteTask2()
        {
            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance?.PlayCorrect();

            DialogueManager.Instance?.ShowDialogue(
                "Awesome! Every fifth vote gets a line across the first four. Now we have one neat group of five. Groups of five make counting super fast!",
                "vo_lvl2_task2_correct",
                true
            );

            // Wait until dialogue is completely spoken before advancing gameplay
            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(DialogueManager.Instance.WaitForDialogueCompletion());
            }
            else
            {
                yield return new WaitForSeconds(3.5f);
            }

            // Advance to Task 3: 3 more fish arrive
            currentTask = 3;
            if (questionBannerText != null)
            {
                questionBannerText.text = "Draw 3 more tally marks to finish counting all 8 fish votes!";
                questionBannerText.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 4, 0.5f);
            }

            for (int i = 5; i < 8 && i < fishQueue.Count; i++)
            {
                if (fishQueue[i] != null)
                {
                    fishQueue[i].gameObject.SetActive(true);
                    fishQueue[i].localScale = Vector3.zero;
                    fishQueue[i].DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetDelay((i - 5) * 0.2f);
                }
            }

            DialogueManager.Instance?.ShowDialogue(
                "Three more fish picked the Red Car. Tap your laser once for each fish to add three more tally marks after the group of five.",
                "vo_lvl2_task3",
                false
            );
        }

        private IEnumerator CompleteTask3()
        {
            yield return new WaitForSeconds(0.3f);
            SoundManager.Instance?.PlayCorrect();

            DialogueManager.Instance?.ShowDialogue(
                "Great counting! Five marks in one group, plus three more marks, makes eight tally marks in total. So eight sea creatures picked the Red Car. Let's write that number on our chart!",
                "vo_lvl2_task3_correct",
                true
            );

            if (questionBannerText != null)
            {
                questionBannerText.text = "Count complete! Five plus three equals eight votes for the Red Car!";
                questionBannerText.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 4, 0.5f);
            }

            // Wait until dialogue is completely spoken before showing chart count
            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(DialogueManager.Instance.WaitForDialogueCompletion());
            }
            else
            {
                yield return new WaitForSeconds(3.5f);
            }

            // Chart fills in automatically
            if (chartCountText != null)
            {
                chartCountText.text = "8";
                chartCountText.transform.localScale = Vector3.zero;
                chartCountText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }

            if (chartRow != null)
            {
                chartRow.DOPunchScale(Vector3.one * 0.15f, 0.45f, 6, 0.5f);
            }

            SoundManager.Instance?.PlayStarPress();

            yield return new WaitForSeconds(0.8f);

            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(true);
                nextLevelButton.transform.DOKill();
                nextLevelButton.transform.localScale = Vector3.zero;
                nextLevelButton.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        nextLevelButton.transform.DOScale(Vector3.one * 1.05f, 1.25f)
                            .SetEase(Ease.InOutSine)
                            .SetLoops(-1, LoopType.Yoyo);
                    });
            }
        }

        private void ShootLaserBeam()
        {
            if (laserBeamEffect == null) return;
            laserBeamEffect.gameObject.SetActive(true);
            laserBeamEffect.color = new Color(1f, 1f, 1f, 1f);
            laserBeamEffect.transform.localScale = new Vector3(0.5f, 1.8f, 1f);
            laserBeamEffect.transform.DOScale(new Vector3(1.2f, 1f, 1f), 0.12f);
            laserBeamEffect.DOFade(0f, 0.18f).OnComplete(() =>
            {
                laserBeamEffect.gameObject.SetActive(false);
            });
        }
    }
}
