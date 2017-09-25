using Cassette.Configuration;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace PDR.Web
{
    public class CassetteConfiguration : ICassetteConfiguration
    {
        public void Configure(BundleCollection bundles, CassetteSettings settings)
        {
            bundles.AddPerIndividualFile<StylesheetBundle>("Content/css");
            bundles.AddPerIndividualFile<StylesheetBundle>("Content/themes");
            bundles.AddPerIndividualFile<ScriptBundle>("Content/js");
        }
    }
}