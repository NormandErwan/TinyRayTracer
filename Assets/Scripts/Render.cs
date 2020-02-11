using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.TinyRayTracer
{
    public sealed class Render : MonoBehaviour
    {
        public static readonly uint2 DefaultSize = new uint2(1024, 1024);

        private const int TextureDepth = 0;

        [SerializeField]
        private ComputeShader shader = default;

        [SerializeField]
        private string kernelName = default;

        [SerializeField]
        private RawImage image = default;

        [SerializeField]
        private uint2 size = DefaultSize;

        private ComputeKernel kernel;

        private int3 threadGroupsCount;

        public RenderTexture Texture { get; private set; }

        private void Awake()
        {
            kernel = new ComputeKernel(shader, kernelName);

            Texture = new RenderTexture((int)size.x, (int)size.y, TextureDepth)
            {
                enableRandomWrite = true
            };
            Texture.Create();

            image.texture = Texture;
            kernel.Set("Texture", Texture);

            threadGroupsCount = new int3((int2)(size.xy / kernel.ThreadsCount.xy), 1);
        }

        private void OnDestroy()
        {
            kernel.Dispose();
            Texture.Release();
        }

        private void LateUpdate()
        {
            try
            {
                kernel.Dispatch(threadGroupsCount);
            }
            finally
            {
                kernel.ReleaseBuffers();
            }
        }

        /// <summary>
        /// Calls <see cref="GL.Clear(bool, bool, Color)"/> on <see cref="Texture"/>.
        /// </summary>
        private void Clear()
        {
            var active = RenderTexture.active;

            RenderTexture.active = Texture;
            GL.Clear(clearDepth: true, clearColor: true, backgroundColor: Color.black);

            RenderTexture.active = active;
        }
    }
}