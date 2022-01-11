using Autofac;
using BassClefStudio.DocLang.Parsing;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang
{
    /// <summary>
    /// An <see cref="IParser{T}"/> interface for using a variety of available <see cref="IDocParser"/>s and other parsing services to save and load <see cref="Document"/> objects from their DocLang XML counterparts.
    /// </summary>
    public class DocLangParser : IDisposable
    {
        /// <summary>
        /// The <see cref="IContainer"/> DI container able to resolve various <see cref="IParser{T}"/> services.
        /// </summary>
        protected IContainer Container { get; }

        /// <summary>
        /// Creates a new <see cref="DocLangParser"/> and initializes the available services.
        /// </summary>
        public DocLangParser()
        {
            ContainerBuilder builder = new ContainerBuilder();
            ConfigureServices(builder);
            Container = builder.Build();
        }

        public virtual void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(DocLangParser).Assembly)
                .AsClosedTypesOf(typeof(IParser<>))
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<DocParserCollection>()
                .SingleInstance()
                .AsImplementedInterfaces();
        }

        /// <inheritdoc/>
        public Document Read(XDocument content)
        {
            Guard.IsNotNull(content.Root, nameof(content.Root));
            var node = Container.Resolve<IDocParserCollection>().Read(content.Root);
            if (node is Document document)
            {
                return document;
            }
            else
            {
                throw new InvalidOperationException($"Failed to parse {content} as a DocLang document");
            }
        }

        /// <inheritdoc/>
        public XDocument Write(Document node)
        {
            var rootElement = Container.Resolve<IDocParserCollection>().Write(node);
            return new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), rootElement);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
