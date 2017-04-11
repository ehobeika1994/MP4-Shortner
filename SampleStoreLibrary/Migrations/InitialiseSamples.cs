using SampleStoreLibrary.Migrations;
using System.Data.Entity.Migrations;

namespace SampleStoreLibrary.Migrations
{
    public static class InitialiseSamples
    {
        public static void go()
        {
            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}
