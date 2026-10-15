using UnityEngine;

namespace Ocean
{
    public class OceanController : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private OceanWave[] waves =
        {
            new OceanWave
            {
                amplitude = 0.18f,
                wavelength = 7f,
                speed = 1.2f,
                steepness = 0.25f,
                direction = new Vector2(1f, 0.3f)
            },
            new OceanWave
            {
                amplitude = 0.10f,
                wavelength = 4f,
                speed = 0.8f,
                steepness = 0.15f,
                direction = new Vector2(-0.4f, 1f)
            },
            new OceanWave
            {
                amplitude = 0.05f,
                wavelength = 2.5f,
                speed = 0.5f,
                steepness = 0.10f,
                direction = new Vector2(0.7f, -0.5f)
            }
        };

        public float CurrentTime => Time.time;

        public Vector3 GetDisplacement(Vector3 position)
        {
            return OceanWaveModel.GetDisplacement(
                position,
                CurrentTime,
                waves
            );
        }

        public float GetWaterHeight(Vector3 position)
        {
            return OceanWaveModel.GetWaterHeight(
                position,
                CurrentTime,
                waves
            );
        }
    }
}