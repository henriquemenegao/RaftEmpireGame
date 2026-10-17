using System.Collections.Generic;
using Resources;
using UnityEngine;

namespace Equipment.Progression
{
    public class EquipmentPurchaseService : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ResourceInventory resourceInventory;

        public bool CanAfford(EquipmentDefinition definition)
        {
            if (!TryGetCosts(definition, out var costs))
                return false;

            return resourceInventory.CanSpendBatch(costs);
        }

        public bool TryPay(EquipmentDefinition definition)
        {
            if (!TryGetCosts(definition, out var costs))
                return false;

            return resourceInventory.TrySpendBatch(costs);
        }

        private bool TryGetCosts(
            EquipmentDefinition definition,
            out Dictionary<ResourceType, int> costs)
        {
            costs = new Dictionary<ResourceType, int>();

            if (definition == null || resourceInventory == null)
                return false;

            var level = definition.GetLevel(1);

            if (level == null || level.Costs == null)
                return false;

            foreach (var cost in level.Costs)
            {
                if (cost == null || cost.Amount <= 0)
                    return false;

                if (!costs.ContainsKey(cost.Resource))
                    costs[cost.Resource] = 0;

                long total = (long)costs[cost.Resource] + cost.Amount;

                if (total > int.MaxValue)
                    return false;

                costs[cost.Resource] = (int)total;
            }

            return true;
        }
    }
}