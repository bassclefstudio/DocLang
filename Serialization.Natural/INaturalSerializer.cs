using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.NET.Serialization.Natural
{
    /// <summary>
    /// Provides a serialization/deserialization interface for objects that can be serialized naturally, meaning that they require little-to-no additional setup before being serialized and/or the resulting data should be human-writable.
    /// </summary>
    /// <typeparam name="T">The type of .NET objects being serialized and deserialized.</typeparam>
    /// <typeparam name="TData">The type of the data used to represent <typeparamref name="T"/> instances.</typeparam>
    public interface INaturalSerializer<T, TData>
    {
        /// <summary>
        /// Serializes a <typeparamref name="T"/> object and writes it to a <typeparamref name="TData"/> representation.
        /// </summary>
        /// <param name="content">The <typeparamref name="T"/> object being parsed.</param>
        /// <returns>A <typeparamref name="TData"/> object representing <paramref name="content"/>.</returns>
        public TData Write(T content);

        /// <summary>
        /// Deserializes data contained in an <typeparamref name="TData"/> representation and produces the described <typeparamref name="T"/> object.
        /// </summary>
        /// <param name="data">The <typeparamref name="TData"/> being parsed.</param>
        /// <returns>The <typeparamref name="T"/> object which <paramref name="data"/> represents.</returns>
        public T Read(TData data);
    }
}
