using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DocumentManagerApi.Models
{
    public class Document
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int DocumentId { get; set; }

        [Required]
        public string DocumentFileName { get; set; }

        public string AzureFileReference { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string LastReviewed { get; set; }
    }
}