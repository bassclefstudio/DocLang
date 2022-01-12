using Autofac;
using BassClefStudio.DocLang.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing.Base
{
    /// <summary>
    /// Provides a base <see cref="IDocLangSchema"/> implementation that associates basic <see cref="string"/> element names and their DocLang <see cref="IDocNode"/> equivalents.
    /// </summary>
    public class BaseDocLangSchema : IDocLangSchema
    {
        /// <inheritdoc/>
        public virtual void ConfigureSchema(ContainerBuilder builder)
        {
            //// Setup the default node (mixed-conent text) as TextNode.
            builder.ConfigureNode<TextNode, XText>(string.Empty);
        }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="ContainerBuilder"/> class for adding <see cref="IDocLangSchema"/> configurations.
    /// </summary>
    public static class DocLangSchemaExtensions
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
                builder.Register<TNode>(nodeDelegate).Named<IDocNode>(name);
            }

            if (elementDelegate is null)
            {
                builder.RegisterType<TData>().Keyed<XNode>(typeof(TNode));
            }
            else
            {
                builder.Register<TData>(elementDelegate).Keyed<XNode>(typeof(TNode));
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
    }
}
