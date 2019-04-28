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
            using (IDependencyResolver dependencyResolver = DependencyManager.CreateChildDependencyResolver())
            {
                SampleAppDbContext dbContext = dependencyResolver.Resolve<SampleAppDbContext>();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}
