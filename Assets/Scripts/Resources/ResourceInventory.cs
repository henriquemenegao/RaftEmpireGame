
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

            int current = GetAmount(type);

            if (current > int.MaxValue - amount)
            {
                Debug.LogWarning(
                    $"[ResourceInventory] Resource overflow: {type}");
                return;
            }

            resources[type] = current + amount;

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

        public bool CanSpendBatch(
            IReadOnlyDictionary<ResourceType, int> costs)
        {
            if (costs == null)
                return false;

            foreach (var cost in costs)
            {
                if (cost.Value <= 0)
                    return false;

                if (GetAmount(cost.Key) < cost.Value)
                    return false;
            }

            return true;
        }

        public bool TrySpendBatch(
            IReadOnlyDictionary<ResourceType, int> costs)
        {
            // Valida todos os custos antes de descontar qualquer recurso.
            if (!CanSpendBatch(costs))
                return false;

            foreach (var cost in costs)
            {
                resources[cost.Key] =
                    GetAmount(cost.Key) - cost.Value;
            }

            // Notifica a HUD somente após concluir o pagamento.
            foreach (var cost in costs)
            {
                OnResourceChanged?.Invoke(
                    cost.Key,
                    resources[cost.Key]);
            }

            return true;
        }

        public List<ResourceSaveData> ExportSaveData()
        {
            var result = new List<ResourceSaveData>();

            foreach (ResourceType type in Enum.GetValues(
                         typeof(ResourceType)))
            {
                result.Add(new ResourceSaveData
                {
                    resourceId = type.ToString(),
                    amount = GetAmount(type)
                });
            }

            return result;
        }

        public void ImportSaveData(
            List<ResourceSaveData> savedResources)
        {
            resources.Clear();

            if (savedResources != null)
            {
                foreach (ResourceSaveData saved in savedResources)
                {
                    if (saved == null ||
                        !Enum.TryParse(
                            saved.resourceId,
                            true,
                            out ResourceType type) ||
                        !Enum.IsDefined(typeof(ResourceType), type))
                    {
                        Debug.LogWarning(
                            $"[ResourceInventory] Unknown resource: " +
                            $"{saved?.resourceId}");

                        continue;
                    }

                    resources[type] = Mathf.Max(0, saved.amount);
                }
            }

            // Atualiza todos os recursos, inclusive os zerados.
            foreach (ResourceType type in Enum.GetValues(
                         typeof(ResourceType)))
            {
                OnResourceChanged?.Invoke(
                    type,
                    GetAmount(type));
            }
        }
    }
}
