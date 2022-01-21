using Bit.Core.Contracts;

namespace SampleApp.DataAccess.Implementations
{
    public class SampleAppDbContextInitializer : IAppEvents
    {
        public virtual IDependencyManager DependencyManager { get; set; }

        public virtual void OnAppEnd()
        {

        }

        public virtual void OnAppStartup()
        {
            DependencyManager.TransactionAction("Create Database", async resolver =>
            {
                SampleAppDbContext dbContext = resolver.Resolve<SampleAppDbContext>();
                dbContext.Database.EnsureCreated();
            }).GetAwaiter().GetResult();
        }
    }
}
