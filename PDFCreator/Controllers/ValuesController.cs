using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.IO;
using System.Web;
using System.Net.Http.Headers;
using PDFCreator.Models;

namespace PDFCreator.Controllers
{
    public class PDFController : ApiController
    {
        
        public HttpResponseMessage Get(int id)
        {
            try
            {
                
                var pdf = new PdfFactory(id);

                var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(pdf.GeneratedPdf) };

                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = id.ToString() + ".pdf" };

                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                return result;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(ex.ToString()) };
                throw;
            }
        }

    }
}
