using eQuantic.Core.Ioc.Scanning;
using Microsoft.Extensions.DependencyInjection;

namespace eQuantic.Core.Ioc.Conventions
{
    // SAMPLE: IRegistrationConvention
    /// <summary>
    /// Used to create custom type scanning conventions
    /// </summary>
    public interface IRegistrationConvention
    {
        void ScanTypes(TypeSet types, IServiceCollection services);
    }

    // ENDSAMPLE
}