using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleStoreLibrary.Models
{
    public class Sample
    {
        public int SampleID { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Genre { get; set; }

        [StringLength(1024)]
        public string Mp4Blob { get; set; }

        [StringLength(1024)]
        public string SampleMp4Blob { get; set; }

        [StringLength(1024)]
        public string SampleMp4URL { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}
