using UnityEngine;
using Unity.Mathematics;

namespace NormandErwan.TinyRayTracer
{
    /// <summary>
    /// Represents shader data for <see cref="Camera"/>.
    /// </summary>
    public struct CameraData
    {
        /// <summary>
        /// Creates a new <see cref="CameraData"/> from a <see cref="Camera"/>.
        /// </summary>
        /// <param name="camera">The source <see cref="Camera"/> to copy.</param>
        public CameraData(Camera camera)
        {
            BackgroundColor = (Vector4)camera.backgroundColor;
            Forward = camera.transform.forward;
            OrthographicHeight = 4 * camera.orthographicSize;
            Position = camera.transform.position;
        }

        /// <summary>
        /// Gets the <see cref="Camera.backgroundColor"/>.
        /// </summary>
        float4 BackgroundColor;

        /// <summary>
        /// Gets the <see cref="Transform.forward"/> vector of the <see cref="Camera"/>.
        /// </summary>
        float3 Forward;

        /// <summary>
        /// Gets the <see cref="Camera.orthographicSize"/>.
        /// </summary>
        float OrthographicHeight;

        /// <summary>
        /// Gets the <see cref="Transform.position"/> of the <see cref="Camera"/>.
        /// </summary>
        float3 Position;
    }
}