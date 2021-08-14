using Bit.Core;
using Bit.Owin;
using Bit.Owin.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace SampleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            AssemblyContainer.Current.Init();

            await BuildWebHost(args)
                .RunAsync();
        }

        public static IHost BuildWebHost(string[] args) =>
            BitWebHost.CreateWebHost<AppStartup>(args)
                .Build();
    }
}
