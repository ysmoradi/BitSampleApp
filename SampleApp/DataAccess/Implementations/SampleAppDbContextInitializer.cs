using Bit.Core.Contracts;

namespace SampleApp.DataAccess.Implementations
{
    public class SampleAppDbContextInitializer : IAppEvents
    {
        public virtual void OnAppEnd()
        {

        }

        public virtual void OnAppStartup()
        {
            using (SampleAppDbContext dbContext = new SampleAppDbContext())
            {
                dbContext.Database.EnsureCreated();
            }
        }
    }
}
