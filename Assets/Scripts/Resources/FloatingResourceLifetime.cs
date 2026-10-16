using UnityEngine;

namespace Resources
{
    public class FloatingResourceLifetime : MonoBehaviour
    {
        [Header("Despawn Settings")]
        [SerializeField, Min(0.1f)]
        private float maxTravelDistance = 12f;

        private Vector3 spawnPosition;

        private void Start()
        {
            spawnPosition = transform.position;
        }

        public void Configure(float distance)
        {
            maxTravelDistance = Mathf.Max(0.1f, distance);
        }

        private void Update()
        {
            Vector3 currentPosition = transform.position;

            // Ignora a altura das ondas.
            Vector2 start = new Vector2(spawnPosition.x, spawnPosition.z);
            Vector2 current = new Vector2(currentPosition.x, currentPosition.z);

            if (Vector2.Distance(start, current) >= maxTravelDistance)
                Destroy(gameObject);
        }
    }
}