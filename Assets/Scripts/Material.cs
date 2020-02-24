using UnityEngine;
using Unity.Mathematics;

namespace NormandErwan.TinyRayTracer
{
    /// <summary>
    /// Represents shader data for <see cref="UnityEngine.Material"/>.
    /// </summary>
    public struct Material
    {
        /// <summary>
        /// Creates a new <see cref="Material"/> from a <see cref="UnityEngine.Material"/>.
        /// </summary>
        /// <param name="material">The source <see cref="UnityEngine.Material"/> to copy.</param>
        public Material(UnityEngine.Material material)
        {
            Color = (Vector4)material.color;
        }

        /// <summary>
        /// Gets the <see cref="UnityEngine.Material.color"/>.
        /// </summary>
        public float4 Color { get; }
    }
}