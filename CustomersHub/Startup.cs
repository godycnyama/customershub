using CustomersHub.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

    [assembly: FunctionsStartup(typeof(CustomersHub.Startup))]
namespace CustomersHub
{
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ICustomersService, CustomersService>();
        }
    }
}
