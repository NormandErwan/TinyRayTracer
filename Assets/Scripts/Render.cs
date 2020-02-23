using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace NormandErwan.TinyRayTracer
{
    public sealed class Render : MonoBehaviour
    {
        private const int TextureDepth = 0;

        [SerializeField]
        private ComputeShader shader = default;

        [SerializeField]
        private string kernelName = default;

        [SerializeField]
        private new Camera camera = default;

        [SerializeField]
        private RawImage image = default;

        [SerializeField]
        private List<Sphere> spheres = default;

        private ComputeKernel kernel;

        private int3 threadGroupsCount;

        /// <summary>
        /// Gets the <see cref="UnityEngine.Camera"/> thats render the scene.
        /// </summary>
        public Camera Camera => camera;

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
            kernel = new ComputeKernel(shader, kernelName);

            SetTexture();
        }

        private void OnDestroy()
        {
            kernel.Dispose();
            Texture.Release();
        }

        private void LateUpdate()
        {
            if (Texture.width != camera.pixelWidth || Texture.height != camera.pixelHeight)
            {
                SetTexture();
            }

            try
            {
                shader.SetVector("BackgroundColor", camera.backgroundColor);
                shader.SetFloat("OrthographicSize", camera.orthographicSize);
                kernel.Set("Spheres", Spheres);
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

            image.texture = Texture;
            kernel.Set("Texture", Texture);

            var threadGroups = math.ceil(new float2(Texture.width, Texture.height) / kernel.ThreadsCount.xy);
            threadGroupsCount = new int3((int2)threadGroups.xy, 1);
        }
    }
}
