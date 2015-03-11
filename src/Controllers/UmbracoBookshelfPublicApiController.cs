using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
using UmbracoBookshelf.Helpers;

namespace UmbracoBookshelf.Controllers
{
    [PluginController("UmbracoBookshelfApi")]
    public class UmbracoBookshelfPublicController : UmbracoApiController
    {
        [HttpGet]
        public object GetMedia(string filePath)
        { 
            var systemFilePath = WebUtility.UrlDecode(filePath).ToSystemPath();

            var httpResponseMessage = new HttpResponseMessage();

            using (var image = Image.FromFile(systemFilePath))
            {
                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, ImageFormat.Png);

                    httpResponseMessage.Content = new ByteArrayContent(memoryStream.ToArray());
                }
            }

            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            httpResponseMessage.StatusCode = HttpStatusCode.OK;

            return httpResponseMessage;
        }
    }
}