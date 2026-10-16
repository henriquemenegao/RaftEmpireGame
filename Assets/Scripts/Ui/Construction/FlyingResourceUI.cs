
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Construction
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class FlyingResourceUI : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Image image;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();

            image.raycastTarget = false;
        }

        public void Play(
            Sprite sprite,
            Vector2 startPosition,
            System.Func<Vector2> getTargetPosition,
            float duration,
            float arcHeight,
            float scale,
            System.Action onComplete = null)
        {
            image.sprite = sprite;
            image.preserveAspect = true;

            rectTransform.anchoredPosition = startPosition;
            rectTransform.localScale = Vector3.one * scale;

            StartCoroutine(Animate(
                getTargetPosition,
                duration,
                arcHeight,
                onComplete));
        }

        private IEnumerator Animate(
            System.Func<Vector2> getTargetPosition,
            float duration,
            float arcHeight,
            System.Action onComplete)
        {
            Vector2 start = rectTransform.anchoredPosition;
            float elapsed = 0f;

            duration = Mathf.Max(0.01f, duration);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float eased = t * t * (3f - 2f * t);

                Vector2 target = getTargetPosition();

                Vector2 position = Vector2.Lerp(
                    start,
                    target,
                    eased);

                position.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

                rectTransform.anchoredPosition = position;

                yield return null;
            }

            rectTransform.anchoredPosition = getTargetPosition();

            onComplete?.Invoke();

            Destroy(gameObject);
        }
    }
}
