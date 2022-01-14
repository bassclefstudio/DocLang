using Autofac;
using BassClefStudio.DocLang.Content;
using BassClefStudio.NET.Serialization.Natural;
using System.Xml.Linq;

namespace BassClefStudio.DocLang.Xml
{
    /// <summary>
    /// Core implementation of <see cref="NaturalSerializerConfig{T, TData}"/> that serializes <see cref="IDocNode"/>s to DocLang XML.
    /// </summary>
    public class XmlParser : NaturalSerializerConfig<IDocNode, XNode>
    {
        /// <summary>
        /// Indicates the current <see cref="uint"/> version number of the latest DocLang XML schema.
        /// </summary>
        public const uint CurrentSchemaVersion = 1;

        /// <summary>
        /// Gets the <see cref="uint"/> schema version loaded for this <see cref="XmlParser"/>.
        /// </summary>
        public uint SchemaVersion { get; }

        /// <summary>
        /// Creates a new <see cref="XmlParser"/>.
        /// </summary>
        /// <param name="schemaVersion">The <see cref="uint"/> form of the schema version, used to select the correct type configuration in the DI container.</param>
        public XmlParser(uint schemaVersion = CurrentSchemaVersion)
        {
            SchemaVersion = schemaVersion;
        }

        /// <inheritdoc/>
        public override void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<XmlNaturalSerializer>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(XmlParser).Assembly)
                .AssignableTo<INaturalSerializerService<IDocNode, XNode>>()
                .SingleInstance()
                .AsImplementedInterfaces()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            //// Setup the default node (mixed-conent text) as TextNode:
            builder.ConfigureNode<TextNode, XText>(string.Empty, elementDelegate: c => new XText(string.Empty));
            //// Heading setup:
            builder.ConfigureElementNode<HeadingNode>("Heading");
            builder.ConfigureElementNode<Document>("Document");
            //// Content blocks:
            builder.ConfigureElementNode<ParagraphNode>("Paragraph");
        }
    }
}
