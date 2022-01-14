using Autofac;
using BassClefStudio.NET.Serialization.Natural;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Xml
{
    /// <summary>
    /// A collection of extension methods for <see cref="ContainerBuilder"/> and data manipulation customized for use in the DocLang XML parser.
    /// </summary>
    public static class XmlParseExtensions
    {
        /// <summary>
        /// Attaches the given <typeparamref name="TNode"/> node and the <typeparamref name="TData"/> XML element types to a <see cref="string"/> DocLang element name.
        /// </summary>
        /// <typeparam name="TNode">The type of <see cref="IDocNode"/> being attached.</typeparam>
        /// <typeparam name="TData">The <see cref="XNode"/> XML element type being attached.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        /// <param name="name">The <see cref="string"/> DocLang name which associates and connects the <see cref="IDocNode"/> with its XML representation.</param>
        /// <param name="nodeDelegate">Optionally, a <see cref="Func{T, TResult}"/> which resolves a new <typeparamref name="TNode"/> node.</param>
        /// <param name="elementDelegate">Optionally, a <see cref="Func{T, TResult}"/> which resolves a new <typeparamref name="TData"/> element.</param>
        public static void ConfigureNode<TNode, TData>(this ContainerBuilder builder, string name, Func<IComponentContext, TNode>? nodeDelegate = null, Func<IComponentContext, TData>? elementDelegate = null) where TNode : IDocNode where TData : XNode
        {
            if (nodeDelegate is null)
            {
                builder.RegisterType<TNode>().Named<IDocNode>(name);
            }
            else
            {
                builder.Register(nodeDelegate).Named<IDocNode>(name);
            }

            if (elementDelegate is null)
            {
                builder.RegisterType<TData>().Keyed<XNode>(typeof(TNode));
            }
            else
            {
                builder.Register(elementDelegate).Keyed<XNode>(typeof(TNode));
            }
        }

        /// <summary>
        /// Attaches the given <see cref="IDocNode"/> node and the <see cref="XNode"/> XML element types to a <see cref="string"/> DocLang element name.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        /// <param name="name">The <see cref="string"/> DocLang name which associates and connects the <see cref="IDocNode"/> with its XML representation.</param>
        /// <param name="nodeType">The type of <see cref="IDocNode"/> being attached.</param>
        /// <param name="elementType">The <see cref="XNode"/> XML element type being attached.</param>
        /// <param name="nodeDelegate">Optionally, a <see cref="Func{T, TResult}"/> which resolves a new <see cref="IDocNode"/> node.</param>
        /// <param name="elementDelegate">Optionally, a <see cref="Func{T, TResult}"/> which resolves a new <see cref="XNode"/> element.</param>
        public static void ConfigureNode(this ContainerBuilder builder, string name, Type nodeType, Type elementType, Func<IComponentContext, IDocNode>? nodeDelegate = null, Func<IComponentContext, XNode>? elementDelegate = null)
        {
            if (nodeDelegate is null)
            {
                builder.RegisterType(nodeType).Named<IDocNode>(name);
            }
            else
            {
                builder.Register(nodeDelegate).Named<IDocNode>(name);
            }

            if (elementDelegate is null)
            {
                builder.RegisterType(elementType).Keyed<XNode>(nodeType);
            }
            else
            {
                builder.Register(elementDelegate).Keyed<XNode>(nodeType);
            }
        }

        /// <summary>
        /// Attaches the given <typeparamref name="TNode"/> node and an <see cref="XElement"/> to a <see cref="string"/> DocLang element name.
        /// </summary>
        /// <typeparam name="TNode">The type of <see cref="IDocNode"/> being attached.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        /// <param name="name">The <see cref="string"/> DocLang name which associates and connects the <see cref="IDocNode"/> with its XML representation. Also sets the <see cref="XElement"/>'s name.</param>
        /// <param name="nodeDelegate">Optionally, a <see cref="Func{T, TResult}"/> which resolves a new <typeparamref name="TNode"/> node.</param>
        public static void ConfigureElementNode<TNode>(this ContainerBuilder builder, string name, Func<IComponentContext, TNode>? nodeDelegate = null) where TNode : IDocNode => ConfigureNode<TNode, XElement>(builder, name, nodeDelegate, c => new XElement(name));

        /// <summary>
        /// Attaches the given <see cref="IDocNode"/> node and an <see cref="XElement"/> to a <see cref="string"/> DocLang element name.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        /// <param name="name">The <see cref="string"/> DocLang name which associates and connects the <see cref="IDocNode"/> with its XML representation. Also sets the <see cref="XElement"/>'s name.</param>
        /// <param name="nodeType">The type of <see cref="IDocNode"/> being attached.</param>
        /// <param name="nodeDelegate">Optionally, a <see cref="Func{T, TResult}"/> which resolves a new <see cref="IDocNode"/> node.</param>
        public static void ConfigureElementNode(this ContainerBuilder builder, string name, Type nodeType, Func<IComponentContext, IDocNode>? nodeDelegate = null) => ConfigureNode(builder, name, nodeType, typeof(XElement), nodeDelegate, c => new XElement(name));

        /// <summary>
        /// Enforces the presence of a child <see cref="XElement"/> with a given <see cref="string"/> key.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> XML parent element.</param>
        /// <param name="key">The required <see cref="string"/> name of the element.</param>
        /// <returns>The <see cref="XElement"/> child of <paramref name="parent"/> with the key/name <paramref name="key"/>.</returns>
        public static XElement EnforceElement(this XContainer parent, string key)
        {
            var child = parent.Element(key);
            if (child is null)
            {
                throw new ParseSchemaException($"Expected required element {key} in XML {parent}.");
            }
            return child;
        }

        /// <summary>
        /// Enforces the presence of a child <see cref="XAttribute"/> with a given <see cref="string"/> key.
        /// </summary>
        /// <param name="parent">The <see cref="XElement"/> XML parent element.</param>
        /// <param name="key">The required <see cref="string"/> name of the element.</param>
        /// <returns>The <see cref="XAttribute"/> child of <paramref name="parent"/> with the key/name <paramref name="key"/>.</returns>
        public static XAttribute EnforceAttribute(this XElement parent, string key)
        {
            var child = parent.Attribute(key);
            if (child is null)
            {
                throw new ParseSchemaException($"Expected required attribute {key} in XML {parent}.");
            }
            return child;
        }

        /// <summary>
        /// Enforces the presence of at least one child <see cref="XElement"/> with a given <see cref="string"/> key.
        /// </summary>
        /// <param name="parent">The <see cref="XContainer"/> XML parent element.</param>
        /// <param name="key">The required <see cref="string"/> name of the element.</param>
        /// <returns>The collection of <see cref="XElement"/> children of <paramref name="parent"/> with the key/name <paramref name="key"/>.</returns>
        public static IEnumerable<XElement> EnforceElements(this XContainer parent, string key)
        {
            var children = parent.Elements(key);
            if (!children.Any())
            {
                throw new ParseSchemaException($"Expected one or more required {key} elements in XML {parent}.");
            }
            return children;
        }
    }
}
