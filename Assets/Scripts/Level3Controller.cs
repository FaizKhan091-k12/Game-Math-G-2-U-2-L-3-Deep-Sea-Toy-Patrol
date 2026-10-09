using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class Level3Controller : MonoBehaviour
    {
        public enum ToyRow { RedCars = 0, BlueBoats = 1, GreenPlanes = 2, None = -1 }

        [Header("Wheel UI")]
        [SerializeField] private RectTransform shipWheelTransform;
        [SerializeField] private RectTransform pointerNeedle;
        [SerializeField] private Button rotateClockwiseBtn;
        [SerializeField] private Button rotateCounterBtn;
        [SerializeField] private Button lockRowButton;

        [Header("Table Rows")]
        [SerializeField] private List<RectTransform> tableRows = new List<RectTransform>(); // 0: Red Cars, 1: Blue Boats, 2: Green Planes
        [SerializeField] private List<Button> rowSelectButtons = new List<Button>();
        [SerializeField] private List<Image> rowHighlightOverlays = new List<Image>();

        [Header("Question Banner")]
        [SerializeField] private TextMeshProUGUI questionBannerText;

        [Header("Navigation")]
        [SerializeField] private Button continueToSummaryButton;

        private int currentTask = 1; // 1: Biggest total (Red Cars: 8), 2: Smallest total (Blue Boats: 3)
        private ToyRow selectedRow = ToyRow.None;
        private bool isTransitioning = false;

        // Angle mapping for the wheel pointer:
        // Red Cars (top row): -25 degrees
        // Blue Boats (middle row): 0 degrees
        // Green Planes (bottom row): 25 degrees
        private readonly float[] rowAngles = new float[] { -28f, 0f, 28f };

        private void Start()
        {
            if (rotateClockwiseBtn != null)
            {
                rotateClockwiseBtn.onClick.AddListener(() => RotateWheelStep(1));
            }
            if (rotateCounterBtn != null)
            {
                rotateCounterBtn.onClick.AddListener(() => RotateWheelStep(-1));
            }

            if (lockRowButton != null)
            {
                lockRowButton.onClick.AddListener(ConfirmSelection);
            }

            for (int i = 0; i < rowSelectButtons.Count; i++)
            {
                int index = i;
                if (rowSelectButtons[i] != null)
                {
                    rowSelectButtons[i].onClick.AddListener(() =>
                    {
                        if (isTransitioning) return;
                        if (DialogueManager.Instance != null && DialogueManager.Instance.IsSpeaking) return;
                        SelectRow((ToyRow)index);
                        ConfirmSelection();
                    });
                }
            }

            if (continueToSummaryButton != null)
            {
                continueToSummaryButton.onClick.AddListener(() =>
                {
                    DeepSeaGameManager.Instance.GoToSummary();
                });
                continueToSummaryButton.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            ResetLevel();
        }

        public void ResetLevel()
        {
            currentTask = 1;
            selectedRow = ToyRow.BlueBoats; // Start at middle
            isTransitioning = false;

            if (continueToSummaryButton != null) continueToSummaryButton.gameObject.SetActive(false);

            ApplyRowSelection(selectedRow, animate: false);

            StartCoroutine(IntroSequence());
        }

        private IEnumerator IntroSequence()
        {
            if (questionBannerText != null)
            {
                questionBannerText.text = "Turn the wheel to point at the toy with the BIGGEST total votes!";
            }

            DialogueManager.Instance?.ShowDialogue(
                "Look! The Red Car total on this wall is 8 - the same number we just tallied together. The other two totals were counted by your ocean friends while you worked. Now let's read the whole chart!",
                "vo_lvl3_intro",
                true
            );

            // Wait until the intro is completely spoken before giving task 1 instruction
            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(DialogueManager.Instance.WaitForDialogueCompletion());
            }
            else
            {
                yield return new WaitForSeconds(3.5f);
            }

            DialogueManager.Instance?.ShowDialogue(
                "Look at the numbers on the wall. Turn the wheel to point at the toy with the biggest total.",
                "vo_lvl3_task1",
                false
            );
        }

        public void RotateWheelStep(int direction)
        {
            if (isTransitioning) return;

            int current = (int)selectedRow;
            int next = Mathf.Clamp(current + direction, 0, 2);
            SelectRow((ToyRow)next);
        }

        public void SelectRow(ToyRow row)
        {
            if (isTransitioning || row == ToyRow.None) return;
            selectedRow = row;
            ApplyRowSelection(row, animate: true);
            SoundManager.Instance?.PlayWheelTick();
        }

        private void ApplyRowSelection(ToyRow row, bool animate)
        {
            int idx = (int)row;
            float targetAngle = rowAngles[idx];

            if (shipWheelTransform != null)
            {
                shipWheelTransform.DOKill();
                float rotZ = (idx - 1) * -45f;
                if (animate)
                {
                    shipWheelTransform.DORotate(new Vector3(0f, 0f, rotZ), 0.35f).SetEase(Ease.OutBack);
                }
                else
                {
                    shipWheelTransform.localEulerAngles = new Vector3(0f, 0f, rotZ);
                }
            }

            if (pointerNeedle != null)
            {
                pointerNeedle.DOKill();
                if (animate)
                {
                    pointerNeedle.DORotate(new Vector3(0f, 0f, targetAngle), 0.35f).SetEase(Ease.OutBack);
                }
                else
                {
                    pointerNeedle.localEulerAngles = new Vector3(0f, 0f, targetAngle);
                }
            }

            // Highlights
            for (int i = 0; i < rowHighlightOverlays.Count; i++)
            {
                if (rowHighlightOverlays[i] != null)
                {
                    bool isActive = (i == idx);
                    rowHighlightOverlays[i].gameObject.SetActive(isActive);
                    if (isActive && animate)
                    {
                        rowHighlightOverlays[i].transform.DOPunchScale(Vector3.one * 0.05f, 0.25f, 4, 0.5f);
                    }
                }
            }
        }

        private void Update()
        {
            bool canInteract = !isTransitioning && (DialogueManager.Instance == null || !DialogueManager.Instance.IsSpeaking);
            if (lockRowButton != null && lockRowButton.interactable != canInteract)
            {
                lockRowButton.interactable = canInteract;
            }
        }

        public void ConfirmSelection()
        {
            if (isTransitioning) return;
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsSpeaking) return;

            if (currentTask == 1)
            {
                // Biggest total: Red Cars (8)
                if (selectedRow == ToyRow.RedCars)
                {
                    StartCoroutine(HandleTask1Success());
                }
                else
                {
                    StartCoroutine(HandleTask1Incorrect());
                }
            }
            else if (currentTask == 2)
            {
                // Smallest total: Blue Boats (3)
                if (selectedRow == ToyRow.BlueBoats)
                {
                    StartCoroutine(HandleTask2Success());
                }
                else
                {
                    StartCoroutine(HandleTask2Incorrect());
                }
            }
        }

        private IEnumerator HandleTask1Success()
        {
            isTransitioning = true;
            SoundManager.Instance?.PlayCorrect();

            tableRows[0]?.DOPunchScale(Vector3.one * 0.12f, 0.4f, 6, 0.5f);

            DialogueManager.Instance?.ShowDialogue(
                "You got it! Eight is the biggest number on the chart. Red cars were found the most!",
                "vo_lvl3_task1_correct",
                true
            );

            // Wait until speech is complete before advancing to task 2
            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(DialogueManager.Instance.WaitForDialogueCompletion());
            }
            else
            {
                yield return new WaitForSeconds(3.0f);
            }

            currentTask = 2;
            isTransitioning = false;

            if (questionBannerText != null)
            {
                questionBannerText.text = "Turn the wheel to point at the toy with the SMALLEST total votes!";
                questionBannerText.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 4, 0.5f);
            }

            DialogueManager.Instance?.ShowDialogue(
                "Now look at the chart again. Turn the wheel to point at the toy with the smallest total.",
                "vo_lvl3_task2",
                false
            );
        }

        private IEnumerator HandleTask1Incorrect()
        {
            isTransitioning = true;
            SoundManager.Instance?.PlayWrong();

            DialogueManager.Instance?.ShowDialogue(
                "Not quite. Check each number again - which one is the highest? Try turning the wheel to that row.",
                "vo_lvl3_task1_wrong",
                false
            );

            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(DialogueManager.Instance.WaitForDialogueCompletion());
            }
            else
            {
                yield return new WaitForSeconds(3.0f);
            }

            isTransitioning = false;
        }

        private IEnumerator HandleTask2Success()
        {
            isTransitioning = true;
            SoundManager.Instance?.PlayCorrect();

            tableRows[1]?.DOPunchScale(Vector3.one * 0.12f, 0.4f, 6, 0.5f);

            DialogueManager.Instance?.ShowDialogue(
                "Spot on! Three is the smallest number on the chart. Blue boats were found the least!",
                "vo_lvl3_task2_correct",
                true
            );

            if (questionBannerText != null)
            {
                questionBannerText.text = "Chart reading complete! You found the biggest and smallest totals!";
                questionBannerText.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 4, 0.5f);
            }

            // Wait until speech is complete before showing summary button
            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(DialogueManager.Instance.WaitForDialogueCompletion());
            }
            else
            {
                yield return new WaitForSeconds(3.0f);
            }

            if (continueToSummaryButton != null)
            {
                continueToSummaryButton.gameObject.SetActive(true);
                continueToSummaryButton.transform.DOKill();
                continueToSummaryButton.transform.localScale = Vector3.zero;
                continueToSummaryButton.transform.DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        continueToSummaryButton.transform.DOScale(Vector3.one * 1.05f, 1.25f)
                            .SetEase(Ease.InOutSine)
                            .SetLoops(-1, LoopType.Yoyo);
                    });
            }
        }

        private IEnumerator HandleTask2Incorrect()
        {
            isTransitioning = true;
            SoundManager.Instance?.PlayWrong();

            DialogueManager.Instance?.ShowDialogue(
                "Not yet. Look for the smallest number of all three - is it really this one? Try again.",
                "vo_lvl3_task2_wrong",
                false
            );

            if (DialogueManager.Instance != null)
            {
                yield return StartCoroutine(DialogueManager.Instance.WaitForDialogueCompletion());
            }
            else
            {
                yield return new WaitForSeconds(3.0f);
            }

            isTransitioning = false;
        }
    }
}
