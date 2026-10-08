using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class DraggableBubble : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum BubbleType { ToyQuestion_A, OceanQuestion_B }

        [SerializeField] private BubbleType bubbleType;
        public BubbleType Type => bubbleType;

        [SerializeField] private RectTransform dropTargetArea;
        [SerializeField] private Level1Controller levelController;

        private RectTransform rectTransform;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private Vector2 originalAnchoredPosition;
        private Vector3 originalScale = Vector3.one;
        private bool isLocked = false;
        private bool hasRecordedOriginalPos = false;
        private bool isDragging = false;

        private void EnsureInitialized()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            if (!hasRecordedOriginalPos && rectTransform != null)
            {
                originalAnchoredPosition = rectTransform.anchoredPosition;
                originalScale = transform.localScale;
                hasRecordedOriginalPos = true;
            }
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void Start()
        {
            EnsureInitialized();
            if (rectTransform != null)
            {
                // Gentle continuous floating
                rectTransform.DOAnchorPosY(originalAnchoredPosition.y + 6f, 1.8f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void SetDropTarget(RectTransform target, Level1Controller controller)
        {
            dropTargetArea = target;
            levelController = controller;
        }

        public void SetLocked(bool locked)
        {
            isLocked = locked;
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = !locked;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isLocked) return;
            transform.DOKill();
            transform.DOScale(originalScale * 1.05f, 0.12f);
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayBubble();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isLocked) return;
            if (!isDragging)
            {
                // Just released without dragging: gently return to normal scale, DO NOT SUBMIT
                transform.DOKill();
                transform.DOScale(originalScale, 0.15f);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isLocked) return;
            isDragging = true;
            transform.DOKill();
            rectTransform.DOKill();
            transform.DOScale(originalScale * 1.08f, 0.15f);
            canvasGroup.alpha = 0.90f;
            canvasGroup.blocksRaycasts = false;
            transform.SetAsLastSibling();

            if (levelController != null)
            {
                levelController.OnBubbleDragStateChanged(true);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isLocked || canvas == null) return;
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isLocked) return;
            isDragging = false;
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;

            if (levelController != null)
            {
                levelController.OnBubbleDragStateChanged(false);
            }

            // ONLY submit if dropped within the designated drop target area!
            if (IsOverTarget(eventData.position))
            {
                SubmitBubble();
            }
            else
            {
                ReturnToOrigin();
            }
        }

        // Tap/click fallback is deliberately disabled: drag only required for Level 1!
        public void TapToSubmit()
        {
            // Drag and drop is strictly required
        }

        private bool IsOverTarget(Vector2 screenPosition)
        {
            if (dropTargetArea != null && RectTransformUtility.RectangleContainsScreenPoint(dropTargetArea, screenPosition))
            {
                return true;
            }
            if (levelController != null && levelController.IsPointInDropZone(screenPosition))
            {
                return true;
            }
            return false;
        }

        private void SubmitBubble()
        {
            if (levelController != null)
            {
                if (dropTargetArea != null)
                {
                    // Snap cleanly into the center of the drop zone
                    rectTransform.DOMove(dropTargetArea.transform.position, 0.22f).SetEase(Ease.OutBack);
                }
                levelController.OnBubbleSelected(this);
            }
        }

        public void ReturnToOrigin(bool shake = false)
        {
            EnsureInitialized();
            if (rectTransform == null) return;
            rectTransform.DOKill();
            if (shake)
            {
                rectTransform.DOShakeAnchorPos(0.4f, 25f, 14, 90, false, true)
                    .OnComplete(() =>
                    {
                        if (rectTransform != null)
                        {
                            rectTransform.DOAnchorPos(originalAnchoredPosition, 0.35f).SetEase(Ease.OutBack);
                        }
                    });
            }
            else
            {
                rectTransform.DOAnchorPos(originalAnchoredPosition, 0.4f).SetEase(Ease.OutBack);
            }
            transform.DOScale((originalScale != Vector3.zero) ? originalScale : Vector3.one, 0.3f);
        }

        public void ResetPosition()
        {
            EnsureInitialized();
            if (rectTransform != null)
            {
                rectTransform.DOKill();
                if (hasRecordedOriginalPos)
                {
                    rectTransform.anchoredPosition = originalAnchoredPosition;
                }
            }
            transform.DOKill();
            transform.localScale = (originalScale != Vector3.zero) ? originalScale : Vector3.one;
            isLocked = false;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1.0f;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void OnDestroy()
        {
            rectTransform?.DOKill();
            transform.DOKill();
        }
    }
}
