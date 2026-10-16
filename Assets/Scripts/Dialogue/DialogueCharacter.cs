using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [Serializable]
    public class DialoguePortrait
    {
        public DialogueExpression expression;
        public Sprite sprite;
    }

    [CreateAssetMenu(
        fileName = "NewDialogueCharacter",
        menuName = "Raft Empire/Dialogue Character"
    )]
    public class DialogueCharacter : ScriptableObject
    {
        [SerializeField] private string characterId;
        [SerializeField] private string displayName;
        [SerializeField] private List<DialoguePortrait> portraits = new();

        public string CharacterId => characterId;
        public string DisplayName => displayName;

        public Sprite GetPortrait(DialogueExpression expression)
        {
            foreach (var portrait in portraits)
            {
                if (portrait.expression == expression && portrait.sprite != null)
                    return portrait.sprite;
            }

            foreach (var portrait in portraits)
            {
                if (portrait.expression == DialogueExpression.Normal)
                    return portrait.sprite;
            }

            return null;
        }
    }
}