using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DeepSeaPatrol
{
    public class BubbleParticleSystem : MonoBehaviour
    {
        [SerializeField] private Sprite bubbleSprite;
        [SerializeField] private int bubblePoolCount = 18;
        [SerializeField] private RectTransform containerRect;
        [SerializeField] private float minSpawnInterval = 0.35f;
        [SerializeField] private float maxSpawnInterval = 0.85f;

        private List<Image> bubblePool = new List<Image>();

        private void Start()
        {
            if (containerRect == null)
            {
                containerRect = GetComponent<RectTransform>();
            }

            if (bubbleSprite == null)
            {
                bubbleSprite = Resources.Load<Sprite>("Art/Sprites/bubble_particle");
            }

            InitPool();
            StartCoroutine(SpawnRoutine());
        }

        private void InitPool()
        {
            for (int i = 0; i < bubblePoolCount; i++)
            {
                GameObject obj = new GameObject($"Bubble_{i}");
                obj.transform.SetParent(containerRect, false);
                Image img = obj.AddComponent<Image>();
                img.sprite = bubbleSprite;
                img.raycastTarget = false;
                img.color = new Color(1f, 1f, 1f, 0f);
                obj.SetActive(false);
                bubblePool.Add(img);
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
                SpawnBubble();
            }
        }

        private void SpawnBubble()
        {
            Image bubble = bubblePool.Find(b => !b.gameObject.activeSelf);
            if (bubble == null) return;

            RectTransform rect = bubble.rectTransform;
            bubble.gameObject.SetActive(true);

            float width = containerRect.rect.width > 0 ? containerRect.rect.width : 1920f;
            float height = containerRect.rect.height > 0 ? containerRect.rect.height : 1080f;

            float startX = Random.Range(-width * 0.48f, width * 0.48f);
            float startY = -height * 0.52f;
            float endY = height * 0.55f;

            float size = Random.Range(20f, 60f);
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = new Vector2(startX, startY);

            float duration = Random.Range(4.5f, 7.5f);
            float driftAmount = Random.Range(25f, 50f);

            bubble.color = new Color(1f, 1f, 1f, Random.Range(0.45f, 0.85f));

            // Float up and sway side to side
            rect.DOAnchorPosY(endY, duration).SetEase(Ease.Linear);
            rect.DOAnchorPosX(startX + driftAmount, duration * 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(2, LoopType.Yoyo);

            // Pop / fade near top
            bubble.DOFade(0f, 0.6f)
                .SetDelay(duration - 0.6f)
                .OnComplete(() =>
                {
                    bubble.gameObject.SetActive(false);
                });
        }
    }
}
