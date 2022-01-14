using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Core implementation of <see cref="IDocParser"/> that performs XML parsing on <see cref="IDocNode"/>s using dependency injection to load required services.
    /// </summary>
    public class XmlParser : IDocParser, IDisposable
    {
        /// <summary>
        /// The Autofac <see cref="IContainer"/> DI container used to resolve parsing services.
        /// </summary>
        public IContainer Container { get; }

        /// <summary>
        /// Creates a new <see cref="XmlParser"/> with a configured DI <see cref="IContainer"/>.
        /// </summary>
        /// <param name="schema">The <see cref="IDocLangSchema"/> describing the schema which should be used by this parser for converting DocLang into XML.</param>
        public XmlParser(IDocLangSchema schema)
        {
            ContainerBuilder builder = new ContainerBuilder();
            ConfigureServices(builder);
            schema.ConfigureSchema(builder);
            Container = builder.Build();
        }

        /// <summary>
        /// Registers and configures various DI servies required for this <see cref="XmlParser"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        public virtual void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<DocParser>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(XmlParser).Assembly)
                .AssignableTo<IDocParseService>()
                .SingleInstance()
                .AsImplementedInterfaces()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }

        /// <inheritdoc/>
        public XNode Write(IDocNode node) => Container.Resolve<IDocParser>().Write(node);

        /// <inheritdoc/>
        public IDocNode Read(XNode data) => Container.Resolve<IDocParser>().Read(data);

        /// <inheritdoc/>
        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
