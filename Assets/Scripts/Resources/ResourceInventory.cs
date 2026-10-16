using System;
using System.Collections.Generic;
using Save;
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

        public List<ResourceSaveData> ExportSaveData()
        {
            var result = new List<ResourceSaveData>();

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                result.Add(new ResourceSaveData
                {
                    resourceId = type.ToString(),
                    amount = GetAmount(type)
                });
            }

            return result;
        }

        public void ImportSaveData(List<ResourceSaveData> savedResources)
        {
            resources.Clear();

            if (savedResources != null)
            {
                foreach (ResourceSaveData saved in savedResources)
                {
                    if (saved == null ||
                        !Enum.TryParse(saved.resourceId, true, out ResourceType type) ||
                        !Enum.IsDefined(typeof(ResourceType), type))
                    {
                        Debug.LogWarning(
                            $"[ResourceInventory] Unknown resource: {saved?.resourceId}"
                        );

                        continue;
                    }

                    resources[type] = Mathf.Max(0, saved.amount);
                }
            }

            // Atualiza todos os recursos, inclusive os que ficaram zerados.
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                OnResourceChanged?.Invoke(type, GetAmount(type));
            }
        }
    }
}