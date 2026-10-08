using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class Level1Controller : MonoBehaviour
    {
        [Header("Octopuses")]
        [SerializeField] private RectTransform octopusesContainer;
        [SerializeField] private List<Image> octopusImages = new List<Image>();
        [SerializeField] private List<Image> heldToyImages = new List<Image>();
        [SerializeField] private List<GameObject> yesBubbles = new List<GameObject>();

        [Header("Sprites")]
        [SerializeField] private Sprite redCarSprite;
        [SerializeField] private Sprite blueBoatSprite;
        [SerializeField] private Sprite greenPlaneSprite;

        [Header("Questions & Drop Area")]
        [SerializeField] private DraggableBubble bubbleA; // Which toy do you like best?
        [SerializeField] private DraggableBubble bubbleB; // Do you live in the ocean?
        [SerializeField] private RectTransform dropTargetArea;
        [SerializeField] private GameObject dropTargetVisual;
        [SerializeField] private TextMeshProUGUI dropTargetLabel;

        [Header("Results & Navigation")]
        [SerializeField] private RectTransform resultsCard;
        [SerializeField] private Button nextLevelButton;

        private bool levelCompleted = false;

        private void Start()
        {
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.AddListener(() =>
                {
                    DeepSeaGameManager.Instance.GoToLevel(2);
                });
                nextLevelButton.gameObject.SetActive(false);
            }

            if (bubbleA != null) bubbleA.SetDropTarget(dropTargetArea, this);
            if (bubbleB != null) bubbleB.SetDropTarget(dropTargetArea, this);

            InitOctopuses();
        }

        private void OnEnable()
        {
            ResetLevel();
        }

        public void ResetLevel()
        {
            levelCompleted = false;

            if (bubbleA != null)
            {
                bubbleA.gameObject.SetActive(true);
                bubbleA.ResetPosition();
            }
            if (bubbleB != null)
            {
                bubbleB.gameObject.SetActive(true);
                bubbleB.ResetPosition();
            }

            if (resultsCard != null) resultsCard.gameObject.SetActive(false);
            if (nextLevelButton != null) nextLevelButton.gameObject.SetActive(false);

            if (dropTargetVisual != null)
            {
                dropTargetVisual.SetActive(true);
                dropTargetVisual.transform.DOKill();
                dropTargetVisual.transform.localScale = Vector3.one;
                if (dropTargetLabel != null)
                {
                    dropTargetLabel.text = "DROP SELECTED QUESTION HERE";
                    dropTargetLabel.color = new Color(1f, 0.91f, 0.42f, 1f);
                }
            }

            for (int i = 0; i < heldToyImages.Count; i++)
            {
                if (heldToyImages[i] != null)
                {
                    heldToyImages[i].gameObject.SetActive(false);
                    heldToyImages[i].transform.localScale = Vector3.zero;
                }
            }

            for (int i = 0; i < yesBubbles.Count; i++)
            {
                if (yesBubbles[i] != null) yesBubbles[i].SetActive(false);
            }

            DialogueManager.Instance?.ShowDialogue(
                "We want to know which toy these five octopuses like best. Drag the question that will give us different answers we can count.",
                "vo_lvl1_intro",
                false
            );
        }

        public void OnBubbleDragStateChanged(bool isDragging)
        {
            if (levelCompleted || dropTargetVisual == null) return;
            dropTargetVisual.transform.DOKill();
            if (isDragging)
            {
                dropTargetVisual.transform.DOScale(Vector3.one * 1.05f, 0.35f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
                if (dropTargetLabel != null)
                {
                    dropTargetLabel.text = "RELEASE HERE TO TEST QUESTION!";
                    dropTargetLabel.color = new Color(1f, 0.96f, 0.45f, 1f);
                }
            }
            else
            {
                dropTargetVisual.transform.DOScale(Vector3.one, 0.22f);
                if (dropTargetLabel != null)
                {
                    dropTargetLabel.text = "DROP SELECTED QUESTION HERE";
                    dropTargetLabel.color = new Color(1f, 0.91f, 0.42f, 1f);
                }
            }
        }

        public bool IsPointInDropZone(Vector2 screenPos)
        {
            if (dropTargetArea != null && RectTransformUtility.RectangleContainsScreenPoint(dropTargetArea, screenPos))
            {
                return true;
            }
            if (octopusesContainer != null && RectTransformUtility.RectangleContainsScreenPoint(octopusesContainer, screenPos))
            {
                return true;
            }
            return false;
        }

        private void InitOctopuses()
        {
            for (int i = 0; i < octopusImages.Count; i++)
            {
                if (octopusImages[i] != null)
                {
                    float delay = i * 0.2f;
                    octopusImages[i].rectTransform.DOAnchorPosY(octopusImages[i].rectTransform.anchoredPosition.y + 12f, 1.5f + (i * 0.1f))
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetDelay(delay);
                }
            }
        }

        public void OnBubbleSelected(DraggableBubble bubble)
        {
            if (levelCompleted) return;

            if (bubble.Type == DraggableBubble.BubbleType.OceanQuestion_B)
            {
                // Incorrect: Bubble B
                StartCoroutine(HandleIncorrectChoice(bubble));
            }
            else
            {
                // Correct: Bubble A
                StartCoroutine(HandleCorrectChoice(bubble));
            }
        }

        private IEnumerator HandleIncorrectChoice(DraggableBubble bubble)
        {
            SoundManager.Instance?.PlayWrong();

            // All octopuses say YES
            for (int i = 0; i < yesBubbles.Count; i++)
            {
                if (yesBubbles[i] != null)
                {
                    yesBubbles[i].SetActive(true);
                    yesBubbles[i].transform.localScale = Vector3.zero;
                    yesBubbles[i].transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                }
            }

            DialogueManager.Instance?.ShowDialogue(
                "Oops! Every octopus lives in the ocean, so they would all answer 'yes.' That question can't be counted in different groups. Try the toy question instead.",
                "vo_lvl1_wrong",
                false
            );

            yield return new WaitForSeconds(1.8f);

            bubble.ReturnToOrigin(shake: true);

            for (int i = 0; i < yesBubbles.Count; i++)
            {
                if (yesBubbles[i] != null)
                {
                    yesBubbles[i].transform.DOScale(Vector3.zero, 0.25f)
                        .OnComplete(() => yesBubbles[i].SetActive(false));
                }
            }
        }

        private IEnumerator HandleCorrectChoice(DraggableBubble bubble)
        {
            levelCompleted = true;
            bubble.SetLocked(true);
            if (bubbleB != null) bubbleB.SetLocked(true);

            SoundManager.Instance?.PlayCorrect();

            // Octopuses cheerfully reveal their favorite toys:
            // 2 Red Cars, 2 Blue Boats, 1 Green Plane
            Sprite[] toySprites = new Sprite[]
            {
                redCarSprite,   // Octopus 0: Red Car
                redCarSprite,   // Octopus 1: Red Car
                blueBoatSprite, // Octopus 2: Blue Boat
                blueBoatSprite, // Octopus 3: Blue Boat
                greenPlaneSprite// Octopus 4: Green Plane
            };

            for (int i = 0; i < heldToyImages.Count && i < toySprites.Length; i++)
            {
                if (heldToyImages[i] != null)
                {
                    heldToyImages[i].sprite = toySprites[i];
                    heldToyImages[i].gameObject.SetActive(true);
                    heldToyImages[i].transform.localScale = Vector3.zero;
                    heldToyImages[i].transform.DOScale(Vector3.one, 0.45f).SetEase(Ease.OutBack).SetDelay(i * 0.15f);
                    SoundManager.Instance?.PlayBubble();
                    yield return new WaitForSeconds(0.18f);
                }
            }

            yield return new WaitForSeconds(0.4f);

            DialogueManager.Instance?.ShowDialogue(
                "Great choice! Every octopus can give a different answer. Now we can count how many like each toy. Let's remember this - we'll tally these votes very soon!",
                "vo_lvl1_correct",
                true
            );

            // Hide the drop slot to reveal results summary
            if (dropTargetVisual != null)
            {
                dropTargetVisual.transform.DOScale(Vector3.zero, 0.35f)
                    .OnComplete(() => dropTargetVisual.SetActive(false));
            }

            // Hide the question bubbles so the Continue button is cleanly visible
            if (bubbleA != null) bubbleA.gameObject.SetActive(false);
            if (bubbleB != null) bubbleB.gameObject.SetActive(false);

            // Pop up summary results card
            if (resultsCard != null)
            {
                resultsCard.gameObject.SetActive(true);
                resultsCard.localScale = Vector3.zero;
                resultsCard.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }

            yield return new WaitForSeconds(0.5f);

            if (nextLevelButton != null)
            {
                nextLevelButton.gameObject.SetActive(true);
                nextLevelButton.transform.DOKill();
                nextLevelButton.transform.localScale = Vector3.zero;
                nextLevelButton.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        // Slow, gentle breathing pulse between 1.0 and 1.05
                        nextLevelButton.transform.DOScale(Vector3.one * 1.05f, 1.25f)
                            .SetEase(Ease.InOutSine)
                            .SetLoops(-1, LoopType.Yoyo);
                    });
            }
        }
    }
}
