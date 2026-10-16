using UnityEngine;

namespace Resources
{
    public class FloatingResourceDrift : MonoBehaviour
    {
        [Header("Current Settings")]
        [SerializeField] private Vector2 direction = new Vector2(0.7f, -15f);
        [SerializeField, Min(0f)] private float speed = 0.3f;

        private void Update()
        {
            if (direction.sqrMagnitude < 0.0001f)
                return;

            Vector2 movement = direction.normalized * (speed * Time.deltaTime);

            transform.position += new Vector3(
                movement.x,
                0f,
                movement.y
            );
        }
        
        
        public void Configure(Vector2 newDirection, float newSpeed)
        {
            direction = newDirection;
            speed = Mathf.Max(0f, newSpeed);
        }

    }
}