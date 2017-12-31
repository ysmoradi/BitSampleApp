using Bit.Test;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SampleApp.Test
{
    public class BitOwinTestEnvironment : TestEnvironmentBase
    {
        public BitOwinTestEnvironment(TestEnvironmentArgs args = null)
            : base(ApplyArgsDefaults(args))
        {

        }

        private static TestEnvironmentArgs ApplyArgsDefaults(TestEnvironmentArgs args)
        {
            args = args ?? new TestEnvironmentArgs();

            args.CustomDependenciesManagerProvider = args.CustomDependenciesManagerProvider ?? new SampleAppDependenciesManager();

            args.UseAspNetCore = true;

            return args;
        }

        protected override List<Func<TypeInfo, bool>> GetAutoProxyCreationIncludeRules()
        {
            List<Func<TypeInfo, bool>> baseList = base.GetAutoProxyCreationIncludeRules();

            baseList.Add(implementationType => implementationType.Assembly == typeof(AppStartup).GetTypeInfo().Assembly);

            return baseList;
        }
    }
}
