using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentManagerApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentManagerApi.Controllers
{
    public class DocumentsController : ApiController
    {
        private DocumentsContext db = new DocumentsContext();
        private String storageconn = ConfigurationManager.AppSettings["StorageConnectionString"];
        private String containerName = ConfigurationManager.AppSettings["TestContainerName"];

        [HttpGet]
        [Route("api/Download/{id}")]
        public async Task<HttpResponseMessage> DownloadBlobToFile(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            CloudBlobContainer container = getContainer(ConfigurationManager.AppSettings["TestContainerName"]);

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = db.Documents.Find(id);
                    CloudBlockBlob cloudBlockBlob = getBlockBlob(container, document.AzureFileReference);

                    cloudBlockBlob.DownloadToStream(memoryStream);

                    response.Content = new ByteArrayContent(memoryStream.ToArray());
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = document.DocumentFileName;
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(document.DocumentFileName));
                    response.Content.Headers.ContentLength = memoryStream.Length;
                }

                return response;
            }
            catch (Exception e)
            {
                return new HttpResponseMessage { Content = new StringContent(e.Message) };
            }
        }

        [HttpPost]
        [Route("api/Upload")]
        public async Task<HttpResponseMessage> UploadFile()
        {

            Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            CloudBlobContainer container = getContainer(ConfigurationManager.AppSettings["TestContainerName"]);

            container.CreateIfNotExists();

            var streamProvider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(streamProvider);
            
            String category = HttpContext.Current.Request.Params.Get("Category");
            String lastReviewed = HttpContext.Current.Request.Params.Get("LastReviewed");
            String filename = "";

            foreach (var file in streamProvider.Contents)
            {
                if (file.Headers.ContentDisposition.FileName != null)
                {
                    filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var buffer = await file.ReadAsByteArrayAsync();
                    files.Add(filename, buffer);
                }
            }
            
            try {
                string blobName = "philipJonas_blob_" + Guid.NewGuid().ToString();
                Stream stream = new MemoryStream(files.First().Value);

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
                
                blockBlob.UploadFromStream(stream);

                Document document = new Document();
                document.Category = category;
                document.LastReviewed = lastReviewed;
                document.DocumentFileName = filename;
                document.AzureFileReference = blobName;
                db.Documents.Add(document);
                db.SaveChanges();

                return new HttpResponseMessage { Content = new StringContent(document.DocumentId.ToString()) };
            }
            catch (Exception e) {
                return new HttpResponseMessage { Content = new StringContent(e.Message) };
            }
        }

        // GET: api/Documents
        public IQueryable<Document> GetDocuments()
        {
            return db.Documents;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DocumentExists(int id)
        {
            return db.Documents.Count(e => e.DocumentId == id) > 0;
        }

        private CloudBlobContainer getContainer(String containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageconn);
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = cloudBlobClient.GetContainerReference(containerName);

            return container;
        }

        private CloudBlockBlob getBlockBlob(CloudBlobContainer container, String blobName)
        {
            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(blobName);
            return cloudBlockBlob;
        }

        
    }
}