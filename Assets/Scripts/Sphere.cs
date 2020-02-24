using Unity.Mathematics;
using UnityEngine;

namespace NormandErwan.TinyRayTracer
{
    /// <summary>
    /// Represents a 3D sphere.
    /// </summary>
    public sealed class Sphere : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The radius of this sphere")]
        private float radius = 1;

        /// <summary>
        /// Gets the center position of this <see cref="Sphere"/>.
        /// </summary>
        public float3 Center => transform.localPosition;

        /// <summary>
        /// Gets the radius of this <see cref="Sphere"/>.
        /// </summary>
        public float Radius => radius;

        /// <summary>
        /// Gets the shader <see cref="Data"/> of this <see cref="Sphere"/>.
        /// </summary>
        public Data ShaderData => new Data(this);

        /// <summary>
        /// Adds this <see cref="Sphere"/> to <see cref="RayTracer.Spheres"/>.
        /// </summary>
        private void Start()
        {
            RayTracer.Spheres.Add(this);
        }
        
        /// <summary>
        /// Removes this <see cref="Sphere"/> to <see cref="RayTracer.Spheres"/>.
        /// </summary>
        private void OnDisable()
        {
            RayTracer.Spheres.Remove(this);
        }
        
        /// <summary>
        /// Adds this <see cref="Sphere"/> to <see cref="RayTracer.Spheres"/>.
        /// </summary>
        private void OnEnable()
        {
            if (RayTracer.Spheres != null)
            {
                RayTracer.Spheres.Add(this);
            }
        }

        /// <summary>
        /// Sets <see cref="Transform.localScale"/> from <see cref="Radius"/>.
        /// </summary>
        private void OnValidate()
        {
            transform.localScale = new float3(Radius);
        }

        /// <summary>
        /// The shader data of <see cref="Sphere"/>.
        /// </summary>
        public struct Data
        {
            /// <summary>
            /// Creates a new <see cref="Data"/> from a <see cref="Sphere"/>.
            /// </summary>
            /// <param name="sphere">The <see cref="Sphere"/> from which extract shader data.</param>
            public Data(Sphere sphere)
            {
                Center = sphere.Center;
                Radius = sphere.Radius;
            }

            /// <summary>
            /// Gets <see cref="Sphere.Center"/>.
            /// </summary>
            public float3 Center { get; }

            /// <summary>
            /// Gets <see cref="Sphere.Radius"/>.
            /// </summary>
            public float Radius { get; }
        }
    }
}
