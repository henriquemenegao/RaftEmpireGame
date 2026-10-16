using UnityEngine;

namespace Resources
{
    [RequireComponent(typeof(Collider))]
    public class FloatingResourceCollector : MonoBehaviour
    {
        [Header("Resource")]
        [SerializeField] private ResourceType resourceType = ResourceType.Wood;
        [SerializeField, Min(1)] private int amount = 1;

        [Header("References")]
        [SerializeField] private ResourceInventory inventory;

        private bool collected;

        private void Awake()
        {
            if (inventory == null)
                inventory = FindAnyObjectByType<ResourceInventory>();
        }

        private void OnMouseDown()
        {
            Collect();
        }

        private void Collect()
        {
            if (collected || inventory == null)
                return;

            collected = true;

            inventory.Add(resourceType, amount);

            Debug.Log($"Collected {amount} {resourceType}");

            Destroy(gameObject);
        }
    }
}