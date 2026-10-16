using System;
using System.Collections.Generic;
using UnityEngine;

namespace Equipment.Progression
{
    [Serializable]
    public class EquipmentUnlockRule
    {
        public EquipmentDefinition Equipment;

        [Min(1)]
        public int MaxLevel = 1;

        [Min(1)]
        public int MaxInstances = 1;
    }

    [CreateAssetMenu(
        fileName = "RaftLevelDefinition",
        menuName = "Raft Empire/Raft/Level Definition")]
    public class RaftLevelDefinition : ScriptableObject
    {
        [Min(1)]
        public int Level = 1;

        [Header("Available Equipment")]
        public List<EquipmentUnlockRule> EquipmentRules = new();

        public int GetMaxEquipmentLevel(
            EquipmentDefinition equipment)
        {
            var rule = EquipmentRules.Find(
                x => x.Equipment == equipment);

            return rule != null ? rule.MaxLevel : 0;
        }

        public int GetMaxEquipmentInstances(
            EquipmentDefinition equipment)
        {
            var rule = EquipmentRules.Find(
                x => x.Equipment == equipment);

            return rule != null ? rule.MaxInstances : 0;
        }

        public bool IsEquipmentUnlocked(
            EquipmentDefinition equipment)
        {
            return GetMaxEquipmentInstances(equipment) > 0;
        }
    }
}