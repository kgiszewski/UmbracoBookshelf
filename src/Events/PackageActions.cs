using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace UmbracoBookshelf.Events
{
    public class PackageActions : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            if(string.IsNullOrEmpty(ConfigurationManager.AppSettings["UmbracoBookshelf:customFolder"]))
            {
                addDashboard();
                addLanguageKey();

                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                config.AppSettings.Settings.Add("UmbracoBookshelf:customFolder", "~/UmbracoBookshelf");
                config.Save();
            }
        }

        private void addDashboard()
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

        private void addLanguageKey()
        {
            var xd = new XmlDocument();

            xd.LoadXml(@"<Action runat='install' undo='true' alias='AddLanguageFileKey' language='en' position='beginning' area='sections' key='UmbracoBookshelf' value='Umbraco Bookshelf' />");

            LogHelper.Info<PackageActions>("Running Bookshelf language action.");
            PackageAction.RunPackageAction("UmbracoBookshelf", "AddLanguageFileKey", xd.FirstChild);
        }
    }
}