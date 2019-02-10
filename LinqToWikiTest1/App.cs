using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToWikiTest1
{
    /// <summary>
    /// Duty - incapsulate a logic of application execution
    /// All logic of this application must be here
    /// </summary>
    internal class App : IApplication
    {
        public async Task Go(IAppEnviroment appEnviroment)
        {
            var appExecutionContext = await SetupAndBindAndConnetctAndEmbedToAppEnviroment(appEnviroment);
            var appConfig = await GetConfig(appExecutionContext);
            var services = await Init(appConfig);
            await Run(services);
        }

        async Task<IAppExecutionContext> SetupAndBindAndConnetctAndEmbedToAppEnviroment(IAppEnviroment appEnviroment)
        {
            return new AppExecutionContext();
        }

        async Task<AppConfig> GetConfig(IAppExecutionContext appExecutionContext)
        {
            return new AppConfig();
        }

        async Task<IServiceCollection> Init(AppConfig appConfig)
        {
            return new ServiceCollection();
        }

        async Task Run(IServiceCollection serviceCollection)
        {
            return;
        }
    }
}
