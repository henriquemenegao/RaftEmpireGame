using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum DialogueExpression
    {
        Normal,
        Confused,
        Scared,
        Happy,
        Thinking,
        Determined
    }

    [Serializable]
    public class DialogueLine
    {
        public string speakerId;

        public DialogueExpression expression;

        [TextArea(2, 5)]
        public string text;
    }

    [CreateAssetMenu(
        fileName = "NewDialogueSequence",
        menuName = "Raft Empire/Dialogue Sequence"
    )]
    public class DialogueSequence : ScriptableObject
    {
        [SerializeField] private string dialogueId;
        [SerializeField] private List<DialogueLine> lines = new();
        public string DialogueId => dialogueId;
        public IReadOnlyList<DialogueLine> Lines => lines;
    }
}