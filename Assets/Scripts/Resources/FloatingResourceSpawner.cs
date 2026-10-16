
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
    public class FloatingResourceSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnEntry
        {
            public string name;
            public GameObject prefab;
            public bool enabled = true;

            [Min(0f)]
            public float weight = 1f;
        }

        [Header("Available Resources")]
        [SerializeField] private List<SpawnEntry> resources = new();

        [Header("Spawn Frequency")]
        [SerializeField, Min(0.1f)] private float minInterval = 5f;
        [SerializeField, Min(0.1f)] private float maxInterval = 10f;
        [SerializeField, Min(1)] private int maxActiveResources = 12;

        [Header("Spawn Area")]
        [SerializeField] private Transform spawnCenter;
        [SerializeField] private Vector2 spawnAreaSize = new Vector2(7f, 2f);

        [Header("Current")]
        [SerializeField] private Vector2 currentDirection =
            new Vector2(0.73f, -15f);

        [SerializeField] private Vector2 speedRange =
            new Vector2(0.2f, 0.4f);

        [Header("Lifetime")]
        [SerializeField, Min(1f)] private float maxLifetime = 60f;
        
        [SerializeField, Min(0.1f)]
        private float maxTravelDistance = 12f;

        private readonly List<GameObject> activeResources = new();

        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                float interval = Random.Range(
                    Mathf.Min(minInterval, maxInterval),
                    Mathf.Max(minInterval, maxInterval)
                );

                yield return new WaitForSeconds(interval);

                activeResources.RemoveAll(resource => resource == null);

                if (activeResources.Count >= maxActiveResources)
                    continue;

                Spawn();
            }
        }

        private void Spawn()
        {
            GameObject prefab = ChoosePrefab();

            if (prefab == null)
                return;

            Vector3 center = spawnCenter != null
                ? spawnCenter.position
                : transform.position;

            Vector3 position = center + new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                0f,
                Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f)
            );

            GameObject instance = Instantiate(
                prefab,
                position,
                Quaternion.identity
            );

            FloatingResourceDrift drift =
                instance.GetComponent<FloatingResourceDrift>();

            if (drift != null)
            {
                drift.Configure(
                    currentDirection,
                    Random.Range(
                        Mathf.Min(speedRange.x, speedRange.y),
                        Mathf.Max(speedRange.x, speedRange.y)
                    )
                );
            }

            activeResources.Add(instance);
            
            FloatingResourceLifetime lifetime =
                instance.GetComponent<FloatingResourceLifetime>();

            if (lifetime == null)
                lifetime = instance.AddComponent<FloatingResourceLifetime>();

            lifetime.Configure(maxTravelDistance);

            Destroy(instance, maxLifetime);
        }

        private GameObject ChoosePrefab()
        {
            float totalWeight = 0f;

            foreach (SpawnEntry entry in resources)
            {
                if (entry.enabled && entry.prefab != null && entry.weight > 0f)
                    totalWeight += entry.weight;
            }

            if (totalWeight <= 0f)
                return null;

            float roll = Random.Range(0f, totalWeight);

            foreach (SpawnEntry entry in resources)
            {
                if (!entry.enabled || entry.prefab == null || entry.weight <= 0f)
                    continue;

                roll -= entry.weight;

                if (roll <= 0f)
                    return entry.prefab;
            }

            return null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            Vector3 center = spawnCenter != null
                ? spawnCenter.position
                : transform.position;

            Gizmos.DrawWireCube(
                center,
                new Vector3(spawnAreaSize.x, 0.1f, spawnAreaSize.y)
            );
        }
    }
}
