using System.Threading.Tasks;

namespace LinqToWikiTest1
{
    internal interface IApplication
    {
        Task Go(IAppEnviroment appEnviroment);
    }
}