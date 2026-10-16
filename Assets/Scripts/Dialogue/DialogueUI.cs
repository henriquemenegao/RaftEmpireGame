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

        [Header("Panel Animation")]
        [SerializeField] private CanvasGroup dialogueCanvasGroup;
        [SerializeField] private float panelFadeDuration = 0.25f;

        private RectTransform portraitRect;
        private Vector2 portraitRestPosition;

        private Coroutine animationCoroutine;
        private Coroutine panelFadeCoroutine;

        private bool isClosing;

        private void Awake()
        {
            portraitRect = characterPortrait.rectTransform;

            // Guarda a posição configurada no Inspector.
            portraitRestPosition = portraitRect.anchoredPosition;

            dialogueCanvasGroup.alpha = 0f;

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

            StopCurrentAnimation();
            StopPanelFade();
        }

        private void HandleDialogueStarted()
        {
            StopCurrentAnimation();
            StopPanelFade();

            isClosing = false;

            // Reinicia a transparência antes de abrir.
            dialogueCanvasGroup.alpha = 0f;

            dialoguePanel.SetActive(true);
            characterPortrait.gameObject.SetActive(true);

            // Fred começa fora da tela.
            portraitRect.anchoredPosition = GetOffscreenPosition();

            // Animações independentes.
            StartPanelFade(1f);

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

            // Fred desliza para fora.
            animationCoroutine = StartCoroutine(
                AnimatePortrait(
                    GetOffscreenPosition(),
                    exitDuration,
                    true
                )
            );

            // A faixa desaparece suavemente.
            StartPanelFade(0f);
        }

        private Vector2 GetOffscreenPosition()
        {
            float distance =
                portraitRect.rect.width + offscreenPadding;

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

                    // Aceleração e desaceleração suaves.
                    float eased = t * t * (3f - 2f * t);

                    portraitRect.anchoredPosition =
                        Vector2.LerpUnclamped(
                            start,
                            target,
                            eased
                        );

                    yield return null;
                }

                portraitRect.anchoredPosition = target;
            }

            if (hideWhenFinished && isClosing)
            {
                // Interrompe qualquer fade ainda em andamento.
                StopPanelFade();

                // Garante que o painel fique invisível.
                dialogueCanvasGroup.alpha = 0f;

                dialoguePanel.SetActive(false);
                characterPortrait.gameObject.SetActive(false);

                // Restaura a posição original para a próxima conversa.
                portraitRect.anchoredPosition = portraitRestPosition;

                isClosing = false;
            }

            animationCoroutine = null;
        }

        private void StartPanelFade(float targetAlpha)
        {
            StopPanelFade();

            panelFadeCoroutine = StartCoroutine(
                FadePanel(targetAlpha)
            );
        }

        private IEnumerator FadePanel(float targetAlpha)
        {
            float startAlpha = dialogueCanvasGroup.alpha;
            float elapsed = 0f;

            if (panelFadeDuration <= 0f)
            {
                dialogueCanvasGroup.alpha = targetAlpha;
                panelFadeCoroutine = null;
                yield break;
            }

            while (elapsed < panelFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(
                    elapsed / panelFadeDuration
                );

                dialogueCanvasGroup.alpha = Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

                yield return null;
            }

            dialogueCanvasGroup.alpha = targetAlpha;
            panelFadeCoroutine = null;
        }

        private void StopCurrentAnimation()
        {
            if (animationCoroutine == null)
                return;

            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        private void StopPanelFade()
        {
            if (panelFadeCoroutine == null)
                return;

            StopCoroutine(panelFadeCoroutine);
            panelFadeCoroutine = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isClosing || !dialogueManager.IsPlaying)
                return;

            dialogueManager.Next();
        }
    }
}