using System;
using UnityEngine;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        private DialogueCharacterRegistry characterRegistry;

        private DialogueSequence currentSequence;
        private int currentLineIndex;
        private bool isPlaying;

        public bool IsPlaying => isPlaying;

        public event Action<DialogueCharacter, DialogueLine> OnLineChanged;
        public event Action OnDialogueStarted;
        public event Action OnDialogueFinished;

        public void Play(DialogueSequence sequence)
        {
            if (sequence == null)
            {
                Debug.LogWarning("DialogueManager: sequence is null.");
                return;
            }

            if (isPlaying)
            {
                Debug.LogWarning(
                    "DialogueManager: a dialogue is already playing."
                );
                return;
            }

            if (sequence.Lines == null || sequence.Lines.Count == 0)
            {
                Debug.LogWarning(
                    $"DialogueManager: sequence '{sequence.name}' has no lines."
                );
                return;
            }

            currentSequence = sequence;
            currentLineIndex = 0;
            isPlaying = true;

            OnDialogueStarted?.Invoke();

            ShowCurrentLine();
        }

        public void Next()
        {
            if (!isPlaying)
                return;

            currentLineIndex++;

            if (currentLineIndex >= currentSequence.Lines.Count)
            {
                Finish();
                return;
            }

            ShowCurrentLine();
        }

        private void ShowCurrentLine()
        {
            DialogueLine line = currentSequence.Lines[currentLineIndex];

            DialogueCharacter character = null;

            if (characterRegistry != null)
            {
                character = characterRegistry.GetCharacter(
                    line.speakerId
                );
            }

            if (character == null)
            {
                Debug.LogWarning(
                    $"DialogueManager: character '{line.speakerId}' not found."
                );
            }

            Debug.Log(
                $"[Dialogue] {character?.DisplayName ?? line.speakerId}: {line.text}"
            );

            OnLineChanged?.Invoke(character, line);
        }

        private void Finish()
        {
            isPlaying = false;
            currentSequence = null;
            currentLineIndex = 0;

            Debug.Log("[Dialogue] Finished.");

            OnDialogueFinished?.Invoke();
        }
    }
}