using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace DeepSeaPatrol
{
    [RequireComponent(typeof(Button))]
    public class JuicyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Animation Settings")]
        [SerializeField] private float hoverScale = 1.06f;
        [SerializeField] private float punchScaleAmount = 0.15f;
        [SerializeField] private float animationDuration = 0.2f;
        [SerializeField] private bool playClickSound = true;
        [SerializeField] private bool continuousIdleBobbing = false;

        private Vector3 initialScale;
        private Tween currentTween;
        private Tween bobbingTween;

        private void Awake()
        {
            initialScale = transform.localScale;
        }

        private void OnEnable()
        {
            transform.localScale = initialScale;
            if (continuousIdleBobbing)
            {
                StartBobbing();
            }
        }

        private void OnDisable()
        {
            currentTween?.Kill();
            bobbingTween?.Kill();
            transform.localScale = initialScale;
        }

        private void StartBobbing()
        {
            bobbingTween?.Kill();
            bobbingTween = transform.DOScale(initialScale * 1.05f, 1.2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (continuousIdleBobbing) return;
            currentTween?.Kill();
            currentTween = transform.DOScale(initialScale * hoverScale, animationDuration).SetEase(Ease.OutQuad);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (continuousIdleBobbing) return;
            currentTween?.Kill();
            currentTween = transform.DOScale(initialScale, animationDuration).SetEase(Ease.OutQuad);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            currentTween?.Kill();
            transform.localScale = initialScale * 0.94f;
            if (playClickSound && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayClick();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            currentTween?.Kill();
            currentTween = transform.DOPunchScale(Vector3.one * punchScaleAmount, 0.3f, 8, 0.5f)
                .OnComplete(() =>
                {
                    if (!continuousIdleBobbing)
                    {
                        transform.localScale = initialScale;
                    }
                });
        }
    }
}
