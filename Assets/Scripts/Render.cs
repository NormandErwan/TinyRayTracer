using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace NormandErwan.TinyRayTracer
{
    [RequireComponent(typeof(Camera))]
    public sealed class Render : MonoBehaviour
    {
        private const int TextureDepth = 0;

        [SerializeField]
        private ComputeShader shader = default;

        [SerializeField]
        private string kernelName = default;

        [SerializeField]
        private List<Sphere> spheres = default;

        private new Camera camera;
        private ComputeKernel kernel;
        private int3 threadGroupsCount;

        /// <summary>
        /// Gets the <see cref="Sphere"/> collection to render.
        /// </summary>
        public List<Sphere> Spheres => spheres;

        /// <summary>
        /// Gets the <see cref="RenderTexture"/> result of the <see cref="Camera"/>.
        /// </summary>
        public RenderTexture Texture { get; private set; }

        private void Awake()
        {
            camera = GetComponent<Camera>();
            kernel = new ComputeKernel(shader, kernelName);

            SetTexture();
        }

        private void OnDestroy()
        {
            kernel.Dispose();
            Texture.Release();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            try
            {
                if (Texture.width != camera.pixelWidth || Texture.height != camera.pixelHeight)
                {
                    SetTexture();
                }

                shader.SetVector("BackgroundColor", camera.backgroundColor);
                shader.SetFloat("OrthographicSize", camera.orthographicSize);
                kernel.Set("Spheres", Spheres);
                kernel.Dispatch(threadGroupsCount);

                Graphics.Blit(Texture, destination);
            }
            finally
            {
                kernel.ReleaseBuffers();
            }
        }

        private void SetTexture()
        {
            if (Texture != null)
            {
                Texture.Release();
            }

            Texture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, TextureDepth)
            {
                enableRandomWrite = true
            };
            Texture.Create();

            kernel.Set("Texture", Texture);

            var threadGroups = math.ceil(new float2(Texture.width, Texture.height) / kernel.ThreadsCount.xy);
            threadGroupsCount = new int3((int2)threadGroups.xy, 1);
        }
    }
}
