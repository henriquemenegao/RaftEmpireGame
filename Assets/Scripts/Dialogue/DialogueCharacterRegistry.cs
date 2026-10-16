using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(
        fileName = "DialogueCharacterRegistry",
        menuName = "Raft Empire/Dialogue Character Registry"
    )]
    public class DialogueCharacterRegistry : ScriptableObject
    {
        [SerializeField]
        private List<DialogueCharacter> characters = new();

        private Dictionary<string, DialogueCharacter> lookup;

        public DialogueCharacter GetCharacter(string characterId)
        {
            if (lookup == null)
                BuildLookup();

            if (string.IsNullOrWhiteSpace(characterId))
                return null;

            lookup.TryGetValue(characterId, out var character);
            return character;
        }

        private void BuildLookup()
        {
            lookup = new Dictionary<string, DialogueCharacter>(
                System.StringComparer.OrdinalIgnoreCase
            );

            foreach (var character in characters)
            {
                if (character == null)
                    continue;

                string id = character.CharacterId;

                if (string.IsNullOrWhiteSpace(id))
                {
                    Debug.LogWarning(
                        $"Dialogue character '{character.name}' has no CharacterId."
                    );
                    continue;
                }

                if (!lookup.TryAdd(id, character))
                {
                    Debug.LogWarning(
                        $"Duplicate dialogue character ID: '{id}'."
                    );
                }
            }
        }

        private void OnEnable()
        {
            lookup = null;
        }

        private void OnValidate()
        {
            lookup = null;
        }
    }
}