using Autofac.Features.Indexed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Serialization.Natural
{
    /// <summary>
    /// A basic implementation of <see cref="INaturalSerializer{T, TData}"/> which uses a <typeparamref name="TKey"/> key and dependency injection to create and (de)serialize objects using a collection of child <see cref="INaturalSerializerService{T, TData}"/>s.
    /// </summary>
    /// <typeparam name="T">The type of .NET objects being serialized and deserialized.</typeparam>
    /// <typeparam name="TData">The type of the data used to represent <typeparamref name="T"/> instances.</typeparam>
    /// <typeparam name="TKey">The type of key that this <see cref="NaturalSerializer{T, TData, TKey, TDataKey}"/> can associate with each <typeparamref name="T"/> object to correctly initialize the desired <typeparamref name="TData"/> data.</typeparam>
    /// <typeparam name="TDataKey">The type of key that this <see cref="NaturalSerializer{T, TData, TKey, TDataKey}"/> can associate with each <typeparamref name="TData"/> object to correctly initialize the represented <typeparamref name="T"/> object.</typeparam>
    public abstract class NaturalSerializer<T, TData, TKey, TDataKey> : INaturalSerializer<T, TData>
    {
        /// <summary>
        /// An <see cref="IIndex{TKey, TValue}"/> associating <typeparamref name="TDataKey"/> keys on <typeparamref name="TData"/> data with factories for creating <typeparamref name="T"/> objects.
        /// </summary>
        public IIndex<TDataKey, Func<T>> ObjectIndex { get; }

        /// <summary>
        /// An <see cref="IIndex{TKey, TValue}"/> associating <typeparamref name="TKey"/> keys on <typeparamref name="T"/> objects with factories for creating <typeparamref name="TData"/> data nodes.
        /// </summary>
        public IIndex<TKey, Func<TData>> DataIndex { get; }

        /// <summary>
        /// A collection of child <see cref="INaturalSerializerService{T, TData}"/>s which can serialize and deserialize parts of the <typeparamref name="T"/> objects and <typeparamref name="TData"/>.
        /// </summary>
        public IEnumerable<INaturalSerializerService<T, TData>> Services { get; }

        /// <summary>
        /// Creates a new <see cref="NaturalSerializer{T, TData, TKey, TDataKey}"/> from the given DI services.
        /// </summary>
        /// <param name="services">A collection of child <see cref="INaturalSerializerService{T, TData}"/>s which can serialize and deserialize parts of the <typeparamref name="T"/> objects and <typeparamref name="TData"/>.</param>
        /// <param name="objects">An <see cref="IIndex{TKey, TValue}"/> associating <typeparamref name="TDataKey"/> keys on <typeparamref name="TData"/> data with factories for creating <typeparamref name="T"/> objects.</param>
        /// <param name="data">An <see cref="IIndex{TKey, TValue}"/> associating <typeparamref name="TKey"/> keys on <typeparamref name="T"/> objects with factories for creating <typeparamref name="TData"/> data nodes.</param>
        public NaturalSerializer(IEnumerable<INaturalSerializerService<T, TData>> services, IIndex<TDataKey, Func<T>> objects, IIndex<TKey, Func<TData>> data)
        {
            Services = services.OrderByDescending(s => s.Priority);
            ObjectIndex = objects;
            DataIndex = data;
        }

        /// <inheritdoc/>
        public T Read(TData data)
        {
            TDataKey key = GetKey(data);
            T content = ObjectIndex[key]();
            foreach (var service in Services)
            {
                service.Read(content, data);
            }
            return content;
        }


        /// <summary>
        /// Gets the <typeparamref name="TDataKey"/> key associated with the <typeparamref name="TData"/> data node.
        /// </summary>
        /// <param name="data">The <typeparamref name="TData"/> being queried.</param>
        /// <returns>A <typeparamref name="TDataKey"/> which can be used to create a corresponding <typeparamref name="T"/> object.</returns>
        protected abstract TDataKey GetKey(TData data);

        /// <inheritdoc/>
        public TData Write(T content)
        {
            TKey key = GetKey(content);
            TData data = DataIndex[key]();
            foreach (var service in Services)
            {
                service.Write(content, data);
            }
            return data;
        }

        /// <summary>
        /// Gets the <typeparamref name="TKey"/> key associated with the <typeparamref name="T"/> data node.
        /// </summary>
        /// <param name="content">The <typeparamref name="T"/> being queried.</param>
        /// <returns>A <typeparamref name="TKey"/> which can be used to create a corresponding <typeparamref name="TData"/> data node.</returns>
        protected abstract TKey GetKey(T content);
    }
}
