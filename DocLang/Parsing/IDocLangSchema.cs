using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DocLang.Parsing
{
    /// <summary>
    /// Provides a means for configuring an <see cref="XmlParser"/> to associate certain <see cref="Type"/>s of DocLang objects with their respective XML elements.
    /// </summary>
    public interface IDocLangSchema
    {
        /// <summary>
        /// Registers and configures various DI servies required for this <see cref="XmlParser"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> responsible for building the DI container.</param>
        void ConfigureSchema(ContainerBuilder builder);
    }
}
