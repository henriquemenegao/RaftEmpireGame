using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueUI : MonoBehaviour, IPointerClickHandler
    {
        [Header("References")]
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Image characterPortrait;
        [SerializeField] private TMP_Text speakerName;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private GameObject continueIndicator;

        [Header("Portrait Animation")]
        [SerializeField] private float enterDuration = 0.4f;
        [SerializeField] private float exitDuration = 0.3f;
        [SerializeField] private float offscreenPadding = 50f;

        private RectTransform portraitRect;
        private Vector2 portraitRestPosition;
        private Coroutine animationCoroutine;

        private bool isClosing;

        private void Awake()
        {
            portraitRect = characterPortrait.rectTransform;

            // Guarda a posição configurada no Inspector.
            portraitRestPosition = portraitRect.anchoredPosition;

            dialoguePanel.SetActive(false);
            characterPortrait.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            dialogueManager.OnDialogueStarted += HandleDialogueStarted;
            dialogueManager.OnLineChanged += HandleLineChanged;
            dialogueManager.OnDialogueFinished += HandleDialogueFinished;
        }

        private void OnDisable()
        {
            dialogueManager.OnDialogueStarted -= HandleDialogueStarted;
            dialogueManager.OnLineChanged -= HandleLineChanged;
            dialogueManager.OnDialogueFinished -= HandleDialogueFinished;

            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
        }

        private void HandleDialogueStarted()
        {
            StopCurrentAnimation();

            isClosing = false;

            dialoguePanel.SetActive(true);
            characterPortrait.gameObject.SetActive(true);

            portraitRect.anchoredPosition = GetOffscreenPosition();

            animationCoroutine = StartCoroutine(
                AnimatePortrait(
                    portraitRestPosition,
                    enterDuration,
                    false
                )
            );
        }

        private void HandleLineChanged(
            DialogueCharacter character,
            DialogueLine line)
        {
            speakerName.text = character != null
                ? character.DisplayName
                : line.speakerId;

            dialogueText.text = line.text;

            characterPortrait.sprite = character != null
                ? character.GetPortrait(line.expression)
                : null;

            characterPortrait.enabled =
                characterPortrait.sprite != null;

            continueIndicator.SetActive(true);
        }

        private void HandleDialogueFinished()
        {
            StopCurrentAnimation();

            isClosing = true;

            animationCoroutine = StartCoroutine(
                AnimatePortrait(
                    GetOffscreenPosition(),
                    exitDuration,
                    true
                )
            );
        }

        private Vector2 GetOffscreenPosition()
        {
            // Como o pivot está em (0, 0), deslocar a largura
            // completa deixa o retrato fora da lateral esquerda.
            float distance = portraitRect.rect.width + offscreenPadding;

            return portraitRestPosition + Vector2.left * distance;
        }

        private IEnumerator AnimatePortrait(
            Vector2 target,
            float duration,
            bool hideWhenFinished)
        {
            Vector2 start = portraitRect.anchoredPosition;

            if (duration <= 0f)
            {
                portraitRect.anchoredPosition = target;
            }
            else
            {
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;

                    float t = Mathf.Clamp01(elapsed / duration);

                    // Smoothstep: acelera e desacelera suavemente.
                    float eased = t * t * (3f - 2f * t);

                    portraitRect.anchoredPosition =
                        Vector2.LerpUnclamped(start, target, eased);

                    yield return null;
                }

                portraitRect.anchoredPosition = target;
            }

            if (hideWhenFinished && isClosing)
            {
                dialoguePanel.SetActive(false);
                characterPortrait.gameObject.SetActive(false);

                portraitRect.anchoredPosition = portraitRestPosition;
                isClosing = false;
            }

            animationCoroutine = null;
        }

        private void StopCurrentAnimation()
        {
            if (animationCoroutine == null)
                return;

            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isClosing || !dialogueManager.IsPlaying)
                return;

            dialogueManager.Next();
        }
    }
}