using Microsoft.Extensions.DependencyInjection;

namespace MediaRelay.Url;


public delegate bool UrlValidator(Uri uri);


public static class UrlValidatorExtensions
{
    extension(IServiceCollection services)
    {
        public void AddUrlValidator(Func<IServiceProvider, UrlValidator> factory)
        {
            services.AddSingleton(factory);
        }
    }
}