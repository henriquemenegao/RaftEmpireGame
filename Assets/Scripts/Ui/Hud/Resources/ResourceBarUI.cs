using System;
using System.Collections.Generic;
using UnityEngine;
using Resources;

namespace UI.Hud.Resources
{
    public class ResourceBarUI : MonoBehaviour
    {
        [Serializable]
        private class ResourceDisplay
        {
            public ResourceType type;
            public Sprite icon;
        }

        [Header("References")] [SerializeField]
        private ResourceInventory inventory;

        [SerializeField] private ResourceItemUI resourceItemPrefab;

        [Header("Displayed Resources")] [SerializeField]
        private List<ResourceDisplay> displayedResources = new();

        private readonly Dictionary<ResourceType, ResourceItemUI> items = new();


        public bool TryGetResourceItem(
            ResourceType type,
            out ResourceItemUI item)
        {
            return items.TryGetValue(type, out item);
        }


        private void Awake()
        {
            if (inventory == null)
                inventory = FindAnyObjectByType<ResourceInventory>();

            if (inventory == null || resourceItemPrefab == null)
            {
                Debug.LogError(
                    "[ResourceBarUI] Inventory or ResourceItem prefab not configured.",
                    this
                );
                enabled = false;
                return;
            }

            foreach (ResourceDisplay resource in displayedResources)
            {
                if (items.ContainsKey(resource.type))
                    continue;

                ResourceItemUI item = Instantiate(
                    resourceItemPrefab,
                    transform
                );

                item.SetResource(
                    resource.icon,
                    inventory.GetAmount(resource.type)
                );

                items.Add(resource.type, item);
            }
        }

        private void OnEnable()
        {
            if (inventory != null)
                inventory.OnResourceChanged += HandleResourceChanged;
        }

        private void OnDisable()
        {
            if (inventory != null)
                inventory.OnResourceChanged -= HandleResourceChanged;
        }

        private void HandleResourceChanged(ResourceType type, int amount)
        {
            if (items.TryGetValue(type, out ResourceItemUI item))
                item.SetAmount(amount);
        }
    }
}