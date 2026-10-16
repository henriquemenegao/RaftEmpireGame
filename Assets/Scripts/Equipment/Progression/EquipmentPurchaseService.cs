using Equipment.Progression;
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
            if (definition == null || resourceInventory == null)
                return false;

            var level = definition.GetLevel(1);

            if (level == null || level.Costs == null)
                return false;

            foreach (var cost in level.Costs)
            {
                if (cost == null || cost.Amount <= 0)
                    return false;

                if (resourceInventory.GetAmount(cost.Resource) < cost.Amount)
                    return false;
            }

            return true;
        }

        public bool TryPay(EquipmentDefinition definition)
        {
            if (!CanAfford(definition))
                return false;

            var level = definition.GetLevel(1);

            foreach (var cost in level.Costs)
            {
                resourceInventory.TrySpend(cost.Resource, cost.Amount);
            }

            return true;
        }
    }
}