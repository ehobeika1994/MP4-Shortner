using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleStoreLibrary.Models
{
    public class SamplesContext : DbContext
    {
        public SamplesContext() : base("SamplesContext")
        {
        }

        public SamplesContext(string connString) : base(connString)
        {
        }

        public System.Data.Entity.DbSet<SampleStoreLibrary.Models.Sample> Samples { get; set; }

    }
}