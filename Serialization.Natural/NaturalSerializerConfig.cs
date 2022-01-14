using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Serialization.Natural
{
    /// <summary>
    /// Provides a means through which a service can create an <see cref="INaturalSerializer{T, TData}"/> with dependency injection resources configured to serialize/deserialize a specific type according to a given schema.
    /// </summary>
    /// <typeparam name="T">The type of .NET objects being serialized and deserialized.</typeparam>
    /// <typeparam name="TData">The type of the data used to represent <typeparamref name="T"/> instances.</typeparam>
    public abstract class NaturalSerializerConfig<T, TData> : INaturalSerializer<T, TData>, IDisposable
    {
        /// <summary>
        /// The <see cref="IContainer"/> used to resolve serialization services internally.
        /// </summary>
        public IContainer Container { get; }

        /// <summary>
        /// Creates a new <see cref="NaturalSerializerConfig{T, TData}"/> and configures the DI container.
        /// </summary>
        public NaturalSerializerConfig()
        {
            ContainerBuilder builder = new ContainerBuilder();
            ConfigureServices(builder);
            Container = builder.Build();
        }

        /// <summary>
        /// Configure the <see cref="ContainerBuilder"/> to contain DI instructions for resolving all of the required <see cref="INaturalSerializer{T, TData}"/> and <see cref="INaturalSerializerService{T, TData}"/> services.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        public abstract void ConfigureServices(ContainerBuilder builder);

        /// <inheritdoc/>
        public T Read(TData data)
        {
            return Container.Resolve<INaturalSerializer<T, TData>>().Read(data);
        }

        /// <inheritdoc/>
        public TData Write(T content)
        {
            return Container.Resolve<INaturalSerializer<T, TData>>().Write(content);
        }

        /// <inheritdoc/>
        public void Dispose() => Container.Dispose();
    }

    /// <summary>
    /// Provides extension methods for configuring a <see cref="NaturalSerializerConfig"/> DI container for use in a <see cref="NaturalSerializer{T, TData, TKey, TDataKey}"/>.
    /// </summary>
    public static class NaturalSerializerConfigExtensions
    {
        /// <summary>
        /// Sets up a data type to be associated with the given <typeparamref name="TKey"/> key.
        /// </summary>
        /// <typeparam name="TKey">The type of key associated with each content object to correctly initialize the desired <typeparamref name="TData"/> data.</typeparam>
        /// <typeparam name="TData">The data type of serialized objects.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        /// <param name="dataType">The <see cref="Type"/> of the specific <typeparamref name="TData"/> object to create in this instance.</param>
        /// <param name="key">The <typeparamref name="TKey"/> to identify this <paramref name="dataType"/> in the serializer.</param>
        public static void SetupData<TKey, TData>(this ContainerBuilder builder, Type dataType, TKey key) where TKey : notnull where TData : notnull
        {
            builder.RegisterType(dataType).Keyed<TData>(key);
        }

        /// <summary>
        /// Sets up a data type to be associated with the given <typeparamref name="TKey"/> key.
        /// </summary>
        /// <typeparam name="TKey">The type of key associated with each content object to correctly initialize the desired <typeparamref name="TData"/> data.</typeparam>
        /// <typeparam name="TData">The data type of serialized objects.</typeparam>
        /// <typeparam name="TActual">The type of the specific <typeparamref name="TData"/> object to create in this instance.</param>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        /// <param name="key">The <typeparamref name="TKey"/> to identify this <paramref name="dataType"/> in the serializer.</param>
        public static void SetupData<TKey, TData, TActual>(this ContainerBuilder builder, TKey key) where TKey : notnull where TData : notnull where TActual : TData
        {
            builder.RegisterType<TActual>().Keyed<TData>(key);
        }

        /// <summary>
        /// Sets up a content type to be associated with the given <typeparamref name="TDataKey"/> key.
        /// </summary>
        /// <typeparam name="TDataKey">The type of key associated with each data node to correctly initialize the represented <typeparamref name="T"/> content.</typeparam>
        /// <typeparam name="T">The .NET type being serialized.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        /// <param name="contentType">The <see cref="Type"/> of the specific <typeparamref name="T"/> object to create in this instance.</param>
        /// <param name="key">The <typeparamref name="TDataKey"/> to identify this <paramref name="contentType"/> in the serializer.</param>
        public static void SetupContent<TDataKey, T>(this ContainerBuilder builder, Type contentType, TDataKey key) where TDataKey : notnull where T : notnull
        {
            builder.RegisterType(contentType).Keyed<T>(key);
        }

        /// <summary>
        /// Sets up a content type to be associated with the given <typeparamref name="TDataKey"/> key.
        /// </summary>
        /// <typeparam name="TDataKey">The type of key associated with each data node to correctly initialize the represented <typeparamref name="T"/> content.</typeparam>
        /// <typeparam name="T">The .NET type being serialized.</typeparam>
        /// <typeparam name="TActual">The type of the specific <typeparamref name="T"/> object to create in this instance.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        /// <param name="key">The <typeparamref name="TDataKey"/> to identify this <paramref name="contentType"/> in the serializer.</param>
        public static void SetupContent<TDataKey, T, TActual>(this ContainerBuilder builder, Type contentType, TDataKey key) where TDataKey : notnull where T : notnull where TActual : T
        {
            builder.RegisterType(contentType).Keyed<T>(key);
        }
    }
}
