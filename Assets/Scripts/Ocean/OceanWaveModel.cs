
using System;
using UnityEngine;

namespace Ocean
{
    [Serializable]
    public struct OceanWave
    {
        [Header("Wave Settings")]
        [Min(0f)] public float amplitude;
        [Min(0.1f)] public float wavelength;
        [Min(0f)] public float speed;

        [Range(0f, 1f)]
        public float steepness;

        public Vector2 direction;
    }

    public static class OceanWaveModel
    {
        public static Vector3 GetDisplacement(
            Vector3 position,
            float time,
            OceanWave[] waves)
        {
            Vector3 displacement = Vector3.zero;

            if (waves == null || waves.Length == 0)
                return displacement;

            foreach (OceanWave wave in waves)
            {
                if (wave.amplitude <= 0f ||
                    wave.wavelength <= 0f ||
                    wave.direction.sqrMagnitude < 0.0001f)
                    continue;

                Vector2 direction = wave.direction.normalized;

                float k = 2f * Mathf.PI / wave.wavelength;

                float phase = k * (
                    direction.x * position.x +
                    direction.y * position.z -
                    wave.speed * time
                );

                float horizontalAmplitude =
                    wave.steepness * wave.amplitude;

                displacement.x +=
                    direction.x *
                    horizontalAmplitude *
                    Mathf.Cos(phase);

                displacement.y +=
                    wave.amplitude *
                    Mathf.Sin(phase);

                displacement.z +=
                    direction.y *
                    horizontalAmplitude *
                    Mathf.Cos(phase);
            }

            return displacement;
        }

        public static float GetWaterHeight(
            Vector3 position,
            float time,
            OceanWave[] waves)
        {
            return GetDisplacement(position, time, waves).y;
        }
    }
}
