using Bit.OData.Contracts;
using System.Reflection;
using System.Web.OData.Builder;

namespace SampleApp.Api.Implementations
{
    public class SampleAppODataServiceBuilder : IODataServiceBuilder
    {
        public IAutoODataModelBuilder AutoODataModelBuilder { get; set; }

        public string GetODataRoute()
        {
            return "SampleApp";
        }

        public void BuildModel(ODataModelBuilder odataModelBuilder)
        {
            // odataModelBuilder is useful for advanced scenarios.
            AutoODataModelBuilder.AutoBuildODatModelFromAssembly(typeof(SampleAppODataServiceBuilder).GetTypeInfo().Assembly, odataModelBuilder);
        }
    }
}
