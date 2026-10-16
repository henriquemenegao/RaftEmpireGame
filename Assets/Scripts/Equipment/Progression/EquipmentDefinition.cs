using System.Collections.Generic;
using UnityEngine;

namespace Equipment.Progression
{
    [CreateAssetMenu(
        fileName = "EquipmentDefinition",
        menuName = "Raft Empire/Equipment/Definition"
    )]
    public class EquipmentDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string EquipmentId;
        public string DisplayName;

        [TextArea(2, 4)]
        public string Description;

        public Sprite Icon;

        [Header("Levels")]
        public List<EquipmentLevelDefinition> Levels = new();

        public EquipmentLevelDefinition GetLevel(int level)
        {
            return Levels.Find(x => x.Level == level);
        }

        public int MaxDefinedLevel
        {
            get
            {
                int max = 0;

                foreach (var definition in Levels)
                {
                    if (definition.Level > max)
                        max = definition.Level;
                }

                return max;
            }
        }
    }
}