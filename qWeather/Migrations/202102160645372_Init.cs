namespace qWeather.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WEATHER",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DATETIME = c.DateTime(nullable: false),
                        VAL1 = c.Single(),
                        VAL2 = c.Single(),
                        HUMIDITY = c.Single(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WEATHER");
        }
    }
}
