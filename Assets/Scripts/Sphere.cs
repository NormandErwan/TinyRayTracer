using System;
using Unity.Mathematics;
using UnityEngine;

namespace NormandErwan.TinyRayTracer
{
    /// <summary>
    /// Represents a 3D sphere.
    /// </summary>
    [Serializable]
    public struct Sphere
    {
        [SerializeField]
        private float3 center;

        [SerializeField]
        private float radius;

        /// <summary>
        /// Creates a new <see cref="Sphere"/>.
        /// </summary>
        /// <param name="center">The value of <see cref="Center"/>.</param>
        /// <param name="radius">The value of <see cref="Radius"/>.</param>
        public Sphere(float3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        /// <summary>
        /// The position of the center of this <see cref="Sphere"/>.
        /// </summary>
        float3 Center => center;

        /// <summary>
        /// The radius of this <see cref="Sphere"/>.
        /// </summary>
        float Radius => radius;
    }
}