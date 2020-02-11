using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace NormandErwan.TinyRayTracer
{
    /// <summary>
    /// Represents a kernel function on a <see cref="ComputeShader"/>.
    /// </summary>
    public sealed class ComputeKernel : IDisposable, IEquatable<ComputeKernel>
    {
        /// <summary>
        /// List of instantiated buffers that will need to be released.
        /// </summary>
        private readonly Dictionary<string, ComputeBuffer> instanciatedBuffers;

        /// <summary>
        /// Finds the kernel function of a <see cref="ComputeShader"/> by its name.
        /// </summary>
        /// <param name="shader">The shader to use.</param>
        /// <param name="name">The name of the kernel function to find.</param>
        public ComputeKernel(ComputeShader shader, string name)
        {
            Name = name;
            Id = shader.FindKernel(name);
            Shader = shader;

            shader.GetKernelThreadGroupSizes(Id, out uint x, out uint y, out uint z);
            ThreadsCount = new uint3(x, y, z);

            instanciatedBuffers = new Dictionary<string, ComputeBuffer>();
        }

        /// <summary>
        /// Gets the index of the kernel function.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the name of the kernel function.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the shader program which belongs this kernel function.
        /// </summary>
        public ComputeShader Shader { get; }

        /// <summary>
        /// Gets the number of threads each instance of the kernel will use.
        /// </summary>
        /// <remarks>
        /// See: https://docs.unity3d.com/ScriptReference/ComputeShader.GetKernelThreadGroupSizes.html
        /// </remarks>
        public uint3 ThreadsCount { get; }

        /// <summary>
        /// Returns whether the current instance and a specified kernel are equal.
        /// </summary>
        public static bool operator ==(ComputeKernel left, ComputeKernel right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns whether the current instance and a specified kernel are different.
        /// </summary>
        public static bool operator !=(ComputeKernel left, ComputeKernel right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Executes the kernel function. Runs `X x Y x Z` groups of instances of the kernel.
        /// </summary>
        /// <param name="threadGroupCount">The number of groups of the kernel to run.</param>
        /// <remarks>
        /// See: https://docs.microsoft.com/en-us/windows/desktop/api/D3D11/nf-d3d11-id3d11devicecontext-dispatch
        /// </remarks>
        public void Dispatch(int3 threadGroupCount)
        {
            Shader.Dispatch(Id, threadGroupCount.x, threadGroupCount.y, threadGroupCount.z);
        }

        /// <summary>
        /// Executes the kernel function. Runs `X x Y x Z` groups of instances of the kernel.
        /// </summary>
        /// <param name="threadGroupCountX">The number of groups in the X dimension.</param>
        /// <param name="threadGroupCountY">The number of groups in the Y dimension.</param>
        /// <param name="threadGroupCountZ">The number of groups in the Z dimension.</param>
        public void Dispatch(int threadGroupCountX, int threadGroupCountY, int threadGroupCountZ)
        {
            Shader.Dispatch(Id, threadGroupCountX, threadGroupCountY, threadGroupCountZ);
        }

        /// <summary>
        /// Calls <see cref="ReleaseBuffers"/>.
        /// </summary>
        public void Dispose()
        {
            ReleaseBuffers();
        }

        /// <summary>
        /// Returns whether this instance and a specified <see cref="object"/> are equal.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns>
        /// `true` if the <paramref name="obj"/> parameter is a <see cref="ComputeKernel"/>, and its value is equal to
        /// the current instance; otherwise, `false`.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj is ComputeKernel other && Equals(other);
        }

        /// <summary>
        /// Returns whether this instance and the specified kernel are equal.
        /// </summary>
        /// <param name="other">The <see cref="ComputeKernel"/> to compare.</param>
        /// <returns>
        /// `true` if the <paramref name="other"/> parameter and the current instance have the same value;
        /// otherwise, `false`.
        /// </returns>
        public bool Equals(ComputeKernel other)
        {
            return Shader == other.Shader && Id == other.Id;
        }

        /// <summary>
        /// Gets a <see cref="ComputeBuffer"/> that has been
        /// <see cref="Set{T}(string, IEnumerable{T}, ComputeBufferType)"/> and which has been not
        /// <see cref="ComputeBuffer.Release"/>.
        /// </summary>
        /// <param name="bufferName">The name of the buffer variable on the <see cref="Shader"/>.</param>
        /// <returns>
        /// The current <see cref="ComputeBuffer"/> associated to <paramref name="bufferName"/> on the
        /// <see cref="Shader"/>.
        /// </returns>
        public ComputeBuffer GetBuffer(string bufferName)
        {
            return instanciatedBuffers[bufferName];
        }

        /// <summary>
        /// Gets data on the kernel from the specified buffer.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the array's values. Must be a
        /// <a href="https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types">blittable type</a>.
        /// </typeparam>
        /// <param name="bufferName">
        /// The name of the buffer variable on the <see cref="Shader"/> to get data from.
        /// </param>
        /// <returns>The data from the <see cref="Shader"/> buffer variable.</returns>
        public IEnumerable<T> Get<T>(string bufferName) where T : struct
        {
            var buffer = GetBuffer(bufferName);

            var data = new T[buffer.count];
            buffer.GetData(data);

            return data.AsEnumerable();
        }

        /// <summary>
        /// Gets data on the kernel from the specified buffer.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the array's values. Must be a <a href="https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types">blittable type</a>.
        /// </typeparam>
        /// <param name="bufferName">
        /// The name of the buffer variable on the <see cref="Shader"/> to get data from.
        /// </param>
        /// <param name="data">
        /// The array where to copy buffer's data. Must be the same size than the buffer
        /// (see: <see cref="ComputeBuffer.count"/>).
        /// </param>
        public void Get<T>(string bufferName, T[] data) where T : struct
        {
            var buffer = GetBuffer(bufferName);
            buffer.GetData(data);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code of this instance.</returns>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = (29 * hash) + Shader.GetHashCode();
            hash = (29 * hash) + Id.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns if a buffer has been
        /// <see cref="Set{T}(string, IEnumerable{T}, ComputeBufferType)"/> and that has not been
        /// <see cref="ComputeBuffer.Release"/>.
        /// </summary>
        /// <param name="bufferName">The name of the buffer variable on the <see cref="Shader"/> to find.</param>
        /// <returns>
        /// `true` if the buffer variable on the <see cref="Shader"/> has been initialized and not released;
        /// `false` otherwise.
        /// </returns>
        public bool HasBufferSet(string bufferName)
        {
            return instanciatedBuffers.ContainsKey(bufferName);
        }

        /// <summary>
        /// Releases the specified buffer.
        /// </summary>
        /// /// <param name="bufferName">The name of the buffer variable on the <see cref="Shader"/> to release.</param>
        public void ReleaseBuffer(string bufferName)
        {
            instanciatedBuffers[bufferName].Release();
            instanciatedBuffers.Remove(bufferName);
        }

        /// <summary>
        /// Releases all the buffers that have been
        /// <see cref="Set{T}(string, IEnumerable{T}, ComputeBufferType)"/> but not been
        /// <see cref="ComputeBuffer.Release"/>.
        /// </summary>
        public void ReleaseBuffers()
        {
            var buffersCopy = instanciatedBuffers.ToList();
            foreach (var buffer in buffersCopy)
            {
                buffer.Value.Release();
                instanciatedBuffers.Remove(buffer.Key);
            }
        }

        /// <summary>
        /// Sets an array parameter to the kernel.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the array's values. It must be a
        /// <a href="https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types">blittable type</a>
        /// and should has a
        /// <a href="https://developer.nvidia.com/content/understanding-structured-buffer-performance">size multiple of 16 bytes</a>.
        /// </typeparam>
        /// <param name="bufferName">
        /// The name of the buffer variable on the <see cref="Shader"/> to fill with <paramref name="data"/>.
        /// </param>
        /// <param name="data">The array to set to the <see cref="Shader"/> buffer variable.</param>
        /// <param name="bufferType">The type of the <see cref="ComputeBuffer"/>.</param>
        /// <returns>
        /// The <see cref="ComputeBuffer"/> created to bind the <paramref name="data"/> to the <see cref="Shader"/>
        /// buffer variable.
        /// </returns>
        public ComputeBuffer Set<T>(string bufferName, IEnumerable<T> data,
            ComputeBufferType bufferType = ComputeBufferType.Default) where T : struct
        {
            var dataArray = data.ToArray();
            int stride = Marshal.SizeOf(default(T));

            var buffer = new ComputeBuffer(dataArray.Length, stride, bufferType);
            buffer.SetData(dataArray);

            Shader.SetBuffer(Id, bufferName, buffer);

            instanciatedBuffers.Add(bufferName, buffer);
            return buffer;
        }

        /// <summary>
        /// Sets a texture parameter to the kernel.
        /// </summary>
        /// <param name="bufferName">
        /// The name of the buffer variable on the <see cref="Shader"/> to fill with <paramref name="texture"/>.
        /// </param>
        /// <param name="texture">The texture to set to the <see cref="Shader"/> buffer variable.</param>
        /// <remarks>
        /// The texture can be an input <see cref="Texture"/> that will be read by the kernel, or an output
        /// <see cref="Texture"/> that will be written by the kernel. For an output texture, it must be a
        /// <see cref="RenderTexture"/> with <see cref="RenderTexture.enableRandomWrite"/> flag set.
        /// </remarks>
        public void Set(string bufferName, Texture texture)
        {
            Shader.SetTexture(Id, bufferName, texture);
        }
    };
}
