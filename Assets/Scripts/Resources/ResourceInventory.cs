using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
    public class ResourceInventory : MonoBehaviour
    {
        private readonly Dictionary<ResourceType, int> resources = new();

        public event Action<ResourceType, int> OnResourceChanged;

        public int GetAmount(ResourceType type)
        {
            return resources.GetValueOrDefault(type, 0);
        }

        public void Add(ResourceType type, int amount)
        {
            if (amount <= 0)
                return;

            resources[type] = GetAmount(type) + amount;

            OnResourceChanged?.Invoke(type, resources[type]);
        }

        public bool TrySpend(ResourceType type, int amount)
        {
            if (amount < 0 || GetAmount(type) < amount)
                return false;

            resources[type] = GetAmount(type) - amount;

            OnResourceChanged?.Invoke(type, resources[type]);

            return true;
        }
    }
}