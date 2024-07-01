using IoLabs.ECH0160.Conversions;
using IoLabs.ECH0160.Conversions.Converters;
using IoLabs.ECH0160.Metadata;
using IoLabs.ECH0160.Metadata.Checksums;
using IoLabs.ECH0160.Normalizations;
using Microsoft.Extensions.DependencyInjection;

namespace IoLabs.ECH0160
{
    /// <summary>Extension methods for <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds eCH-0160 standard services.</summary>
        /// <param name="services">Services collection</param>
        /// <returns>Modified services collection</returns>
        public static IServiceCollection AddEch0160(this IServiceCollection services)
        {
            services.AddScoped<IFilesConversionManager, FilesConversionManager>();
            services.AddScoped<IPathNormalizer, PathNormalizer>();
            services.AddScoped<IHashCalculator, SHA256HashCalculator>();
            services.AddScoped<IMetadataGenerator, MetadataGenerator>();

            return services;
        }

        /// <summary>Adds a custom eCH-0160 standard converter.</summary>
        /// <typeparam name="T">Type of the converter</typeparam>
        /// <param name="services">Services collection</param>
        /// <returns>Modified services collection</returns>
        public static IServiceCollection AddEch0160Converter<T>(this IServiceCollection services) where T : class, IFileConverter
        {
            services.AddScoped<IFileConverter, T>();

            return services;
        }

        /// <summary>Adds a simple copy converter for eCH-0160 standard.</summary>
        /// <param name="services">Services collection</param>
        /// <returns>Modified services collection</returns>
        public static IServiceCollection AddEch0160BaseConverter(this IServiceCollection services)
        {
            services.AddEch0160Converter<SimpleCopyConverter>();

            return services;
        }
    }
}