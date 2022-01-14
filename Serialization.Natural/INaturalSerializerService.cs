using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Serialization.Natural
{
    /// <summary>
    /// Provides a service which can perform part of a serialization read/write operation on a <typeparamref name="T"/> object and its equivalent <typeparamref name="TData"/> representation.
    /// </summary>
    /// <typeparam name="T">The type of .NET objects being serialized and deserialized.</typeparam>
    /// <typeparam name="TData">The type of the data used to represent <typeparamref name="T"/> instances.</typeparam>
    public interface INaturalSerializerService<T, TData>
    {
        /// <summary>
        /// Gets a <see cref="uint"/> priority which defines in which order <see cref="INaturalSerializerService{T, TData}"/>s should read and write content. Generally, this is in descending order.
        /// </summary>
        uint Priority { get; }

        /// <summary>
        /// Reads content from the <typeparamref name="TData"/> data and adds it to the <typeparamref name="T"/> object being parsed.
        /// </summary>
        /// <param name="content">The <typeparamref name="T"/> currently being read/deserialized.</param>
        /// <param name="data">The <typeparamref name="TData"/> representing the desired state of <paramref name="content"/>.</param>
        /// <returns>A <see cref="bool"/> indicating whether the <see cref="INaturalSerializerService{T, TData}"/> was able to parse any of the provided data.</returns>
        bool Read(T content, TData data);

        /// <summary>
        /// Writes content from the <typeparamref name="T"/> object into the <typeparamref name="TData"/> associated representation.
        /// </summary>
        /// <param name="content">The <typeparamref name="T"/> object currently being written/serialized.</param>
        /// <param name="data">A <typeparamref name="TData"/> representation of <paramref name="content"/> that is being built.</param>
        /// <returns>A <see cref="bool"/> indicating whether the <see cref="INaturalSerializerService{T, TData}"/> was able to parse any of the provided data.</returns>
        bool Write(T content, TData data);
    }

    /// <summary>
    /// A base implementation of <see cref="INaturalSerializerService{T, TData}"/> with support for strongly-typed inputs.
    /// </summary>
    /// <typeparam name="T">The type of .NET objects being serialized and deserialized.</typeparam>
    /// <typeparam name="TActual">The actual derived type of <typeparamref name="T"/> objecs this <see cref="INaturalSerializerService{T, TData}"/> supports.</typeparam>
    /// <typeparam name="TData">The type of the data used to represent <typeparamref name="T"/> instances.</typeparam>
    /// <typeparam name="TDataActual">The actual derived type of <typeparamref name="TData"/> objecs this <see cref="INaturalSerializerService{T, TData}"/> supports.</typeparam>
    public abstract class NaturalSerializerService<T, TActual, TData, TDataActual> : INaturalSerializerService<T, TData> where TActual : T where TDataActual : TData
    {
        /// <inheritdoc/>
        public uint Priority { get; }

        /// <summary>
        /// Creates a new <see cref="NaturalSerializerService{T, TActual, TData, TDataActual}"/> with the given priority.
        /// </summary>
        /// <param name="priority">A <see cref="uint"/> priority which defines in which order <see cref="INaturalSerializerService{T, TData}"/>s should read and write content.</param>
        public NaturalSerializerService(uint priority = 0)
        {
            Priority = priority;
        }

        /// <inheritdoc/>
        public bool Read(T content, TData data)
        {
            if (content is TActual actualContent && data is TDataActual actualData)
            {
                return ReadInternal(actualContent, actualData);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc cref="Read(T, TData)"/>
        protected abstract bool ReadInternal(TActual content, TDataActual data);

        /// <inheritdoc/>
        public bool Write(T content, TData data)
        {
            if (content is TActual actualContent && data is TDataActual actualData)
            {
                return WriteInternal(actualContent, actualData);
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc cref="Write(T, TData)"/>
        protected abstract bool WriteInternal(TActual content, TDataActual data);

    }
}
