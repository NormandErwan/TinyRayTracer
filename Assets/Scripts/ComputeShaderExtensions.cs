using UnityEngine;

namespace NormandErwan.TinyRayTracer
{
    /// <summary>
    /// Extensions methods for <see cref="ComputeShader"/>.
    /// </summary>
    public static class ComputeShaderExtensions
    {
        /// <summary>
        /// Finds a <see cref="ComputeKernel"/> with the specified name.
        /// </summary>
        /// <param name="shader">The current shader.</param>
        /// <param name="kernelName">
        /// The name of the <see cref="ComputeKernel"/> to find on <paramref name="shader"/>.
        /// </param>
        /// <returns>The found <see cref="ComputeKernel"/>.</returns>
        public static ComputeKernel GetKernel(this ComputeShader shader, string kernelName)
        {
            return new ComputeKernel(shader, kernelName);
        }
    }
}
