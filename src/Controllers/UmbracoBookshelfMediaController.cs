using System;
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
    //had to do a bit of a hack here b/c of the way image src tags request data without sending a token
    [Umbraco.Web.Mvc.UmbracoAuthorize]
    [PluginController("UmbracoBookshelfApi")]
    public class UmbracoBookshelfMediaController : UmbracoApiController
    {
        [HttpGet]
        public object GetMedia(string filePath)
        {
            var httpResponseMessage = new HttpResponseMessage();

            try
            {
                var systemFilePath = WebUtility.UrlDecode(filePath).ToSystemPath();

                var extension = Path.GetExtension(systemFilePath);

                if (!Helpers.Constants.ALLOWED_IMAGE_EXTENSIONS.Contains(extension))
                {
                    httpResponseMessage.StatusCode = HttpStatusCode.Forbidden;

                    return httpResponseMessage;
                }

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
            catch (Exception ex)
            {
                LogHelper.Error<Exception>(ex.Message, ex);

                httpResponseMessage.StatusCode = HttpStatusCode.NotFound;

                return httpResponseMessage;
            }
        }
    }
}