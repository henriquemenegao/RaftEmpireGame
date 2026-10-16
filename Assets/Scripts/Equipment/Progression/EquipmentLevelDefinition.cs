using System;
using System.Collections.Generic;
using UnityEngine;

namespace Equipment.Progression
{
    [Serializable]
    public class EquipmentLevelDefinition
    {
        [Min(1)]
        public int Level = 1;

        [Header("Visual")]
        public GameObject Prefab;

        [Header("Production")]
        [Min(1)]
        public int BaseProductionAmount = 1;

        [Min(0.1f)]
        public float ProductionDuration = 20f;

        [Header("Purchase / Upgrade")]
        public List<ResourceCost> Costs = new();
    }
}