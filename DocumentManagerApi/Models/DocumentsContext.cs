using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DocumentManagerApi.Models
{
    public class DocumentsContext : DbContext
    {

        public DocumentsContext() : base("DocumentsContext")
        {
        }

        public System.Data.Entity.DbSet<DocumentManagerApi.Models.Document> Documents { get; set; }

    }
}