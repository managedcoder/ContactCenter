using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ComposerComponents
{
    public class RestateOrderComponent : BotComponent
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Anything that could be done in Startup.ConfigureServices can be done here.

            // Make component available to Composer runtime through .NET dependency injection 
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<RestateOrder>(RestateOrder.Kind));
        }
    }
}

