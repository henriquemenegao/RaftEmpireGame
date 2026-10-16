using UnityEngine;

namespace Dialogue
{
    public class DialogueTest : MonoBehaviour
    {
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private DialogueSequence sequence;

        private void Start()
        {
            dialogueManager.Play(sequence);
        }
    }
}