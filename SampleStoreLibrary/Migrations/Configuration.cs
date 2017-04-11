namespace SampleStoreLibrary.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SampleStoreLibrary.Models.SamplesContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SampleStoreLibrary.Models.SamplesContext context)
        {
            // below is what you need to copy into your own class
            context.Samples.AddOrUpdate(s => s.Title,
            new Sample
            {
                Title = "Giraffes",
                Genre = "Adventure",
                Mp4Blob = null,
                SampleMp4Blob = null,
                SampleMp4URL = null,
                CreatedDate = DateTime.Now
            },
            new Sample
            {
                Title = "Bee",
                Genre = "Adventure",
                Mp4Blob = null,
                SampleMp4Blob = null,
                SampleMp4URL = null,
                CreatedDate = DateTime.Now
            },
            new Sample
            {
                Title = "Ostrich",
                Genre = "Adventure",
                Mp4Blob = null,
                SampleMp4Blob = null,
                SampleMp4URL = null,
                CreatedDate = DateTime.Now
            },
            new Sample
            {
                Title = "Swan",
                Genre = "Adventure",
                Mp4Blob = null,
                SampleMp4Blob = null,
                SampleMp4URL = null,
                CreatedDate = DateTime.Now
            },
            new Sample
            {
                Title = "Giraffes",
                Genre = "Adventure",
                Mp4Blob = null,
                SampleMp4Blob = null,
                SampleMp4URL = null,
                CreatedDate = DateTime.Now
            },
            new Sample
            {
                Title = "Bee",
                Genre = "Adventure",
                Mp4Blob = null,
                SampleMp4Blob = null,
                SampleMp4URL = null,
                CreatedDate = DateTime.Now
            }
            );
        }
    }
}
