using System.Configuration;
using System.Xml;
using Examine;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using UmbracoBookshelf.Examine;

namespace UmbracoBookshelf.Events
{
    public class PackageActions : ApplicationEventHandler
    {
        private string _examineIndexConfigFilename
        {
            get
            {
                return IOHelper.MapPath("~/config/ExamineSettings.config");
            }
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            LogHelper.Info<BookshelfExamineDataService>(typeof(BookshelfExamineDataService).FullName);

            if(string.IsNullOrEmpty(ConfigurationManager.AppSettings["UmbracoBookshelf:customFolder"]))
            {
                _addDashboard();
                _addLanguageKey();

                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                config.AppSettings.Settings.Add("UmbracoBookshelf:customFolder", "~/UmbracoBookshelf");
                config.Save();
            }

            if (!_hasExamineConfig())
            {
                LogHelper.Info<PackageActions>("Adding Examine config updates...");

                _addExamineProvider();
            }
        }

        private void _addDashboard()
        {
            var xd = new XmlDocument();

            xd.LoadXml(@"<Action runat='install' alias='addDashboardSection' dashboardAlias='UmbracoBookshelfSection'>
	                            <section alias='UmbracoBookshelfSection'>
		                            <areas>
		                                <area>UmbracoBookshelf</area>
		                            </areas>
		                            <tab caption='Library'>
		                                <control>/app_plugins/umbracobookshelf/backoffice/dashboards/library.html</control>
		                            </tab>
	                            </section>
	                        </Action>");

            LogHelper.Info<PackageActions>("Running Bookshelf dashboard action.");
            PackageAction.RunPackageAction("UmbracoBookshelf", "addDashboardSection", xd.FirstChild);
        }

        private void _addLanguageKey()
        {
            var xd = new XmlDocument();

            xd.LoadXml(@"<Action runat='install' undo='true' alias='AddLanguageFileKey' language='en' position='beginning' area='sections' key='UmbracoBookshelf' value='Umbraco Bookshelf' />");

            LogHelper.Info<PackageActions>("Running Bookshelf language action.");
            PackageAction.RunPackageAction("UmbracoBookshelf", "AddLanguageFileKey", xd.FirstChild);
        }

        private bool _hasExamineConfig()
        {
            var xd = new XmlDocument();
            xd.Load(_examineIndexConfigFilename);

            return xd.SelectSingleNode("//ExamineIndexProviders/providers/add [@name = 'BookshelfIndexer']") != null;
        }

        private void _addExamineProvider()
        {
            var xd = new XmlDocument();
            xd.Load(_examineIndexConfigFilename);

            var provider = xd.SelectSingleNode("//ExamineIndexProviders/providers");

            var newXml = xd.CreateDocumentFragment();
            newXml.InnerXml = @"<add name=""BookshelfIndexer"" type=""Examine.LuceneEngine.Providers.SimpleDataIndexer, Examine"" dataService=""UmbracoBookshelf.Examine.BookshelfExamineDataService,UmbracoBookshelf"" indexTypes=""Bookshelf""/>";

            provider.AppendChild(newXml);

            newXml = xd.CreateDocumentFragment();
            newXml.InnerXml = @"<add name=""BookshelfSearcher"" type=""Examine.LuceneEngine.Providers.LuceneSearcher, Examine"" analyzer=""Lucene.Net.Analysis.Standard.StandardAnalyzer, Lucene.Net"" />";

            provider = xd.SelectSingleNode("//ExamineSearchProviders/providers");
            provider.AppendChild(newXml);

            xd.Save(_examineIndexConfigFilename);

            var indexFilename = IOHelper.MapPath("~/config/ExamineIndex.config");

            xd.Load(indexFilename);

            var indexSets = xd.SelectSingleNode("//ExamineLuceneIndexSets");

            newXml = xd.CreateDocumentFragment();
            newXml.InnerXml = @"<IndexSet SetName=""BookshelfIndexSet"" IndexPath=""~/App_Data/TEMP/ExamineIndexes/Bookshelf"">
    <IndexUserFields>
      <add Name=""id""/>
      <add Name=""book""/>
      <add Name=""path""/>
      <add Name=""title""/>
      <add Name=""text""/>
      <add Name=""url""/>
    </IndexUserFields>
  </IndexSet>";

            indexSets.AppendChild(newXml);

            xd.Save(indexFilename);
        }
    }
}